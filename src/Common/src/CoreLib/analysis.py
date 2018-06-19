# Licensed to the .NET Foundation under one or more agreements.
# The .NET Foundation licenses this file to you under the MIT license.
# See the LICENSE file in the project root for more information.

import lldb
import shlex
import argparse
import os
import threading
import string
import json

class DbgEngine(threading.local):

    # get a list of the frames in the current stack
    # returns  - a list of DbgFrame objects from the current stack
    def get_current_stack(self):
        #try to get the stack via clrstack as this is the most acurate, but will not be available if sos is not loaded or the thread is purely unmanaged
        stack = self.get_current_clrstack()
        
        #if the clrstack was not available get the native stack        
        if stack is None or len(stack) == 0:
            stack = self.get_stack(self.target.GetProcess().GetSelectedThread())
        
        return stack
                                                                  
    # get a list of the frames in the specified threads stack
    # sbThread - the thread to retrieve stack frames for  (an lldb.SBThread object)
    # returns  - a list of DbgFrame objects from the current stack
    def get_stack(self, sbThread):                                                                                                                      
        #try to get the stack via clrstack as this is the most acurate, but will not be available if sos is not loaded or the thread is purely unmanaged
        stack = self.get_clrstack(sbThread)
                                                               
        #if the clrstack was not available get the native stack
        if stack is None or len(stack) == 0:
            stack = [DbgFrame.FromSBFrame(f) for f in sbThread.frames]
        
        return stack

    # get a list of the frames in the specified threads stack using clrstack to augment the managed frames
    # sbThread - the thread to retrieve stack frames for  (an lldb.SBThread object)
    # returns  - a list of DbgFrame objects from the current stack
    def get_clrstack(self, sbThread):
        proc = self.target.GetProcess()
        #get the originally selected thread so we can restore when we're done
        sbThreadOrig = proc.GetSelectedThread()
        #set the selected thread to the specified thread (clrstack can only operate on the current stack)
        proc.SetSelectedThread(sbThread)
        frames = self.get_current_clrstack()
        #restore the current thread to it's previous value
        proc.SetSelectedThread(sbThreadOrig)
        return frames
        
    # get a list of the frames in the current stack using clrstack to augment the managed frames
    # returns  - a list of DbgFrame objects from the current stack
    def get_current_clrstack(self):
        frames = []
        sos = SosInterpreter()
        clrstackLines = sos.clrstackF().splitlines();
        for line in clrstackLines:
            #try to split into three strings childsp, ip, callsite
            splitline = string.split(line, ' ', 2)

            #if the three strings are in the expected format
            if len(splitline) == 3 and self.__ishexstr(splitline[0]) and self.__ishexstr(splitline[1]) and len(splitline[2]) > 0:
                strIp = splitline[1]
                #remove any offset string from the frame so this doesn't get included in strMod or strMeth
                strFrame = string.split(splitline[2], ' + ', 1)[0]
                splitFrame = string.split(strFrame, '!', 1)
                strMod = splitFrame[0]
                strMeth = ''
                if len(splitFrame) > 1:
                    strMeth = splitFrame[1]
                dbgFrame = DbgFrame.FromStrs(strIp, strMod, strMeth)
                frames.append(dbgFrame)
        return frames;

    # find the first frame matching the supplied routine name
    # strRoutine - string name of the routine to find on the selected thread
    # returns    - a DbgFrame index of the first frame matching the supplied routine name
    #              if no matching frames are found None is returned
    def get_first_frame(self, strRoutine):
        frame = None
        matching = [f for f in self.get_current_stack() if strRoutine == f.strRoutine]
            
        if len(matching) > 0:
            frame = matching[0]
        return frame
    
    # evaluate the given expression for the given frame and return the result as a UINT
    # dbgFrame   - DbgFrame to evaluate the given expression on
    # strExpr    - Expression to be evaluated
    # returns    - The value of the evaluated expression, (zero if the expression is not available)
    def eval_uint(self, dbgFrame, strExpr):
        sbVal = dbgFrame.sbFrame.EvaluateExpression(strExpr)
        uint = sbVal.GetValueAsUnsigned()
        return uint

    # try to evaluate the given expression on the first frame matching the supplied routine
    # strRoutine  - The routine to find in the current stack, if the routine appears in the stack multiple times the first occurance will be chosen
    # strExpr     - The expression to evaluate 
    # returns     - The value of the evaluated expression.  Zero, if the specified routine is not found in the current stack or the value of the given
    #               expression is not available. 
    def tryget_frame_uint(self, strRoutine, strExpr):
        val = 0
        frame = self.get_first_frame(strRoutine)
        if frame is not None:
            val = self.eval_uint(frame, strExpr)
        return val

    def __ishexstr(self, str):
        return str is not None and len(str) > 0 and all(c in string.hexdigits for c in str)


