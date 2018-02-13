// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Collections;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl.XsltOld.Debugger;
    using System.Diagnostics;
    using System.Runtime.Versioning;

    internal class DbgData
    {
        private XPathNavigator _styleSheet;
        private VariableAction[] _variables;
        public XPathNavigator StyleSheet { get { return _styleSheet; } }
        public VariableAction[] Variables { get { return _variables; } }
        public DbgData(Compiler compiler)
        {
            DbgCompiler dbgCompiler = (DbgCompiler)compiler;
            _styleSheet = dbgCompiler.Input.Navigator.Clone();
            _variables = dbgCompiler.LocalVariables;
            dbgCompiler.Debugger.OnInstructionCompile(this.StyleSheet);
        }
        internal void ReplaceVariables(VariableAction[] vars) { _variables = vars; }

        // static Empty:
        private static DbgData s_nullDbgData = new DbgData();
        private DbgData()
        {
            _styleSheet = null;
            _variables = Array.Empty<VariableAction>();
        }
        public static DbgData Empty { get { return s_nullDbgData; } }
    }

    internal class DbgCompiler : Compiler
    {
        private IXsltDebugger _debugger;

        public DbgCompiler(IXsltDebugger debugger)
        {
            _debugger = debugger;
        }
        public override IXsltDebugger Debugger { get { return _debugger; } }

        // Variables
        //
        // In XsltDebugger we have to know variables that are visible from each action.
        // We keepping two different sets of wariables because global and local variables have different rules of visibility.
        // Globals: All global variables are visible avryware (uncalucaled will have null value), 
        //          Duplicated globals from different stilesheets are replaced (by import presidence)
        // Locals:  Visible only in scope and after it was defined.
        //          No duplicates posible.
        private ArrayList _globalVars = new ArrayList();
        private ArrayList _localVars = new ArrayList();
        private VariableAction[] _globalVarsCache, _localVarsCache;

        public virtual VariableAction[] GlobalVariables
        {
            get
            {
                Debug.Assert(this.Debugger != null);
                if (_globalVarsCache == null)
                {
                    _globalVarsCache = (VariableAction[])_globalVars.ToArray(typeof(VariableAction));
                }
                return _globalVarsCache;
            }
        }
        public virtual VariableAction[] LocalVariables
        {
            get
            {
                Debug.Assert(this.Debugger != null);
                if (_localVarsCache == null)
                {
                    _localVarsCache = (VariableAction[])_localVars.ToArray(typeof(VariableAction));
                }
                return _localVarsCache;
            }
        }

        private void DefineVariable(VariableAction variable)
        {
            Debug.Assert(this.Debugger != null);
            if (variable.IsGlobal)
            {
                for (int i = 0; i < _globalVars.Count; i++)
                {
                    VariableAction oldVar = (VariableAction)_globalVars[i];
                    if (oldVar.Name == variable.Name)
                    { // Duplicate var definition
                        if (variable.Stylesheetid < oldVar.Stylesheetid)
                        {
                            Debug.Assert(variable.VarKey != -1, "Variable was already placed and it should replace prev var.");
                            _globalVars[i] = variable;
                            _globalVarsCache = null;
                        }
                        return;
                    }
                }
                _globalVars.Add(variable);
                _globalVarsCache = null;
            }
            else
            {
                // local variables never conflict
                _localVars.Add(variable);
                _localVarsCache = null;
            }
        }

        private void UnDefineVariables(int count)
        {
            Debug.Assert(0 <= count, "This scope can't have more variables than we have in total");
            Debug.Assert(count <= _localVars.Count, "This scope can't have more variables than we have in total");
            if (count != 0)
            {
                _localVars.RemoveRange(_localVars.Count - count, count);
                _localVarsCache = null;
            }
        }

        internal override void PopScope()
        {
            this.UnDefineVariables(this.ScopeManager.CurrentScope.GetVeriablesCount());
            base.PopScope();
        }

        // ---------------- Actions: ---------------

        public override ApplyImportsAction CreateApplyImportsAction()
        {
            ApplyImportsAction action = new ApplyImportsActionDbg();
            action.Compile(this);
            return action;
        }

        public override ApplyTemplatesAction CreateApplyTemplatesAction()
        {
            ApplyTemplatesAction action = new ApplyTemplatesActionDbg();
            action.Compile(this);
            return action;
        }

        public override AttributeAction CreateAttributeAction()
        {
            AttributeAction action = new AttributeActionDbg();
            action.Compile(this);
            return action;
        }

        public override AttributeSetAction CreateAttributeSetAction()
        {
            AttributeSetAction action = new AttributeSetActionDbg();
            action.Compile(this);
            return action;
        }

        public override CallTemplateAction CreateCallTemplateAction()
        {
            CallTemplateAction action = new CallTemplateActionDbg();
            action.Compile(this);
            return action;
        }

        public override ChooseAction CreateChooseAction()
        {//!!! don't need to be here
            ChooseAction action = new ChooseAction();
            action.Compile(this);
            return action;
        }

        public override CommentAction CreateCommentAction()
        {
            CommentAction action = new CommentActionDbg();
            action.Compile(this);
            return action;
        }

        public override CopyAction CreateCopyAction()
        {
            CopyAction action = new CopyActionDbg();
            action.Compile(this);
            return action;
        }

        public override CopyOfAction CreateCopyOfAction()
        {
            CopyOfAction action = new CopyOfActionDbg();
            action.Compile(this);
            return action;
        }

        public override ElementAction CreateElementAction()
        {
            ElementAction action = new ElementActionDbg();
            action.Compile(this);
            return action;
        }

        public override ForEachAction CreateForEachAction()
        {
            ForEachAction action = new ForEachActionDbg();
            action.Compile(this);
            return action;
        }

        public override IfAction CreateIfAction(IfAction.ConditionType type)
        {
            IfAction action = new IfActionDbg(type);
            action.Compile(this);
            return action;
        }

        public override MessageAction CreateMessageAction()
        {
            MessageAction action = new MessageActionDbg();
            action.Compile(this);
            return action;
        }

        public override NewInstructionAction CreateNewInstructionAction()
        {
            NewInstructionAction action = new NewInstructionActionDbg();
            action.Compile(this);
            return action;
        }

        public override NumberAction CreateNumberAction()
        {
            NumberAction action = new NumberActionDbg();
            action.Compile(this);
            return action;
        }

        public override ProcessingInstructionAction CreateProcessingInstructionAction()
        {
            ProcessingInstructionAction action = new ProcessingInstructionActionDbg();
            action.Compile(this);
            return action;
        }

        public override void CreateRootAction()
        {
            this.RootAction = new RootActionDbg();
            this.RootAction.Compile(this);
        }

        public override SortAction CreateSortAction()
        {
            SortAction action = new SortActionDbg();
            action.Compile(this);
            return action;
        }

        public override TemplateAction CreateTemplateAction()
        {
            TemplateAction action = new TemplateActionDbg();
            action.Compile(this);
            return action;
        }

        public override TemplateAction CreateSingleTemplateAction()
        {
            TemplateAction action = new TemplateActionDbg();
            action.CompileSingle(this);
            return action;
        }

        public override TextAction CreateTextAction()
        {
            TextAction action = new TextActionDbg();
            action.Compile(this);
            return action;
        }

        public override UseAttributeSetsAction CreateUseAttributeSetsAction()
        {
            UseAttributeSetsAction action = new UseAttributeSetsActionDbg();
            action.Compile(this);
            return action;
        }

        public override ValueOfAction CreateValueOfAction()
        {
            ValueOfAction action = new ValueOfActionDbg();
            action.Compile(this);
            return action;
        }

        public override VariableAction CreateVariableAction(VariableType type)
        {
            VariableAction action = new VariableActionDbg(type);
            action.Compile(this);
            return action;
        }

        public override WithParamAction CreateWithParamAction()
        {
            WithParamAction action = new WithParamActionDbg();
            action.Compile(this);
            return action;
        }

        // ---------------- Events: ---------------

        public override BeginEvent CreateBeginEvent()
        {
            return new BeginEventDbg(this);
        }

        public override TextEvent CreateTextEvent()
        {
            return new TextEventDbg(this);
        }

        // Debugger enabled implemetation of most compiled actions

        private class ApplyImportsActionDbg : ApplyImportsAction
        {
            private DbgData _dbgData;
            internal override DbgData GetDbgData(ActionFrame frame) { return _dbgData; }

            internal override void Compile(Compiler compiler)
            {
                _dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == Initialized)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }
        }

        private class ApplyTemplatesActionDbg : ApplyTemplatesAction
        {
            private DbgData _dbgData;
            internal override DbgData GetDbgData(ActionFrame frame) { return _dbgData; }

            internal override void Compile(Compiler compiler)
            {
                _dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == Initialized)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }
        }

        private class AttributeActionDbg : AttributeAction
        {
            private DbgData _dbgData;
            internal override DbgData GetDbgData(ActionFrame frame) { return _dbgData; }

            internal override void Compile(Compiler compiler)
            {
                _dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == Initialized)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }
        }

        private class AttributeSetActionDbg : AttributeSetAction
        {
            private DbgData _dbgData;
            internal override DbgData GetDbgData(ActionFrame frame) { return _dbgData; }

            internal override void Compile(Compiler compiler)
            {
                _dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == Initialized)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }
        }

        private class CallTemplateActionDbg : CallTemplateAction
        {
            private DbgData _dbgData;
            internal override DbgData GetDbgData(ActionFrame frame) { return _dbgData; }

            internal override void Compile(Compiler compiler)
            {
                _dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == Initialized)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }
        }

        private class CommentActionDbg : CommentAction
        {
            private DbgData _dbgData;
            internal override DbgData GetDbgData(ActionFrame frame) { return _dbgData; }

            internal override void Compile(Compiler compiler)
            {
                _dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == Initialized)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }
        }

        private class CopyActionDbg : CopyAction
        {
            private DbgData _dbgData;
            internal override DbgData GetDbgData(ActionFrame frame) { return _dbgData; }

            internal override void Compile(Compiler compiler)
            {
                _dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == Initialized)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }
        }

        private class CopyOfActionDbg : CopyOfAction
        {
            private DbgData _dbgData;
            internal override DbgData GetDbgData(ActionFrame frame) { return _dbgData; }

            internal override void Compile(Compiler compiler)
            {
                _dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == Initialized)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }
        }

        private class ElementActionDbg : ElementAction
        {
            private DbgData _dbgData;
            internal override DbgData GetDbgData(ActionFrame frame) { return _dbgData; }

            internal override void Compile(Compiler compiler)
            {
                _dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == Initialized)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }
        }

        private class ForEachActionDbg : ForEachAction
        {
            private DbgData _dbgData;
            internal override DbgData GetDbgData(ActionFrame frame) { return _dbgData; }

            internal override void Compile(Compiler compiler)
            {
                _dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == Initialized)
                {
                    processor.PushDebuggerStack();
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
                if (frame.State == Finished)
                {
                    processor.PopDebuggerStack();
                }
            }
        }

        private class IfActionDbg : IfAction
        {
            internal IfActionDbg(ConditionType type) : base(type) { }

            private DbgData _dbgData;
            internal override DbgData GetDbgData(ActionFrame frame) { return _dbgData; }

            internal override void Compile(Compiler compiler)
            {
                _dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == Initialized)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }
        }

        private class MessageActionDbg : MessageAction
        {
            private DbgData _dbgData;
            internal override DbgData GetDbgData(ActionFrame frame) { return _dbgData; }

            internal override void Compile(Compiler compiler)
            {
                _dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == Initialized)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }
        }

        private class NewInstructionActionDbg : NewInstructionAction
        {
            private DbgData _dbgData;
            internal override DbgData GetDbgData(ActionFrame frame) { return _dbgData; }

            internal override void Compile(Compiler compiler)
            {
                _dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == Initialized)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }
        }

        private class NumberActionDbg : NumberAction
        {
            private DbgData _dbgData;
            internal override DbgData GetDbgData(ActionFrame frame) { return _dbgData; }

            internal override void Compile(Compiler compiler)
            {
                _dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == Initialized)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }
        }

        private class ProcessingInstructionActionDbg : ProcessingInstructionAction
        {
            private DbgData _dbgData;
            internal override DbgData GetDbgData(ActionFrame frame) { return _dbgData; }

            internal override void Compile(Compiler compiler)
            {
                _dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == Initialized)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }
        }

        private class RootActionDbg : RootAction
        {
            private DbgData _dbgData;
            internal override DbgData GetDbgData(ActionFrame frame) { return _dbgData; }

            // SxS: This method does not take any resource name and does not expose any resources to the caller.
            // It's OK to suppress the SxS warning.
            internal override void Compile(Compiler compiler)
            {
                _dbgData = new DbgData(compiler);
                base.Compile(compiler);

                Debug.Assert(compiler.Debugger != null);
                string builtIn = compiler.Debugger.GetBuiltInTemplatesUri();
                if (builtIn != null && builtIn.Length != 0)
                {
                    compiler.AllowBuiltInMode = true;
                    builtInSheet = compiler.RootAction.CompileImport(compiler, compiler.ResolveUri(builtIn), int.MaxValue);
                    compiler.AllowBuiltInMode = false;
                }

                _dbgData.ReplaceVariables(((DbgCompiler)compiler).GlobalVariables);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == Initialized)
                {
                    processor.PushDebuggerStack();
                    processor.OnInstructionExecute();
                    processor.PushDebuggerStack();
                }
                base.Execute(processor, frame);
                if (frame.State == Finished)
                {
                    processor.PopDebuggerStack();
                    processor.PopDebuggerStack();
                }
            }
        }

        private class SortActionDbg : SortAction
        {
            private DbgData _dbgData;
            internal override DbgData GetDbgData(ActionFrame frame) { return _dbgData; }

            internal override void Compile(Compiler compiler)
            {
                _dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == Initialized)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }
        }

        private class TemplateActionDbg : TemplateAction
        {
            private DbgData _dbgData;
            internal override DbgData GetDbgData(ActionFrame frame) { return _dbgData; }

            internal override void Compile(Compiler compiler)
            {
                _dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == Initialized)
                {
                    processor.PushDebuggerStack();
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
                if (frame.State == Finished)
                {
                    processor.PopDebuggerStack();
                }
            }
        }

        private class TextActionDbg : TextAction
        {
            private DbgData _dbgData;
            internal override DbgData GetDbgData(ActionFrame frame) { return _dbgData; }

            internal override void Compile(Compiler compiler)
            {
                _dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == Initialized)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }
        }

        private class UseAttributeSetsActionDbg : UseAttributeSetsAction
        {
            private DbgData _dbgData;
            internal override DbgData GetDbgData(ActionFrame frame) { return _dbgData; }

            internal override void Compile(Compiler compiler)
            {
                _dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == Initialized)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }
        }

        private class ValueOfActionDbg : ValueOfAction
        {
            private DbgData _dbgData;
            internal override DbgData GetDbgData(ActionFrame frame) { return _dbgData; }

            internal override void Compile(Compiler compiler)
            {
                _dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == Initialized)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }
        }

        private class VariableActionDbg : VariableAction
        {
            internal VariableActionDbg(VariableType type) : base(type) { }
            private DbgData _dbgData;
            internal override DbgData GetDbgData(ActionFrame frame) { return _dbgData; }

            internal override void Compile(Compiler compiler)
            {
                _dbgData = new DbgData(compiler);
                base.Compile(compiler);
                ((DbgCompiler)compiler).DefineVariable(this);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == Initialized)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }
        }

        private class WithParamActionDbg : WithParamAction
        {
            private DbgData _dbgData;
            internal override DbgData GetDbgData(ActionFrame frame) { return _dbgData; }

            internal override void Compile(Compiler compiler)
            {
                _dbgData = new DbgData(compiler);
                base.Compile(compiler);
            }

            internal override void Execute(Processor processor, ActionFrame frame)
            {
                if (frame.State == Initialized)
                {
                    processor.OnInstructionExecute();
                }
                base.Execute(processor, frame);
            }
        }

        // ---------------- Events: ---------------

        private class BeginEventDbg : BeginEvent
        {
            private DbgData _dbgData;
            internal override DbgData DbgData { get { return _dbgData; } }

            public BeginEventDbg(Compiler compiler) : base(compiler)
            {
                _dbgData = new DbgData(compiler);
            }
            public override bool Output(Processor processor, ActionFrame frame)
            {
                this.OnInstructionExecute(processor);
                return base.Output(processor, frame);
            }
        }

        private class TextEventDbg : TextEvent
        {
            private DbgData _dbgData;
            internal override DbgData DbgData { get { return _dbgData; } }

            public TextEventDbg(Compiler compiler) : base(compiler)
            {
                _dbgData = new DbgData(compiler);
            }
            public override bool Output(Processor processor, ActionFrame frame)
            {
                this.OnInstructionExecute(processor);
                return base.Output(processor, frame);
            }
        }
    }
}
