#!/usr/bin/python

# Licensed to the .NET Foundation under one or more agreements.
# The .NET Foundation licenses this file to you under the MIT license.
# See the LICENSE file in the project root for more information.


import argparse
import os
import zipfile
import string
import platform
import getpass 
import urllib    
import urllib2
import time
import json
import requests
import tempfile
import hashlib  
import zlib
import gzip
import threading
import multiprocessing
import datetime
import copy
import stat
import subprocess
import sys
import errno
import shutil
import io
import psutil

def _json_format(obj):
    return json.dumps(obj, sort_keys=True, indent=4, separators=(',', ': '))

def _json_format_tofile(obj, file):
    json.dump(obj, file, sort_keys=True, indent=4, separators=(',', ': '))

class Output:
    s_squelch=False
    s_verbose=False
    s_quiet=False
    s_noprompt=False
    s_logPath=''
    s_lock=threading.Lock()

    @staticmethod
    def Diagnostic(output):
        if not bool(Output.s_squelch) and bool(Output.s_verbose):
            Output.Print(output)
                 
    @staticmethod
    def Message(output):
        if not bool(Output.s_squelch):
            Output.Print(output)

    @staticmethod
    def Critical(output):
        Output.Print(output)

    @staticmethod
    def Print(output):
        Output.s_lock.acquire()

        try:
            # always print out essential information.
            if not Output.s_quiet:
                print output
        
            # sometimes amend our essential output to an existing log file.
            if(Output.s_logPath is not None and os.path.isfile(Output.s_logPath)):
                # Note: The file must exist.
                with open(Output.s_logPath, 'a') as log_file:
                    log_file.write(output + '\n')
        finally:
            Output.s_lock.release()
     
    @staticmethod
    def Prompt_YN(prompt):
        if Output.s_noprompt or Output.s_squelch:
            return True
        Output.s_lock.acquire()
        result = None
        try:
            while(result != 'y' and result != 'n'):
                result = raw_input(prompt + ' [Y/N]: ').lower()
        finally:
            Output.s_lock.release()
        return result == 'y'

class FileUtils:

    @staticmethod
    def _hash(path):
        hash = hashlib.sha1()
        with open(path, 'rb') as f:
            BLOCKSIZE = 1024 * 8
            buf = f.read(BLOCKSIZE)
            while len(buf) > 0:
                hash.update(buf)  
                buf = f.read(BLOCKSIZE)
        return hash.hexdigest()

    @staticmethod
    def _compress_and_hash(inpath, outpath):     
        FileUtils._ensure_parent_dir(outpath)
        with gzip.open(outpath, 'wb') as fComp:
            with open(inpath, 'rb') as fDecomp:
                shutil.copyfileobj(fDecomp, fComp)
        return FileUtils._hash(outpath);

    @staticmethod
    def _decompress(inpath, outpath):
        FileUtils._ensure_parent_dir(outpath)
        with gzip.open(inpath, 'rb') as fComp:
            with open(outpath, 'wb') as fDecomp:
                shutil.copyfileobj(fComp, fDecomp)

    @staticmethod    
    def _ensure_parent_dir(path):
        FileUtils._ensure_dir(os.path.dirname(path))
       
    @staticmethod    
    def _ensure_dir(path):
        #create the directory if it doesn't exist
        if not os.path.isdir(os.path.abspath(path)):
            try:
                os.makedirs(os.path.abspath(path))
            except:
                return

    @staticmethod
    def _enumerate_unique_files(paths):
        files = set()
        for p in paths:
            p = p.rstrip('\\')
            p = p.rstrip('/')
            abspath = os.path.abspath(p)
            if os.path.isdir(abspath):
                for dirpath, dirnames, filenames in os.walk(abspath):
                    for name in filenames:
                        subpath = os.path.join(dirpath, name)
                        files.add(subpath)
            elif os.path.isfile(abspath): 
                files.add(abspath)   
        return files
    
    @staticmethod
    def _try_remove(path):
        try:
            os.remove(path)
            return True
        except OSError as e:
            # errno.ENOENT = no such file or directory re-raise exception if a different error occured
            if e.errno != errno.ENOENT: 
                raise 
            return False
    