g_dbg = DbgEngine()
g_bPrintDebug = False

def _dbg_write(str):
    if g_bPrintDebug:
        print str

def __lldb_init_module(debugger, internal_dict):    
    debugger.HandleCommand('command script add -f analysis.analyze analyze')
    debugger.HandleCommand('command script add -f analysis.btm btm')

def init_debugger(debugger):
    g_dbg.debugger = debugger
    g_dbg.interpreter = debugger.GetCommandInterpreter()
    g_dbg.target = debugger.GetSelectedTarget()

def analyze(debugger, command, result, internal_dict):
    argList = shlex.split(command)

    dictArgs = { }
    for i, arg in enumerate(argList):
        key = arg
        if key.startswith('-'):
            val = ''
            if i < len(argList) and not argList[i+1].startswith('-'):
                val = argList[i+1]
            dictArgs[key] = val

    bAsync = debugger.GetAsync()
    debugger.SetAsync(False)

    init_debugger(debugger)
    
    eng = AnalysisEngine(dictArgs)
    
    eng.add_analyzer(StackTriageAnalyzer())
    eng.add_analyzer(StopReasonAnalyzer())
    eng.add_analyzer(LastExceptionAnalyzer())
    eng.add_analyzer(HeapCorruptionAnalyzer())  
    
    #dissabling the all threads analyzer as this schema has been removed from dumpling db
    #eng.add_analyzer(AllThreadsAnalyzer())
    
    dictProps = { }

    eng.analyze(dictProps);
        
    if 'STOP_REASON' in dictProps:
        dictProps['FAILURE_HASH'] = dictProps['STOP_REASON']

    if 'FOLLOW_UP' in dictProps and dictProps['FOLLOW_UP'] == 'heap_corruption':
        dictProps['FAILURE_HASH'] = dictProps['FAILURE_HASH'] + '_HEAPCORRUPT'
    
    if ('FOLLOW_UP' not in dictProps or dictProps['FOLLOW_UP'] <> 'heap_corruption') and 'LAST_EXCEPTION_TYPE' in dictProps:
        dictProps['FAILURE_HASH'] = dictProps['FAILURE_HASH'] + '_' + dictProps['LAST_EXCEPTION_TYPE']

    if 'CORRUPT_ROOT_FRAME' in dictProps:
        dictProps['FAILURE_HASH'] = dictProps['FAILURE_HASH'] + '_' + dictProps['CORRUPT_ROOT_FRAME']
    elif 'FAULT_SYMBOL' in dictProps:
        dictProps['FAILURE_HASH'] = dictProps['FAILURE_HASH'] + '_' + dictProps['FAULT_SYMBOL']


    if '-o' in dictArgs:
        with open(dictArgs['-o'], 'w') as f:
            f.write(json.dumps(dictProps))

    if '-v' not in dictArgs:
        dictProps.pop('ALL_THREADS', None)
        

    for key in dictProps.keys():
        result.AppendMessage(" ")
        result.AppendMessage(key + ":")
        result.AppendMessage(dictProps[key])

    debugger.SetAsync(bAsync)


def btm(debugger, command, result, internal_dict):
    bAsync = debugger.GetAsync()
    debugger.SetAsync(False)
    
    init_debugger(debugger)
    
    lstFrame = g_dbg.get_current_stack()

    for i, frame in enumerate(lstFrame):
        result.AppendMessage(str(i) + "\t" + frame.strIp + " " + frame.strFullFrame)
    
    debugger.SetAsync(bAsync)

def _str_to_dict(str, delim = ':'):
    dictOut = { }
    for line in string.split(str, "\n"):
            keyVal = [s.strip() for s in string.split(line, delim, 1)]
            if len(keyVal) > 1:
                dictOut[keyVal[0]] = keyVal[1]
    return dictOut

