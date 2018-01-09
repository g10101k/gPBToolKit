using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PBObjLib;
using PBSymLib;
using VBIDE;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using System.Threading;

using NRefactory = ICSharpCode.NRefactory;
using Dom = ICSharpCode.SharpDevelop.Dom;

namespace gPBToolKit
{
    public partial class ScriptForm : Form
    {

        private PBObjLib.Application m_App = null;

        internal Dom.ProjectContentRegistry pcRegistry;
        internal Dom.DefaultProjectContent myProjectContent;
        internal Dom.ICompilationUnit lastCompilationUnit;
        Thread parserThread;

        public string workDir = "";
        public string fullPathDummyFile = "";
        public const string DummyFileName = @"mPBScript.cs";

        public ScriptForm(PBObjLib.Application app)
        {
            m_App = app;
            InitializeComponent();
            editor.ShowEOLMarkers = false;
            editor.ShowSpaces = false;
            editor.ShowTabs = false;
            editor.ShowInvalidLines = false;
            editor.SetHighlighting("C#");
            workDir = Path.GetTempPath() + "gPBToolKit\\";
            fullPathDummyFile = workDir + DummyFileName;

            if (!Directory.Exists(workDir))
                Directory.CreateDirectory(workDir);
            if (!Directory.Exists(workDir + "\\CSharpCodeCompletion\\" ))
                Directory.CreateDirectory(workDir + "\\CSharpCodeCompletion");

            

            string indexFile = workDir + "CSharpCodeCompletion\\index.dat";
            if (!File.Exists(fullPathDummyFile))
            {

                string sh = @"using System;
using System.Collections.Generic;
using System.Text;
using PBObjLib;
using PBSymLib;
using VBIDE;
using System.Windows.Forms;

namespace gPBToolKit
{
    public class gkScript
    {   
    	// Передается активный дисплей
    	public static void StartScript(Display d)
    	{
    		foreach (Symbol s in d.Symbols)
    		{
    			if (s.Type == 7)
    			{
    				Value v = (Value)s;
    				v.BackgroundColor = 32768;  
    				v.ShowUOM = true;
    			}    		
    		}
    		Action(d.Application);
    	}
    	
    	public static void Action(PBObjLib.Application m_App)
    	{
    		try
            {
                VBProject project = null;
        		CodeModule codeModule = null;
                for (int i = 1; i <= ((VBE)m_App.VBE).VBProjects.Count; i++)
                {
                    VBProject bufProject = ((VBE)m_App.VBE).VBProjects.Item(i);
                    if (bufProject.FileName.ToLower() == m_App.ActiveDisplay.Path.ToLower()) // Если не сохранить будет ошибка
                    {
                        project = bufProject;
                        break;
                    }
                }

                if (project == null)
                {
                    MessageBox.Show(""VBProject not found"");
                    return;
                }

                codeModule = project.VBComponents.Item(""ThisDisplay"").CodeModule;
                try
                {
                    string procName = ""Display_Open"";
                    int procCountLine = -1, procStart = -1;
                    try{
						procStart = codeModule.get_ProcBodyLine(procName, vbext_ProcKind.vbext_pk_Proc);
                		procCountLine = codeModule.get_ProcCountLines(procName, vbext_ProcKind.vbext_pk_Proc);	
                	}
                	catch{}
                	
                	if (procStart > 0) 
                		codeModule.DeleteLines(procStart, procCountLine);
                	
                	string srvName = ""do51-dc1-du-pis.sgpz.gpp.gazprom.ru"";
                	string rootModule = ""СГПЗ"";                	
                	string dispOpenText = string.Format(@""
Private Sub Display_Open()
    AVExtension1.Initialize """"{0}"""", """"{1}"""", ThisDisplay, Trend1
End Sub"", srvName, rootModule);
                	codeModule.AddFromString(dispOpenText);			
                }
                catch (Exception ex) {
                   	MessageBox.Show(ex.Message + "" ""+ ex.StackTrace);
                }
            }
            catch { }
    	}
    }
}
";
                UTF8Encoding asciEncoding = new UTF8Encoding();
                FileStream fs = File.Create(fullPathDummyFile);
                fs.Write(asciEncoding.GetBytes(sh), 0, asciEncoding.GetByteCount(sh));
                fs.Close();
            }
            if (!File.Exists(indexFile))
                File.Create(indexFile).Close();


            editor.LoadFile(fullPathDummyFile);

            refList.Items.Add("System.dll");
            refList.Items.Add("System.Drawing.dll");
            refList.Items.Add("System.Windows.Forms.dll");
            refList.Items.Add(@"C:\Program Files (x86)\PIPC\Procbook\OSIsoft.PBObjLib.dll");
            refList.Items.Add(@"C:\Program Files (x86)\PIPC\Procbook\OSIsoft.PBSymLib.dll");
            refList.Items.Add(@"C:\Program Files (x86)\PIPC\Procbook\Interop.VBIDE.dll");

            CodeCompletionKeyHandler.Attach(this, editor);
            HostCallbackImplementation.Register(this);

            pcRegistry = new Dom.ProjectContentRegistry(); // Default .NET 2.0 registry

            // Persistence caches referenced project contents for faster loading.
            // It also activates loading XML documentation files and caching them
            // for faster loading and lower memory usage.
            pcRegistry.ActivatePersistence(Path.Combine(workDir, "CSharpCodeCompletion"));

            myProjectContent = new Dom.DefaultProjectContent();
            myProjectContent.Language = Dom.LanguageProperties.CSharp;
        }