class DumplingService:
    def __init__(self, baseurl):
        self._dumplingUri = baseurl;

    def DownloadDebugger(self, outputdir):
        url = self._dumplingUri + 'api/tools/debug?'
                               
        osStr = platform.system().lower()

        qargs = { 'os': osStr }

        if osStr == 'linux':
            qargs['distro'] = platform.dist()[0].lower()
        elif osStr == 'windows':
            qargs['distro'] = platform.machine().lower()

        url = url + urllib.urlencode(qargs)

        Output.Message('downloading debugger for client %s'%('-'.join(qargs.values())))
                                                               
        Output.Diagnostic('   url: %s'%(url))
               
        response = requests.get(url);
                  
        response.raise_for_status()
        
        Output.Diagnostic('   response: %s'%(response))

        Output.Diagnostic('   headers: %s'%(response.headers))

        FileUtils._ensure_dir(outputdir)

        DumplingService._stream_zip_archive_from_response(response, outputdir)
        
        dbgPath = 'cdb.exe' if osStr == 'windows' else 'bin/lldb'

        dbgPath = os.path.join(outputdir, dbgPath)
        
        Output.Diagnostic('   dbgpath: %s'%(dbgPath))

        return  dbgPath                                                         

    def DownloadClientFile(self, filename, downdir):
        url = self._dumplingUri + 'api/client/' + filename;
                          
        Output.Diagnostic('   url: %s'%(url))
        
        response = requests.get(url);

        Output.Diagnostic('   response: %s'%(response))   

        response.raise_for_status()

        DumplingService._stream_file_from_response(response, os.path.join(downdir, filename))  
        
        Output.Message('downloaded %s'%(filename))   


    def GetDumplingManfiest(self, dumpid):
        url = self._dumplingUri  + 'api/dumplings/' + dumpid + '/manifest'

        Output.Message('retrieving dumpling %s manifest'%(dumpid))    

        Output.Diagnostic('   url: %s'%(url))

        response = requests.get(url);
                          
        Output.Diagnostic('   response: %s'%(response))
                                                          
        Output.Diagnostic('   content: %s'%(_json_format(response.json())))
        
        return response.json()

    def UploadArtifact(self, dumpid, localpath, hash, file):
        
        qargs = { 'hash': hash, 'localpath': localpath }
        
        url = self._dumplingUri  + 'api/'

        #only include the dumpid if the not None
        if dumpid is not None:
            url = url + 'dumplings/' + dumpid + '/'

        url = url + 'artifacts/uploads?' + urllib.urlencode(qargs)

        Output.Message('uploading artifact %s %s'%(hash, os.path.basename(localpath)))

        Output.Diagnostic('   url: %s'%(url))

        response = requests.post(url, data=file)

        Output.Diagnostic('   response: %s'%(response.content))

        response.raise_for_status()

    
    def DownloadArtifact(self, hash, downpath):  
        if os.path.isdir(downpath):
            self._dumpSvc.DowloadArtifactToDirectory(hash, downpath)

            return

        url = self._dumplingUri + 'api/artifacts/' + hash

        Output.Diagnostic('   url: %s'%(url))
        
        response = requests.get(url, stream=True)
                                                     
        Output.Diagnostic('   response: %s'%(response))
                                    
        response.raise_for_status()

        DumplingService._stream_compressed_file_from_response(response, hash, downpath)
        
    def DowloadArtifactToDirectory(self, hash, dirpath):
        url = self._dumplingUri + 'api/artifacts/' + hash

        Output.Diagnostic('   url: %s'%(url))
        
        response = requests.get(url, stream=True)
                                                     
        Output.Diagnostic('   response: %s'%(response))
                                    
        response.raise_for_status()

        #find the first dumpling-filename in the history of response headers, we need to look through the history b/c of the redirects involved
        filename = next((hist for hist in response.history if next((val for h, val in hist.headers if h == 'dumpling-filename'), None) is not None), tempfile.mktemp())
        
        downpath = os.path.join(dirpath, filename)
        
        DumplingService._stream_compressed_file_from_response(response, hash, downpath)
        
    def UploadDump(self, localpath, hash, origin, displayname, file):    
        dumplingid = self.CreateDump(hash, origin, displayname)

        qargs = { 'hash': hash, 'localpath': localpath  }

        url = self._dumplingUri + 'api/dumplings/uploads?' + str(urllib.urlencode(qargs))

        Output.Message('uploading artifact %s %s'%(hash, os.path.basename(localpath)))

        Output.Diagnostic('   url: %s'%(url))

        response = requests.post(url, data=file)
                                     
        Output.Diagnostic('   response: %s'%(response))
                    
        response.raise_for_status()

        return { 'dumplingId' : hash, 'opToken' : response.text }
    
    def CreateDump(self, hash, origin, displayname):
        qargs = { 'hash': hash, 'user': origin, 'displayname': displayname  }

        url = self._dumplingUri + 'api/dumplings/create?' + str(urllib.urlencode(qargs))
        
        Output.Message('creating dumpling dump %s'%(hash))

        Output.Diagnostic('   url: %s'%(url))
        
        response = requests.get(url)
                                     
        Output.Diagnostic('   response: %s'%(response))
                    
        response.raise_for_status()

        return { 'dumplingId' : hash, 'opToken' : response.text }



    def UpdateDumpProperties(self, dumplingid, dictProps):
        url = self._dumplingUri + 'api/dumplings/' + dumplingid + '/properties'
        
        Output.Diagnostic('   url: %s'%(url))

        Output.Diagnostic('   data: %s'%(_json_format(dictProps)))

        response = requests.post(url, data=dictProps)    

        response.raise_for_status()
                          
        Output.Diagnostic('   response: %s'%(response))

    @staticmethod
    def _stream_zip_archive_from_response(response, unpackdir):
        #write the zip archive a temp file
        tempPath = os.path.join(tempfile.gettempdir(), tempfile.mktemp())
        with open(tempPath, 'wb') as fd:
            for chunk in response.iter_content(1024*8):
                fd.write(chunk)
        
        with open(tempPath, 'rb') as tempFile:
            zip = zipfile.ZipFile(tempFile)
            for path in zip.namelist():
                Output.Diagnostic('extracting   ' + path)
                zip.extract(path, unpackdir)
            zip.close()
        
        os.remove(tempPath)
        
    @staticmethod
    def _stream_compressed_file_from_response(response, hash, path):
        #write the compressed blob to a temp file
        tempPath = os.path.join(tempfile.gettempdir(), tempfile.mktemp())
        hasher = hashlib.sha1()
        with open(tempPath, 'wb') as fd:    
            for chunk in response.iter_content(1024*8):
                hasher.update(chunk)
                fd.write(chunk)
        downhash = hasher.hexdigest()
        
        if downhash != hash:
            Output.Critical("ERROR: downloaded file did not match expected hash value Expected: %s Actual %s" % (hash, downhash))
        else:
            FileUtils._decompress(tempPath, path)
            Output.Message('downloaded artifact %s %s'%(hash, os.path.basename(path)))      

        os.remove(tempPath)
                   
    @staticmethod
    def _stream_file_from_response(response, path):
        FileUtils._ensure_parent_dir(path)
        #if the file already exists delete it and replace
        if os.path.isfile(path):
            os.remove(path)
        with open(path, 'wb') as fd:
            for chunk in response.iter_content(1024*8):
                fd.write(chunk)     
     