class AnalysisEngine(object):
    def __init__(self, dictArgs):
        self.analyzers = []
        self.dictArgs = dictArgs

    def analyze(self, dictProps):
        for a in self.analyzers:
            a.analyze(dictProps, self.dictArgs)

    def add_analyzer(self, analyzer):
        self.analyzers.append(analyzer)

class SosInterpreter(object):
    def ip2md(self, strIp):
        strOut = self.run_command("ip2md " + strIp)
        return strOut
    
    def dumpclass(self, strClassPtr):
        strOut = self.run_command("sos DumpClass " + strClassPtr)
        return strOut

    def pe(self, bNested = False):
        cmd = 'pe'

        if bNested:
            cmd = cmd + ' -nested'

        strOut = self.run_command(cmd)
        return strOut

    def clrstackF(self):
        clrstackOut = self.run_command('clrstack -f')
        return clrstackOut

    def get_symbol(self, strIp):
        strRoutine='UNKNOWN'
        strModule='UNKNOWN'
        ip2mdOut = self.ip2md(strIp)
        ip2mdProps = _str_to_dict(ip2mdOut)
        if 'Method Name' in ip2mdProps: 
            strRoutine = ip2mdProps['Method Name'].split('(')[0]
            if 'Class' in ip2mdProps:
                classPtr = ip2mdProps['Class']
                if classPtr is not None and classPtr <> '':
                    classOut = self.dumpclass(classPtr)
                    classProps = _str_to_dict(classOut)
                    if 'File' in  classProps:
                        strFile = classProps['File']
                        strModule = string.rsplit(string.rsplit(strFile, '.', 1)[0], '/', 1)[1] 
        return strModule + '!' + strRoutine

    def run_command(self, strCmd):
        strOut = ""
        result = lldb.SBCommandReturnObject()
        g_dbg.interpreter.HandleCommand(strCmd, result)
        if result.Succeeded() and result.HasResult():
            _dbg_write("INFO: Command SUCCEEDED: '" + strCmd + "'")
            strOut = result.GetOutput()
            _dbg_write(result.GetOutput())
        else:
            print "ERROR: Command FAILED: '" + strCmd + "'"
            print result.GetError()
        return strOut
    
class DbgFrame(object):

    def __init__(self):
        self.sbFrame = None
        self.strIp = None
        self.strModule = None
        self.strRoutine = None
        self.strFullRoutine = None
        self.strFullFrame = None

    def __str__(self):
        return self.strFullFrame
    
    @staticmethod
    def FromSBFrame(sbFrame):
        frame = DbgFrame()
        frame.sbFrame = sbFrame

        strIp = string.rstrip(hex(sbFrame.addr.GetLoadAddress(g_dbg.target)), 'L')
        strModule = sbFrame.module.file.basename
        strRoutine = sbFrame.symbol.name 
        
        if (strModule is None or strModule == '') and (strRoutine is None or strRoutine == ''):
            tplFrame = DbgFrame.__tryget_managed_frame_info(strIp)
            strModule = tplFrame[0]
            strRoutine = tplFrame[1]

        frame.__populate_frame_strs(strIp, strModule, strRoutine)
        return frame

    @staticmethod
    def FromStrs(strIp, strModule, strFullRoutine):
        frame = DbgFrame()
        frame.__populate_frame_strs(strIp, strModule, strFullRoutine)
        return frame

    def __populate_frame_strs(self, strIp, strModule, strFullRoutine):
        self.strIp = strIp        
        self.strModule = strModule;
        self.strFullRoutine = strFullRoutine         
        
        if self.strModule is None or self.strModule == '' or self.strModule == '<unknown>':
            self.strModule = 'UNKNOWN'
            
        if self.strFullRoutine is None or self.strFullRoutine == '' or self.strFullRoutine == '<unknown>':
            self.strFullRoutine = 'UNKNOWN'
        
        self.strRoutine = string.split(self.strFullRoutine, '(')[0]        
        self.strFrame = self.strModule + '!' + self.strRoutine
        self.strFullFrame = self.strModule + '!' + self.strFullRoutine
        
    @staticmethod
    def __tryget_managed_frame_info(strIp):
        sos = SosInterpreter()     
        strModule = None
        strRoutine = None
        ip2mdOut = sos.ip2md(strIp)
        ip2mdProps = _str_to_dict(ip2mdOut)
        _dbg_write(str(ip2mdProps))
        if 'Method Name' in ip2mdProps: 
            strRoutine = ip2mdProps['Method Name']
            if 'Class' in ip2mdProps:
                classPtr = ip2mdProps['Class']
                if classPtr is not None and classPtr <> '':
                    classOut = sos.dumpclass(classPtr)
                    classProps = _str_to_dict(classOut)
                    _dbg_write(str(classProps))
                    if 'File' in  classProps:
                        strFile = classProps['File']
                        strModule = string.rsplit(string.rsplit(strFile, '.', 1)[0], '/', 1)[1] 
        return strModule, strRoutine