        private void PropertyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            editor.SaveFile(editor.FileName);
            if (parserThread.ThreadState == ThreadState.Running)
                parserThread.Suspend();
        }

        public void toolStripButton1_Click(object sender, EventArgs e)
        {
            errorList.Items.Clear();
            // Source code для компиляции
            string source = editor.Document.TextContent;
            // Настройки компиляции
            Dictionary<string, string> providerOptions = new Dictionary<string, string>();
            providerOptions.Add("CompilerVersion", "v2.0");
            CSharpCodeProvider provider = new CSharpCodeProvider(providerOptions);

            CompilerParameters compilerParams = new CompilerParameters();
            //compilerParams.OutputAssembly = "";
            compilerParams.GenerateInMemory = true;
            compilerParams.GenerateExecutable = false;
            foreach (object o in refList.Items)
                compilerParams.ReferencedAssemblies.Add(o.ToString());


            // Компиляция
            CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, source);

            // Выводим информацию об ошибках
            //System.Windows.Forms.MessageBox.Show(string.Format("Number of Errors: {0}", results.Errors.Count));
            foreach (CompilerError err in results.Errors)
            {
                ListViewItem item = errorList.Items.Add(err.ErrorNumber.ToString());
                item.SubItems.Add(err.ErrorText);
                item.SubItems.Add(err.FileName);
                item.SubItems.Add(err.Line.ToString());
                item.SubItems.Add(err.Column.ToString());
                item.SubItems.Add(err.IsWarning.ToString());
            }
            if (results.Errors.Count == 0)
            {
                Assembly assembly = results.CompiledAssembly;
                Type type = assembly.GetType("gPBToolKit.gkScript");
                MethodInfo method = type.GetMethod("StartScript");
                method.Invoke(null, new object[] { m_App.ActiveDisplay });
                ListViewItem item = errorList.Items.Add("");
                item.SubItems.Add("Выполнено успешно!!!");
            }
        }

        private void ScriptForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (int)e.KeyChar
        }
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool bHandled = false;
            // switch case is the easy way, a hash or map would be better, 
            // but more work to get set up.
            switch (keyData)
            {
                case Keys.F5:
                    // do whatever
                    toolStripButton1_Click(null, null);
                    bHandled = true;
                    break;
            }
            return bHandled;
        }

        void ParserThread()
        {
            try
            {
                //return;
                BeginInvoke(new MethodInvoker(delegate { parserThreadLabel.Text = "Loading mscorlib..."; }));
                myProjectContent.AddReferencedContent(pcRegistry.Mscorlib);

                // do one initial parser step to enable code-completion while other
                // references are loading
                ParseStep();


                foreach (string assemblyName in refList.Items)
                {
                    string assemblyNameCopy = assemblyName.Replace(".dll", "");
                    { // block for anonymous method

                        BeginInvoke(new MethodInvoker(delegate { parserThreadLabel.Text = "Loading " + assemblyNameCopy + "..."; }));
                    }
                    myProjectContent.AddReferencedContent(pcRegistry.GetProjectContentForReference(assemblyNameCopy, assemblyName));
                }
                BeginInvoke(new MethodInvoker(delegate { parserThreadLabel.Text = "Ready"; }));

                // Parse the current file every 2 seconds
                while (!IsDisposed)
                {
                    ParseStep();

                    Thread.Sleep(2000);
                }
            }
            catch {  }
        }

        void ParseStep()
        {
            try
            {
                string code = null;
                Invoke(new MethodInvoker(delegate
                {
                    code = editor.Text;
                }));
                TextReader textReader = new StringReader(code);
                Dom.ICompilationUnit newCompilationUnit;
                using (NRefactory.IParser p = NRefactory.ParserFactory.CreateParser(NRefactory.SupportedLanguage.CSharp, textReader))
                {
                    p.Parse();
                    newCompilationUnit = ConvertCompilationUnit(p.CompilationUnit);
                }
                // Remove information from lastCompilationUnit and add information from newCompilationUnit.
                myProjectContent.UpdateCompilationUnit(lastCompilationUnit, newCompilationUnit, DummyFileName);
                lastCompilationUnit = newCompilationUnit;
            }
            catch { }
        }

        Dom.ICompilationUnit ConvertCompilationUnit(NRefactory.Ast.CompilationUnit cu)
        {
            Dom.NRefactoryResolver.NRefactoryASTConvertVisitor converter;
            converter = new Dom.NRefactoryResolver.NRefactoryASTConvertVisitor(myProjectContent);
            cu.AcceptVisitor(converter, null);
            return converter.Cu;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            openRef.ShowDialog();
            if (File.Exists(openRef.FileName))
                refList.Items.Add(openRef.FileName);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            refList.Items.Remove(refList.SelectedItem);
        }

        private void ScriptForm_Load(object sender, EventArgs e)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            parserThread = new Thread(ParserThread);
            parserThread.IsBackground = true;
            parserThread.Start();
        }

        private void errorList_DoubleClick_1(object sender, EventArgs e)
        {
            editor.ActiveTextAreaControl.Caret.Line = 
                Convert.ToInt32(errorList.Items[errorList.SelectedIndices[0]].SubItems[3].Text) - 1;
            editor.ActiveTextAreaControl.Caret.Column = 
                Convert.ToInt32(errorList.Items[errorList.SelectedIndices[0]].SubItems[4].Text) - 1;
            editor.Focus();
        }
    }
}