class Task:
    def __init__(self, func, args):
        self.completed = False
        self.func = func
        self.args = args
        self.result = None   
        self.exception = None
        self._condvar = threading.Condition(threading.Lock())

    def execute(self):
        try:
            self.result = self.func(*self.args[0:])
        except Exception as e:                              
            self.exception = e
            print "workitem failed with exception: %s" % e
        finally:
            self._condvar.acquire()
            self.completed = True
            self._condvar.notify_all()
            self._condvar.release()

    def wait(self, timeout = None):
        self._condvar.acquire()
        if not self.completed:
            self._condvar.wait(timeout)
        self._condvar.release()
    
    def await_result(self, timeout = None):
        self.wait(timeout)
        if self.exception:
            raise self.exception
        return self.result
        
      
class ThreadPool:
    s_MaxThreads = multiprocessing.cpu_count()

    def __init__(self, maxthreads = None):
        self._condvar = threading.Condition(threading.RLock())
        self._threadcount = 0
        self._availcount = 0
        self._queue = [ ]
        self._emptyevent = threading.Event()
        self._emptyevent.set()
        self._maxthreads = maxthreads or ThreadPool.s_MaxThreads
    
    def queue_work(self, func, args=()):
        self._condvar.acquire()
                          
        self._emptyevent.clear()
        
        task = Task(func, args)

        self._queue.append(task)
        
        if self._availcount == 0 and self._threadcount < self._maxthreads:
            self._threadcount += 1
            self._add_thread()

        self._condvar.notify()

        self._condvar.release()
        
        return task
                
    def wait_on_pending_work(self):
        self._emptyevent.wait()

    def _add_thread(self):
        thread = threading.Thread(target=self._process_queue_items, args=())
        thread.setDaemon(True)
        thread.start()

    def _process_queue_items(self):
        while True:
            self._condvar.acquire()
            if len(self._queue) == 0: 
                self._availcount = self._availcount + 1
                if self._availcount == self._threadcount:
                    self._emptyevent.set()
                self._condvar.wait()
                self._availcount = self._availcount - 1
            task = self._queue.pop(0)
            self._condvar.release()
            task.execute()
 
        
class FileTransferManager:

    def __init__(self, dumpSvc, maxthreads = None):
        self._hashmap = { }
        self._dumpSvc = dumpSvc
        self._threadpool = ThreadPool(maxthreads)
        self._threadpool._maxthreads
         
    def QueueFileDownload(self, hash, abspath):
        return self._threadpool.queue_work(self._dumpSvc.DownloadArtifact, args=(hash, abspath))
        
    def QueueFileUpload(self, dumpid, abspath):
        return self._threadpool.queue_work(self._compress_and_upload, args=(dumpid, abspath))

    def WaitForPendingTransfers(self):
        self._threadpool.wait_on_pending_work()

    def _compress_and_upload(self, dumpid, abspath):              
        hash = None
        Output.Diagnostic('uncompressed file size: %s Kb'%(str(os.path.getsize(abspath) / 1024)))
        tempPath = os.path.join(tempfile.gettempdir(), tempfile.mktemp())
        try:
            hash = FileUtils._compress_and_hash(abspath, tempPath)
            Output.Diagnostic('compressed file size:   %s Kb'%(str(os.path.getsize(tempPath) / 1024)))
            with open(tempPath, 'rb') as fUpld:
                self._dumpSvc.UploadArtifact(dumpid, abspath, hash, fUpld)   
        finally:
            try:
                os.remove(tempPath)
            except:
                Output.Message('WARNING: failed to remove temp file %s'%(tempPath))
        return hash  

    def UploadDump(self, dumppath, incpaths, origin, displayname):
        #
        hash = None                                                      
        Output.Message('processing dump file %s'%(dumppath))
        Output.Diagnostic('uncompressed file size: %s Kb'%(str(os.path.getsize(dumppath) / 1024)))
        tempPath = os.path.join(tempfile.gettempdir(), tempfile.mktemp())
        hash = FileUtils._compress_and_hash(dumppath, tempPath)
        Output.Diagnostic('compressed file size:   %s Kb'%(str(os.path.getsize(tempPath) / 1024)))
        with open(tempPath, 'rb') as fUpld:
            dumpData = self._dumpSvc.UploadDump(dumppath, hash, origin, displayname, fUpld)   
        os.remove(tempPath)
        return dumpData