class DbgThread(object):
    def __init__(self, sbThread):
        self.Osid = sbThread.id
        self.Index = sbThread.idx
        self.Frames = g_dbg.get_stack(sbThread)
                  
    def ToDictionary(self):
        tmpDict = { }
        tmpDict["Osid"] = str(self.Osid)
        tmpDict["Index"] = str(self.Index)
        tmpDict["Frames"] = [str(f) for f in self.Frames];   
        return tmpDict;
                
    def __str__(self):
        return json.dumps(self.ToDictionary())

class StackTriageRule(object):
    """description of class"""
    def __init__(self):
        self.strFollowup = None
        self.strFrame = None
        self.strModule = None
        self.strRoutine = None
        self.bExactModule = False
        self.bExactRoutine = False
        self.bExactFrame = False
        
    def __init__(self, strTriage):
        self.load_from_triage_string(strTriage)

    #Assumes the triage string is in the valid format <strFrame>=
    def load_from_triage_string(self, strTriage):
        splitOnEq = string.split(strTriage, "=")
        self.strFrame = splitOnEq[0]
        self.strFollowup = string.strip(splitOnEq[1])
        splitOnBang = string.split(splitOnEq[0], "!")
        self.strModule = "*"
        self.strRoutine = "*"
        if(len(splitOnBang) > 1):
            self.strModule = splitOnBang[0]
            self.strRoutine = splitOnBang[1]
        elif self.strFrame.endswith("*"):
            self.strModule = self.strFrame.rstrip("*")
        elif self.strFrame.startswith("*"):
            self.strRoutine = self.strFrame.lstrip("*")
        else:
            self.strModule = self.strFrame

        self.bExactModule = "*" not in self.strModule
        self.bExactRoutine = "*" not in self.strRoutine
        self.bExactFrame = self.bExactModule and self.bExactRoutine


