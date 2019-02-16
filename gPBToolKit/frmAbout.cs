/*
 *  "gk.PBToolKit", a set of utilities for processing mimics OSISoft PI Processbook, 
 *  implemented as an add-in.
 *
 *  Copyright (C) 2015-2019  Igor Tyulyakov aka g10101k, g101k. Contacts: <g101k@mail.ru>
 *  
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *       http://www.apache.org/licenses/LICENSE-2.0
 *
 *   Unless required by applicable law or agreed to in writing, software
 *   distributed under the License is distributed on an "AS IS" BASIS,
 *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *   See the License for the specific language governing permissions and
 *   limitations under the License.
 */
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

namespace gPBToolKit
{
    public partial class frmAbout : Form
    {

        private TextEditorControl editor = new TextEditorControl();
        private List<mLayer> mLayers = new List<mLayer>();

        private PBObjLib.Application m_App = null;

        public frmAbout(PBObjLib.Application app)
        {
            m_App = app;
            InitializeComponent();
            string tmp = Path.GetTempPath() + @"\mPBScript.cs";
            if (!File.Exists(tmp))
                File.Create(tmp).Close();

            editor.LoadFile(tmp);
            editor.Dock = DockStyle.Fill;
           // this.groupBox2.Controls.Add(editor);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Display ThisDisplay = m_App.ActiveDisplay;
            ThisDisplay.BackgroundColor = 14869218;
            for (int i = 1; i <= ThisDisplay.Symbols.Count; i++)
            {
                Symbol s = ThisDisplay.Symbols.Item(i);
                if (s.Type == 7)
                {
                    ((Value)s).BackgroundColor = 32768;
                    ((Value)s).LineColor = 16777215;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Display ThisDisplay = m_App.ActiveDisplay;

            for (int i = 1; i <= ThisDisplay.SelectedSymbols.Count; i++)
            {
                Symbol s = ThisDisplay.SelectedSymbols.Item(i);
                if (s.IsMultiState)
                {
                    MultiState ms = s.GetMultiState();
                    ms.GetState(1).Color = 32768;
                    ms.ColorBadData = 65535;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Display ThisDisplay = m_App.ActiveDisplay;
            for (int j = ThisDisplay.Layers.Count; j >= 1; j--)
            {

                for (int i = ThisDisplay.Layers.Item(j).LayerSymbols.Count; i >= 1 ; i--)
                {
                    try
                    {
                        Symbol s = ThisDisplay.Layers.Item(j).LayerSymbols.Item(i);
                        ThisDisplay.Layers.Item(j).LayerSymbols.Remove(s.Name);
                        s.Selected = true;
                        //m_App.Application.DockWindows.Item(1).Application..Send("%f");
                       
                    }
                    catch { }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Display ThisDisplay = m_App.ActiveDisplay;
            Symbol s = ThisDisplay.SelectedSymbols.Item(1);
             for (int i = 1; i <= ThisDisplay.Symbols.Count; i++){
                 Symbol s2 = ThisDisplay.Symbols.Item(i);
                 if (s2.Name == s.Name) {MessageBox.Show(i.ToString());}
             }
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            mLayers.Clear();
            Display ThisDisplay = m_App.Displays.Item(1);
            if (ThisDisplay.Layers.Count == 0)
            {
                //MessageBox.Show("layers not found", "successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (Layer l in ThisDisplay.Layers)
            {
                l.Active = false;
            }

            foreach (Symbol s in ThisDisplay.SelectedSymbols)
            {
                s.Selected = false;
            }

            foreach (Symbol s in ThisDisplay.Symbols)
            {
                if (s.Name.IndexOf("AVE") >= 0) { 
                    s.Selected = true;
                    ThisDisplay.Delete();
                    foreach (Symbol item in ThisDisplay.SelectedSymbols)
                        item.Selected = false;
                }
            }

            foreach (Layer l in ThisDisplay.Layers)
            {
                mLayer mL = new mLayer();
                mL.Name = l.Name;
                try
                {
                    foreach (Symbol s in l.LayerSymbols)
                    {
                        mL.Add(s.Name);
                    }
                }
                catch { }
                mLayers.Add(mL);
            }

            foreach (mLayer l in mLayers)
            {
                foreach (string sName in l)
                {                    
                    try
                    {
                        Symbol s = ThisDisplay.Symbols.Item(sName);
                        ThisDisplay.Layers.Item(l.Name).LayerSymbols.Remove(s.Name);
                        s.Selected = true;
                        short sT = 0, sL = 0;

                        sT = s.Top;
                        sL = (s.Left < -15000) ? (short)-15000 : s.Left;

                        ThisDisplay.Cut();
                        ThisDisplay.Paste();

                        foreach (Symbol item in ThisDisplay.SelectedSymbols)
                        {
                            item.Top = sT;
                            item.Left = sL;
                            item.Selected = false;
                        }
                    }
                    catch
                    {
                        foreach (Symbol item in ThisDisplay.SelectedSymbols)
                        {
                            item.Selected = false;
                        }
                    }
                }
            }

            foreach (mLayer l in mLayers)
                ThisDisplay.Layers.Remove(l.Name);

            //MessageBox.Show("execute successful", "successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button6_Click(object sender, EventArgs e)
        {
        }

        private void PropertyForm_Load(object sender, EventArgs e)
        {
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Source code ��� ����������
            string source = editor.Document.TextContent;
            // ��������� ����������
            Dictionary<string, string> providerOptions = new Dictionary<string, string>();
            providerOptions.Add("CompilerVersion", "v2.0");
            CSharpCodeProvider provider = new CSharpCodeProvider(providerOptions);

            CompilerParameters compilerParams = new CompilerParameters();
            //compilerParams.OutputAssembly = "";
            compilerParams.GenerateInMemory = true;
            compilerParams.GenerateExecutable = false;
            compilerParams.ReferencedAssemblies.Add("System.dll");
            compilerParams.ReferencedAssemblies.Add("System.Drawing.dll");
            compilerParams.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            compilerParams.ReferencedAssemblies.Add(@"C:\Program Files (x86)\PIPC\Procbook\OSIsoft.PBObjLib.dll");
            compilerParams.ReferencedAssemblies.Add(@"C:\Program Files (x86)\PIPC\Procbook\OSIsoft.PBSymLib.dll");

            // ����������
            CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, source);

            // ������� ���������� �� �������
            //System.Windows.Forms.MessageBox.Show(string.Format("Number of Errors: {0}", results.Errors.Count));
            foreach (CompilerError err in results.Errors)
            {
                MessageBox.Show(string.Format("Error {0}", err.ErrorText));
            }
            if (results.Errors.Count == 0)
            {
                Assembly assembly = results.CompiledAssembly;
                Type type = assembly.GetType("gPBToolKit.gkScript");
                MethodInfo method = type.GetMethod("StartScript");
                method.Invoke(null, new object[] { m_App.ActiveDisplay });
            }
        }

        private void PropertyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            editor.SaveFile(editor.FileName);
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog dlg = new FolderBrowserDialog();
                dlg.ShowDialog();
                List<string> listOfPath = new List<string>();

                DirSearch(dlg.SelectedPath, listOfPath);
                foreach (Display d in m_App.Displays)
                {
                    d.Close(false);
                }
                foreach (string path in listOfPath)
                {
                    m_App.Displays.Open(path, null);
                    m_App.RunMode = false;
                    button5_Click(null, null);
                    m_App.RunMode = false;
                    m_App.Displays.Item(1).Close(true);
                }

                MessageBox.Show("execute successful", "successful", MessageBoxButtons.OK, MessageBoxIcon.Information);  
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void DirSearch(string sDir, List<string> list)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetFiles(d, "*.pdi"))
                    {
                        list.Add(f);                    
                    }
                    DirSearch(d, list);
                }
                foreach (string f in Directory.GetFiles(sDir, "*.pdi"))
                {
                    list.Add(f);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

    }
}