class CommandProcessor:
    def __init__(self, filequeue, dumpSvc):
        self._dumpSvc = dumpSvc
        self._filequeue = filequeue

    def Process(self, config):
        if config.command == 'upload':
            self.Upload(config)
        elif config.command == 'update':
            self.Update(config)
        elif config.command == 'download':
            self.Download(config)
        elif config.command == 'config':
            self.Config(config)
        elif config.command == 'install':
            self.Install(config)
        elif config.command == 'debug':
            self.Debug(config)
        elif config.command == 'hang':
            self.Hang(config)
     
    def Install(self, config):
        
        #if the dumpling.py doesn't exist at the install path or update is specified
        if not os.path.isfile(os.path.join('dumpling.py', config.installpath)) or config.update:  
            self._dumpSvc.DownloadClientFile('dumpling.py', config.installpath) 


        if platform.system().lower() != 'windows':
            #create the shortcut link                      
            linkPath = os.path.expanduser('~/bin/dumpling')
            if not os.path.isfile(linkPath):
                FileUtils._ensure_parent_dir(linkPath)
                with open(linkPath, 'w') as link:
                    link.write('#!/bin/sh\npython %s "$@"'%(os.path.join('dumpling.py', config.installpath)))
            
        if config.full:    
            dbgdir = os.path.join(config.installpath, 'dbg')

            #if the dbg dir doesn't exist or update is specified
            if not os.path.isdir(dbgdir) or config.update: 
                
                #if we are updating delete the entire dbg directory
                if os.path.isdir(dbgdir):
                    shutil.rmtree(dbgdir)

                dbgPath = self._dumpSvc.DownloadDebugger(dbgdir)
                if platform.system().lower() != 'windows':
                     os.chmod(dbgPath, stat.S_IEXEC)
                Output.Message('Adding debugger settings dumpling config')
                DumplingConfig.SaveSettings(config.configpath, { 'dbgpath': dbgPath })
                Output.Message('Debugger successfully installed')

            #if the analysis.py doesn't exist or update is specified
            if not os.path.isfile(os.path.join('analysis.py', config.installpath)) or config.update:
                self._dumpSvc.DownloadClientFile('analysis.py', config.installpath) 
            
            #if the analysis.py doesn't exist or update is specified     
            if not os.path.isfile(os.path.join('triage.ini', config.installpath)) or config.update:
                self._dumpSvc.DownloadClientFile('triage.ini', config.installpath)  

            #if we're executing off a local copy of dumpling.py see if there is a dumpling.config.json next to it
            installconfigpath = os.path.join(config.installpath, 'dumpling.config.json')

            #if there is a dumpling config and no config is at the install location copy the file.
            if os.path.isfile(config.configpath) and not os.path.isfile(installconfigpath):
                shutil.copyfile(config.configpath, installconfigpath)

    def Config(self, config):
        if config.action == 'dump':
            loaded = DumplingConfig.Load(config.configpath)
            if loaded is None:
                Output.Message('no dumpling configuration file was found')            
            else:
                Output.Message(str(loaded))
        
        if config.action == 'save':
            Output.Message(str(config))
            config.Save(config.configpath)
                                                               
        if config.action == 'clear':
            if os.path.isfile(config.configpath) and Output.Prompt_YN('Delete file "%s"?'%(config.configpath)):
                os.remove(config.configpath)
                Output.Message('Configuration cleared') 
            else:
                Output.Message('Command aborted. No changes were made.')                                                                                                                       
    
    def Update(self, config):
        self.UpdateProperties(config.dumpid, config, None)

        if config.incpaths:
            for f in FileUtils._enumerate_unique_files(config.incpaths):
                self._filequeue.QueueFileUpload(config.dumpid, f)

    def Upload(self, config):
        dumpid = None
        
        #if nothing was specified to upload
        if config.dumppath is None and (config.incpaths is None or len(config.incpaths) == 0):
            Output.Critical('No artifacts or dumps were specified to upload, either --dumppath or --incpaths is required to upload')

        #if dumppath was specified call create dump and upload dump
        if config.dumppath is not None:
            self.UploadDump(config)
        else:
            self.UploadArtifacts(config)


    def UploadDump(self, config):
        config.dumppath = os.path.abspath(config.dumppath)

        if config.displayname is None:
            config.displayname = str('%s.%.7f'%(getpass.getuser().lower(), time.time()))

        dumpdata = self._filequeue.UploadDump(config.dumppath, config.incpaths, config.user, config.displayname)
        
        dumpid = dumpdata['dumplingId']   
         
        props = None if config.triage == 'none' else CommandProcessor._get_client_triage_properties()

        self.UpdateProperties(dumpid, config, props)

        if config.triage == 'full':
            self._triage_dump(dumpid, config.dumppath, config)

        #the paradigm of how uploading a dump works has changed.  Now that the dump is processed offline after the api 
        #returns we get back an operation token rather than a list of needed refpaths.  In the future this optoken will fetch
        #will be used in an api to check the opertions state, at that time we'll need to check the state then load the manifest 
        #to look for needed files, however since this is not available yet we'll ignore the optoken.
        requestpaths = set() #set(dumpdata['refPaths'])      
        
        incpaths = set()
          
        if not (config.incpaths is None or len(config.incpaths) == 0):
            incpaths = FileUtils._enumerate_unique_files(config.incpaths)
            #if the dump file is in the incpaths remove it as it has already been uploaded
            incpaths.discard(os.path.abspath(config.dumppath))
            requestpaths.difference_update(incpaths)

        if len(requestpaths) > 0:
            prompt = 'The dumpling service has requested the following additional files be uploaded:\n'
            for p in requestpaths:
                prompt += p + '\n'
            prompt += 'Allow upload of requested files?'
            if Output.Prompt_YN(prompt):
                incpaths.update(requestpaths)
    
        for f in incpaths:
            self._filequeue.QueueFileUpload(dumpid, f) 

        self._filequeue.WaitForPendingTransfers();
        
        Output.Message('dumplingid:  %s'%(dumpid))
        Output.Critical('%sapi/dumplings/archived/%s'%(config.url, dumpid ))

        return dumpid

    def UpdateProperties(self, dumpid, config, props):

        props = props or { }

        if config.properties is not None:
            for kvp in config.properties:
                if kvp is not None:
                    CommandProcessor._add_key_if_not_exists(props, kvp[0], kvp[1])
            
        
        if config.propfile is not None:
            loadedProps = json.load(config.propfile)              
            for kvp in loadedProps.iteritems():
                if kvp is not None:
                    CommandProcessor._add_key_if_not_exists(props, kvp[0], kvp[1]) 

        if len(props) > 0: 
            self._dumpSvc.UpdateDumpProperties(dumpid, props)      
                

    def UploadArtifacts(self, config):
        if config.incpaths:
            for f in FileUtils._enumerate_unique_files(config.incpaths):
                self._filequeue.QueueFileUpload(None, f)
        
        self._filequeue.WaitForPendingTransfers();
    
    def Download(self, config):
        
        #choose download path argument downpath takes precedents since downdir has a defualt
        path = config.downpath or config.downdir

        path = os.path.abspath(path)

        #determine the directory of the intended download 
        dir = os.path.dirname(path) if config.downpath else path
            
        #create the directory if it doesn't exist
        if not os.path.isdir(dir):
            FileUtils._ensure_dir(dir)
        
        if config.hash is not None:        
            self._filequeue.QueueFileDownload(config.hash, path)
            self._filequeue.WaitForPendingTransfers();

        elif config.symindex is not None:
            Output.Critical('downloading artifacts from index is not yet supported')
            #self._filequeue.QueueFileIndexDownload(config.symindex, abspath)

        elif config.dumpid is not None:  
            dumpManifest = self._dumpSvc.GetDumplingManfiest(config.dumpid)
            
            self._download_dump(dir, dumpManifest)

    def Debug(self, config):
        if config.dbgpath is None:
            Output.Critical('dbgpath must be specified either as an argument or in the dumpling config to use the debug command')
            return
        
        #get the dump manifest                           
        dumpManifest = self._dumpSvc.GetDumplingManfiest(config.dumpid)
        
        #if the dump OS is not debuggable on this system error and return
        if dumpManifest['os'] != platform.system().lower():
            Output.Critical('the specified dump can only be debugged on the %s platform'%(dumpManifest['oS']))
            return
        
        #find the dump path from the manifest
        dumppath = next((dumpart['relativePath'] for dumpart in dumpManifest['dumpArtifacts'] if dumpart['hash'] == dumpart['dumpId']), None)

        Output.Diagnostic('dumppath: %s'%(dumppath))

        #if there is no dump file found in the manifest error and return 
        if dumppath is None:
            Output.Critical('the specified dump does not have a dump file associated with it')
            return
           
        #donwload the dump
        dumplingDir = self._download_dump(config.downdir, dumpManifest)
           
        fulldumppath = os.path.join(dumplingDir, dumppath)      
            
        Output.Diagnostic('Dump Path:  %s'%(fulldumppath))
                                                                                         
        if platform.system().lower() == 'linux':   
                
            execImage = next((dumpart['relativePath'] for dumpart in dumpManifest['dumpArtifacts'] if dumpart['executableImage']), None)
            
            execImage = None if not execImage else os.path.join(dumplingDir, execImage)

            #if the executable image is not available error and return
            if not execImage or not os.path.isfile(execImage):
                Output.Critical('the executable image for specified core dump is not available')
                return

            searchPaths = { }

            #find all the local paths which have loaded modules under them            
            for dumpart in dumpManifest['dumpArtifacts']:
                searchDir = os.path.dirname(os.path.join(dumplingDir, dumpart['relativePath']))

                Output.Diagnostic('searchDir: %s'%str(searchDir))

                searchPaths[str(searchDir)] = None

            Output.Diagnostic('searchPaths: %s'%str(searchPaths))

            searchPathsStr = ' '.join(searchPaths.keys())
                                                    
            Output.Diagnostic('searchPathsStr: ' + searchPathsStr)

            config.dbgargs.extend(['-o', 'settings set target.exec-search-paths "%s"'%(searchPathsStr)])
                                                                                                                                        
            config.dbgargs.extend([ '-o', 'target create -c %s %s'%(fulldumppath, execImage) ]) 

            sosPath = os.path.join(os.path.dirname(config.dbgpath), 'libsosplugin.so')

            config.dbgargs.extend([ '-o', 'plugin load %s'%(sosPath) ])   


        Output.Diagnostic('Debugger path:  %s'%(config.dbgpath))
                                
        Output.Diagnostic('Debugger args:  %s'%(config.dbgargs))
        
        popdir = os.getcwd()

        os.chdir(os.path.dirname(fulldumppath))
        
        #load the dump in the debugger   
        CommandProcessor._load_dump_in_debugger(config.dbgpath, config.dbgargs)
        
        os.chdir(popdir)

    def _triage_dump(self, dumpid, dumppath, config):  
        if config.dbgpath is None:
            Output.Critical('dbgpath must be specified either as an argument or in the dumpling config to preform a full dump triage')
            return

        scriptPath = os.path.join(config.installpath, 'analysis.py')

        iniPath = os.path.join(config.installpath, 'triage.ini')

        sosPath = os.path.join(os.path.dirname(config.dbgpath), 'libsosplugin.so') 
        
        #if the debugger or the triage tooling is not found error and return
        if not os.path.isfile(scriptPath) or not os.path.isfile(iniPath) or not os.path.isfile(config.dbgpath):
            Output.Critical('unable to find necissary debugger and triage tooling, please ensure these componenets are intalled')
            return

        triageOut = os.path.join(tempfile.gettempdir(), tempfile.mktemp())

        #define the debugger commands to execute
        dbgcmds = []
        dbgcmds.append('target create --core %s' % dumppath)
        dbgcmds.append('plugin load %s' % sosPath)
        dbgcmds.append('command script import %s' % scriptPath)
        dbgcmds.append('analyze -i %s -o %s' % ( iniPath, triageOut ))
        dbgcmds.append('exit')

        #execute the debugger commands to triage the dump file
        CommandProcessor._load_debugger(config.dbgpath, dbgcmds)

        #if the debugger wrote out the triage output file as expected load it and update the dump properties
        if os.path.isfile(triageOut):
            #load the output of analyze
            with open(triageOut, 'r') as fTriage:
                propsDict = json.load(fTriage)
            
                if len(propsDict) > 0: 
                    self._dumpSvc.UpdateDumpProperties(dumpid, propsDict) 
        
            #delete the temporary triage props file
            os.remove(triageOut)
        #if the debugger did not write the triage output file message and return
        else:
            Output.Message('WARNING: Debugger triage analysis failed')

            
    def _download_dump(self, dir, dumpManifest):
        dumplingDir = os.path.join(dir, dumpManifest['displayName'])
            
        if not os.path.exists(dumplingDir):
            FileUtils._ensure_dir(dumplingDir)

        #download all the artifacts for the dump
        for da in dumpManifest['dumpArtifacts']:
            if 'hash' in da and 'relativePath' in da:
                hash = da['hash']
                relPath = da['relativePath']
                if hash and relPath:
                    self._filequeue.QueueFileDownload(hash, os.path.join(dumplingDir, relPath)) 
        
        #save the manifest at the root 
        manifestPath = os.path.join(dumplingDir, 'manifest.json')

        with open(manifestPath, 'w') as manFile:
            _json_format_tofile(dumpManifest, manFile)
 
        self._filequeue.WaitForPendingTransfers();

        return dumplingDir

    def Hang(self, config):
        if os.path.exists(config.dbgpath) and os.path.isdir(config.outpath):            
            process = psutil.Process(int(config.pid))
            name = process.name()
            parent_dump_folder = os.path.join(config.outpath, name + "." + str(config.pid))

            if os.path.exists(parent_dump_folder):
                shutil.rmtree(parent_dump_folder)

            os.makedirs(parent_dump_folder)
            Output.Message("Parent Process %s" % config.pid)
            CommandProcessor._create_hang_dump(config.pid, parent_dump_folder, config.dbgpath)

            for child_process in process.children(recursive=True):
                Output.Message("Child Process %s" % child_process.pid)
                CommandProcessor._create_hang_dump(str(child_process.pid), parent_dump_folder, config.dbgpath)
        else:
            path = config.dbgpath if os.path.exists(config.outpath) else config.outpath
            Output.Critical('Invalid Path %s' % path)

    @staticmethod         
    #TODO: Replace this with _load_debugger after refactoring callers
    def _load_dump_in_debugger(debuggerPath, debuggerCommands):

        procArgs = [ debuggerPath ]

        for dbgcmd in debuggerCommands:
            procArgs.append('-o')
            procArgs.append(dbgcmd)
                           
        Output.Diagnostic('Debugger command:  %s'%(' '.join(procArgs)))

        dbgproc = subprocess.Popen(procArgs)

        dbgproc.wait()
                  
    @staticmethod
    #TODO: This new method should replace the above _load_dump_in_debugger, however the callers of _load_dump_in_debugger must be
    #      refactored to accomidate the slight difference in functionality.
    def _load_debugger(debuggerPath, debuggerCommands):
                                  
        procArgs = [ str(debuggerPath) ]

        for dbgcmd in debuggerCommands:
            procArgs.append('-o')
            procArgs.append(str(dbgcmd))
                     
        dbgcmdline =  ' '.join(procArgs)   
  
        Output.Diagnostic('Debugger command: %s'%(dbgcmdline))

        #BUG: For now we have disabled piping stdout b/c this seems to cause a segfault in lldb (even when executed directly in the shell)
        #     this needs to be investigated and piping re-enabled so that we can properly filter this output
        proc = subprocess.Popen(procArgs) #, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
        
        out, err = proc.communicate()

        returncode = proc.returncode

        Output.Diagnostic(out)     

        #returncode = subprocess.call(procArgs);

        Output.Diagnostic('Debugger exit code %s' % returncode)

    
    @staticmethod
    def _get_client_triage_properties():
        dictProp = { }
        CommandProcessor._add_key_if_not_exists(dictProp, 'CLIENT_ARCHITECTURE', platform.machine())
        CommandProcessor._add_key_if_not_exists(dictProp, 'CLIENT_PROCESSOR', platform.processor())
        CommandProcessor._add_key_if_not_exists(dictProp, 'CLIENT_NAME', platform.node())
        CommandProcessor._add_key_if_not_exists(dictProp, 'CLIENT_OS', platform.system())           
        CommandProcessor._add_key_if_not_exists(dictProp, 'CLIENT_RELEASE', platform.release())     
        CommandProcessor._add_key_if_not_exists(dictProp, 'CLIENT_VERSION', platform.version())
        if platform.system() == 'Linux':
            distroTuple = platform.linux_distribution()
            CommandProcessor._add_key_if_not_exists(dictProp, 'CLIENT_DISTRO', distroTuple[0])
            CommandProcessor._add_key_if_not_exists(dictProp, 'CLIENT_DISTRO_VER', distroTuple[1])
            CommandProcessor._add_key_if_not_exists(dictProp, 'CLIENT_DISTRO_ID', distroTuple[2])
        return dictProp

    @staticmethod
    def _add_key_if_not_exists(dictProp, key, val):
        if not key in dictProp:
            dictProp[key] = val

    @staticmethod
    def _create_hang_dump(pid, outpath, debuggerpath):
        osStr = platform.system().lower()
        process = psutil.Process(int(pid))
        name = process.name()
        command = ""
        if osStr == 'linux':
            command = "./" + debuggerpath + " " + pid + " --name " + outpath + "\\" + name + "." + pid + ".dmp"
        elif osStr == 'windows':
            outputpath = outpath + "\\" + name + "." + pid + ".dmp"
            command = debuggerpath + " -p "+ pid + " -c " + '".dump /mA '+outputpath+';.detach;q"'
        else:
            Output.Critical('Hang Operation not supported on %s' % osStr)
            return

        Output.Message("Creating dump for %s" % pid)
        try:
            return_code = subprocess.call(command)
            Output.Diagnostic('Debugger exit code %s' % return_code)
        except OSError as e:
            Output.Critical('Not able to create Dump for process %s %s' %(pid, e))