class StackTriageEngine(object):
    def __init__(self):
        self.dictExactFrame = { }
        self.dictExactModule = { }
        self.dictExactRoutine = { }
        self.lstWildRules = [ ]

    ## loads the specified rules into the triage engine
    ## lstRules - a list of rules to be added to the current triage engine
    def load_rules(self, lstRules):
        for r in lstRules:
            if(r.bExactFrame):
                self.dictExactFrame[r.strFrame] = r
            elif (r.bExactModule):
                self.add_to_multidict(self.dictExactModule, r.strModule, r)
            elif (r.bExactRoutine):
                self.add_to_multidict(self.dictExactRoutine, r.strRoutine, r)
            else:
                self.lstWildRules.append(r);
        self.sort_rules()

    ## finds the blame symbol for the specified stack
    ## lstFrame - list of frames in the stack to triage
    ## return - tuple (frame, rule) for the blamed symbol of the stack, 
    ##          or None if a blame symbol could not be determined for the stack
    def triage_stack(self, lstFrame):
        for frame in lstFrame:
            rule = self.find_matching_rule(frame)
            if (rule is None or string.strip(rule.strFollowup.lower()) <> 'ignore') and frame.strFullFrame <> 'UNKNOWN!UNKNOWN' and frame.strRoutine <> 'UNKNOWN' and frame.strModule <> 'UNKNOWN':
                return (frame, rule)
        return None
    
    ## finds the first rule matching the specified frame.  If no rules match None is returned
    ## frame - the frame to find matching rules for
    def find_matching_rule(self, frame):
        #initialze rule to none to return if no matching rules are found
        rule = None
        #check if frame matches exact rule
        if(frame.strFrame in self.dictExactFrame):
            rule = self.dictExactFrame[frame.strFrame];
        #check if frame matches rule with an exact module
        if (rule is None and frame.strModule in self.dictExactModule):
            ruleIdx = self.__find_indexof_first_match(frame.strRoutine, [rule.strRoutine for rule in self.dictExactModule[frame.strModule]])
            if (ruleIdx >= 0):
                rule = self.dictExactModule[frame.strModule][ruleIdx]
        #check if frame matches rule with an exact routine
        if (rule is None and frame.strRoutine in self.dictExactRoutine):
            ruleIdx = self.__find_indexof_first_match(frame.strRoutine, [rule.strModule for rule in self.dictExactRoutine[frame.strRoutine]])
            if (ruleIdx >= 0):
                rule = self.dictExactModule[frame.strModule][ruleIdx]
        #check if frame matches wildcard rule
        ruleIdx = self.__find_indexof_first_match(frame.strRoutine, [rule.strFrame for rule in self.lstWildRules])
        if (ruleIdx >= 0):
                rule = self.lstWildRules[ruleIdx]
        return rule

    ## private - finds the index of the first wildcard expression matching the specified string
    ##           str - string to find a matching expression
    ##           lstExpr - a list of expression to evaluate against the specified string
    def __find_indexof_first_match(self, str, lstExpr):
        for i, expr in enumerate(lstExpr):
            if self.__is_wildcard_match(str, expr):
                return i;
        return -1;
    
    ## private - determins if the specified string matches the speicified wild card expression
    ##           str - string to evaluage against the wild card expression
    ##           expr - wildcard expression using * to match any
    ##           returns - true if the specified string matches the given expression otherwise false
    def __is_wildcard_match(self, str, expr):
        match = False
        
        splitOnWild = string.split(expr, "*")
        
        findStartIdx = 0

        #if the expr doesn't start with * verify the start of the string
        if splitOnWild[0] <> "":
            if str.startswith(splitOnWild[0]):
                searchStartIdx = len(splitOnWild[0]);
            else:
                return False

        #match all the interior search strings
        for i in range(1, len(splitOnWild) - 1):
            #ignore "" as this corresponds to a ** in the triage string so we move to the next token
            if splitOnWild[i] <> "":
                matchIdx = string.find(str, splitOnWild[i], findStartIdx)
                #if the token was not found return false
                if  matchIdx == -1:
                        return False
                findStartIdx = matchIdx + len(splitOnWild[i])

        #if the expr doesn't end with * verify the end of the string
        if splitOnWild[len(splitOnWild) - 1] <> "":
            if not str.endswith(splitOnWild[len(splitOnWild) - 1]):
                return False

        #if we haven't returned yet all the search strings were found
        return True


    ## private - sorts all engine rules based of the order they should be evaluated.  In this case by their length ignoring wildcard symbols
    def sort_rules(self):
        for key in self.dictExactModule:
            self.dictExactModule[key] = sorted(self.dictExactModule[key], key=lambda rule: len(rule.strRoutine.strip("*")))
        
        for key in self.dictExactRoutine:
            self.dictExactRoutine[key] = sorted(self.dictExactRoutine[key], key=lambda rule: len(rule.strModule.strip("*")))
        
        self.lstWildRules = sorted(self.lstWildRules, key=lambda rule: len(rule.strModule.strip("*")))

    ## private - adds item to the specified multi-dictionary.  if the key doesn't exist creates a list value for the item
    def add_to_multidict(self, dict, key, val):
        if key in dict:
            dict[key].append(val)
        else:
            dict[key] = [ val ]

class StackTriageAnalyzer(AnalysisEngine):
    def __init__(self):
        self.stackTriageEng = StackTriageEngine()
        self.bLoaded = False


    def analyze(self, dictProps, dictArgs):
        #print " ".join((self.debugger, self.interpreter, self.target))

        if not self.bLoaded:
            self.load_triage_engine(dictArgs)

        #get the eventing thread stack
        lstFrame = g_dbg.get_current_stack()
        
        dictProps["FAULT_THREAD"] = str(g_dbg.target.GetProcess().GetSelectedThread())
        dictProps["FAULT_STACK"] = "\n".join([str(f) for f in lstFrame])

        #triage with the triage engine
        tplFrameRule = self.stackTriageEng.triage_stack(lstFrame)
        
        #if a tuple was returned 
        if tplFrameRule is not None:
            dictProps["FAULT_SYMBOL"] = tplFrameRule[0].strFrame
            # if the rule in the tuple is not null has a strFollowup
            if tplFrameRule[1] is not None and tplFrameRule[1].strFollowup is not None:
                dictProps["FOLLOW_UP"] = tplFrameRule[1].strFollowup
        else:
            dictProps["FAULT_SYMBOL"] = "UNKNOWN!UNKNOWN"


    def load_triage_engine(self, dictArgs):
        rules = []
        
        triageIni = 'triage.ini'

        if '-i' in dictArgs:
            triageIni = dictArgs['-i']

        self.load_rules_from_file(triageIni, rules)

        self.stackTriageEng.load_rules(rules)

    def load_rules_from_file(self, strPath, lstRules):
        with open(strPath) as f:
            #read all lines from the file
            rawlines = [line.rstrip('\n') for line in f]
        
        #filter all comment lines, blank lines and lines not containing a =
        ruleLines = [line for line in rawlines if len(line) <> 0 and line[0] <> '\n' and line[0] <> ';' and '=' in line]

        #create rule for each rule line
        lstRules.extend([StackTriageRule(line) for line in ruleLines])
        