def _get_default_dbgargs():
    if platform.system().lower() == 'windows':
        return [ '-z', '$(dumppath)' ]
    else:
        return [  ]

class DumplingConfig:

    s_unsaved_args = { 'action', 'command', 'configpath', 'verbose', 'squelch', 'noprompt' }
    s_default_args = { 'url': 'https://dumpling.int-dot.net/', 'installpath': os.path.join(os.path.expanduser('~'), '.dumpling'), 'dbgargs': _get_default_dbgargs() }
    def __init__(self, dictConfig):
        self.__dict__ = copy.copy(DumplingConfig.s_default_args)

        self.Merge(dictConfig)

    @staticmethod
    def Load(strpath):           
        if not os.path.isfile(strpath):
            return DumplingConfig({ })

        try:
            with open(strpath, 'r') as fconfig:
                dict = json.load(fconfig)
                return DumplingConfig(dict)
        except:
            return None

    @staticmethod
    def SaveSettings(strpath, dictSettings):
        config = DumplingConfig.Load(strpath)
        config.Merge(dictSettings)
        config.Save(strpath)

    def Merge(self, dictConfig):
        for key, value in dictConfig.iteritems():
            if value or key not in self.__dict__:
                self.__dict__[key] = value

    def Save(self, strpath):
        with open(strpath, 'w') as fconfig:
            json.dump(self._persistable_args(), fconfig, sort_keys=True, indent=4, separators=(',', ': '))  
            Output.Message('configuration saved to %s'%(strpath))

    def _persistable_args(self):
        return dict([(key, value) for key, value in self.__dict__.iteritems() if key not in DumplingConfig.s_unsaved_args and value])

    def __str__(self):
        return _json_format(self._persistable_args())

def _parse_key_value_pair(argStr):
    kvp = string.split(argStr, '=', 1)

    if len(kvp) != 2:
        raise argparse.ArgumentError('the specified property key value pair is invalid. ' + argstr)

    return kvp

def _parse_args(argv):
    sharedparser = argparse.ArgumentParser(add_help=False)
    
    sharedparser.add_argument('--verbose', default=False, action='store_true', help='indicates that  all critical, standard, and diagnostic messages should be output')

    sharedparser.add_argument('--squelch', default=False, action='store_true', help='indicates that only critical messages should be ouput')
                                 
    sharedparser.add_argument('--noprompt', default=False, action='store_true', help='suppress prompts for user input')

    sharedparser.add_argument('--logpath', type=str, help='the path to a log file for appending message output')

    sharedparser.add_argument('--url', type=str, help='url of the dumpling service for the connected client')

    sharedparser.add_argument('--configpath', type=str, default=os.path.join(os.path.dirname(os.path.abspath(__file__)), 'dumpling.config.json'), help='path to the saved dumpling client configuration file')

    parser = argparse.ArgumentParser(parents=[sharedparser], description='dumpling client for managing core files and interacting with the dumpling service')
    
    subparsers = parser.add_subparsers(title='command', dest='command')
    
    config_parser = subparsers.add_parser('config', parents=[sharedparser], help='command used for updating saved dumpling client configuration')                               
    
    config_parser.add_argument('action', choices=['dump', 'save', 'clear'], help='dumps the contents of the dumpling client configuration to the console')     
              
    config_parser.add_argument('--dbgpath', type=str, default=None, help='path to debugger to be used by the dumpling client for debugging and triage')
                                        
    config_parser.add_argument('--dbgargs', nargs='*', help='arguments to be passed to the debugger. NOTE: use $(dumppath) as a replacement token for the dumpfile to open in the debugger')

    upload_parser = subparsers.add_parser('upload', parents=[sharedparser], help='command used for uploading dumps and files to the dumpling service')

    upload_parser.add_argument('--dumppath', type=str, help='path to the dumpfile to be uploaded')
                                        
    upload_parser.add_argument('--displayname', type=str, default=None, help='the name to be displayed in reports for the uploaded dump.  This argument is ignored unless --dumppath is specified')

    upload_parser.add_argument('--user', type=str, default=getpass.getuser().lower(), help='The username to pass to the dumpling service.  This argument is ignored unless --dumppath is specified')
    
    upload_parser.add_argument('--triage', choices=['none', 'client', 'full'], default='client', help='specifies the triage info to be uploadeded with the dump')

    upload_parser.add_argument('--incpaths', nargs='*', type=str, help='paths to files or directories to be included in the upload')

    upload_parser.add_argument('--properties', nargs='*', type=_parse_key_value_pair, help='a list of properties to be associated with the dump in the format key=value', metavar='key=value')  
                                         
    upload_parser.add_argument('--propfile', type=argparse.FileType('r'), help='path to a file containing a json serialized dictionary of property value paires')

    download_parser = subparsers.add_parser('download', parents=[sharedparser], help='command used for downloading dumps and files from the dumpling service')    
    
    download_idtype = download_parser.add_mutually_exclusive_group(required=True)                                                                                             
    
    download_idtype.add_argument('--dumpid', type=str, help='the dumpling id of the dump to download for debugging')   

    download_idtype.add_argument('--hash', type=str, help='the id of the artifact to download')  

    download_idtype.add_argument('--symindex', type=str, help='the symstore index of the artifact to download')

    download_parser.add_argument('--downpath', type=str, help='the path to download the specified content to. NOTE: if both downpath and downdir are specified downdir will be ignored')

    download_parser.add_argument('--downdir', type=str, default=os.getcwd(), help='the path to the directory to download the specified content')    
    
    update_parser = subparsers.add_parser('update', parents=[sharedparser], help='command used for updating dump properties and associated files')
                                                                                                            
    update_parser.add_argument('--dumpid', type=str, help='the dumpling id the specified updates are to be associated with')
    
    update_parser.add_argument('--properties', nargs='*', type=_parse_key_value_pair, help='a list of properties and values to be associated with the dump in the format property=value', metavar='property=value')  
    
    update_parser.add_argument('--propfile', type=argparse.FileType('r'), help='path to a file containing a json serialized dictionary of property value pairs')

    update_parser.add_argument('--incpaths', nargs='*', type=str, help='paths to files or directories to be associated with the specified dump')
    
    install_parser = subparsers.add_parser('install', parents=[sharedparser], help='command used for installing dumpling services and support tooling')

    install_parser.add_argument('--full', default=None, action='store_true', help='indicates that all dumpling tools and scripting should be installed on the client') 
    
    install_parser.add_argument('--update', default=False, action='store_true', help='indicates that already installed components should be updated')

    install_parser.add_argument('--installpath', type=str, help='path to the root directory to install dumpling tooling')

    debug_parser = subparsers.add_parser('debug', parents=[sharedparser], help='download a dumpling dump and load it into the debugger')   
    
    debug_parser.add_argument('--dumpid', type=str, required=True, help='the dumpling id of the dump to download for debugging')   
    
    debug_parser.add_argument('--dbgargs', nargs='*', help='arguments to be passed to the debugger. NOTE: use $(dumppath) as a replacement token for the dumpfile to open in the debugger')

    debug_parser.add_argument('--dbgpath', type=str, default=None, help='path to debugger to be used by the dumpling client for debugging and triage')
                                                 
    debug_parser.add_argument('--downdir', type=str, default=os.getcwd(), help='the path to the directory to download the specified content')

    hung_parser = subparsers.add_parser('hang', parents=[sharedparser], help='Creating the dump for the hang or timeout process')   
    
    hung_parser.add_argument('--pid', type=str, required=True, help='the pid of the process')   
    
    hung_parser.add_argument('--dbgpath', type=str, required=True, help='path to debugger to be used by the dumpling client for creating dump')
                                                 
    hung_parser.add_argument('--outpath', type=str, default=os.getcwd(), help='the path to the directory for memory dump file')

    parsed_args = parser.parse_args(argv)

    config = DumplingConfig.Load(parsed_args.configpath) or DumplingConfig({ })
    
    config.Merge(parsed_args.__dict__)

    return config

def _create_command_processor(config):
    dumplingsvc = DumplingService(config.url)
    
    filequeue = FileTransferManager(dumplingsvc)
    
    return CommandProcessor(filequeue, dumplingsvc)

def _init_output(config):
    Output.s_verbose = config.verbose
    
    Output.s_squelch = config.squelch
    
    Output.s_logPath = config.logpath
    
    Output.s_noprompt = config.noprompt

def main(argv):
    
    starttime = datetime.datetime.now();

    config = _parse_args(argv[1:])
    
    _init_output(config)

    cmdProc = _create_command_processor(config)

    cmdProc.Process(config)

    Output.Message('total elapsed time %s'%(datetime.datetime.now() - starttime))

if __name__ == '__main__':
    main(sys.argv)