class StopReasonAnalyzer(AnalysisEngine):
    def __init__(self):
        self.initVoid = None

    def analyze(self, dictProps, dictArgs):
        thread = g_dbg.target.GetProcess().GetSelectedThread()
        dictProps['STOP_REASON'] = string.replace(thread.GetStopDescription(200), 'signal ', '')


class LastExceptionAnalyzer(AnalysisEngine):
    def __init__(self):
        self.initVoid = None

    def analyze(self, dictProps, dictArgs):
        thread = g_dbg.target.GetProcess().GetSelectedThread()
        sos = SosInterpreter()
        peOut = sos.pe(True)
        dictProps['LAST_EXCEPTION'] = peOut
        peProps = _str_to_dict(peOut)
        if 'Exception type' in peProps and peProps['Exception type'] is not None:
            dictProps['LAST_EXCEPTION_TYPE'] = peProps['Exception type']
            



class HeapCorruptionAnalyzer(AnalysisEngine):
    def __init__(self):
        self.initVoid = None

    def analyze(self, dictProps, dictArgs):
        _dbg_write('RUNNING HEAP CORRUPTION ANALYZE')
        if 'FOLLOW_UP' in dictProps and dictProps['FOLLOW_UP'] == 'heap_corruption':
            #check if there is a stack walker frame on the faulting stack
            _dbg_write('HEAP CORRUPTION IS PRESENT')

            stackWalkFrame = g_dbg.get_first_frame('Thread::StackWalkFramesEx')
            if stackWalkFrame is not None:
                tid = g_dbg.eval_uint(stackWalkFrame,'this->m_OSThreadId')
    
                if not tid == 0:
                    thread = g_dbg.target.GetProcess().GetThreadByID(tid)
                    dictProps['CORRUPT_ROOT_THREAD'] = str(thread)
                
                pc = self._find_walker_pc_as_uint()
                pcHex = string.rstrip(hex(pc), 'L')
                dictProps['CORRUPT_ROOT_FRAME_PC'] = pcHex
                sos = SosInterpreter()
                dictProps['CORRUPT_ROOT_FRAME'] = sos.get_symbol(pcHex)


                
    
    def _find_walker_pc_as_uint(self):
        #need to search for the current stack walker pc starting with the StackWalkFramesEx and working up the stack
        #depending on optimizations and the exact location of the current GC values might or might not be available
        pc = g_dbg.tryget_frame_uint('Thread::StackWalkFramesEx', 'pRD->ControlPC')
        if pc == 0:
            pc = g_dbg.tryget_frame_uint('Thread::MakeStackwalkerCallback', 'pCF->pRD->ControlPC')
        if pc == 0:
            pc = g_dbg.tryget_frame_uint('GcStackCrawlCallBack', 'pCF->pRD->ControlPC')
        if pc == 0:
            pc = g_dbg.tryget_frame_uint('EECodeManager::EnumGcRefs', 'pRD->ControlPC')
        if pc == 0:
            pc = g_dbg.tryget_frame_uint('GcInfoDecoder::EnumerateLiveSlots', 'pRD->ControlPC')
        return pc

class AllThreadsAnalyzer(AnalysisEngine):
    def __init__(self):
        self.initVoid = None

    def analyze(self, dictProps, dictArgs):
        lstThread = [ ]

        proc = g_dbg.target.GetProcess()
       
        for i, t in enumerate(proc.threads):
            lstThread.append(DbgThread(t))

        dictProps['ALL_THREADS'] = json.dumps([t.ToDictionary() for t in lstThread])
