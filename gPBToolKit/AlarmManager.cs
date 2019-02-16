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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using PBObjLib;
using PBSymLib;
using VBIDE;
using PISDK;
using PISDKDlg;

//TODO: ������ ���� � ������
//TODO: ������� ������� ��� ��������
//TODO: ����� ����������  �������� ���� � ��������


namespace gPBToolKit
{
    public partial class AlarmManager : Form
    {
        private PBObjLib.Application m_App = null;
        private List<AVEAlarmObject> ListOfAVESymbol = new List<AVEAlarmObject>();
        private PIProperty AlarmsNode = null;
        private PIProperty editedAlarm = null;
        private VBProject project = null;
        private CodeModule codeModule = null;
        private Symbol currentSymbol = null;
        private Symbol AVE = null;
        private string ServerName = "";
        private string ModuleRoot = "";
        private Connect connect = null;

        public AlarmManager(PBObjLib.Application Application, Connect con)
        {
            m_App = Application;
            connect = con;
            InitializeComponent();
            if (connect.treeNode != null)
            {
                try
                {
                    //treeView1.Nodes.Clear();
                    connect.treeNode.TreeView.Nodes.Clear();
                    //treeView1.Nodes.Add(connect.treeNode);
                    
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
                //treeView1.Nodes.Add(connect.treeNode);
        }

        ///<summary>
        /// ���������� ������� ���������� � �������� ������
        ///</summary> 
        private PBObjLib.Symbol getSymbolByName(string Name)
        {
            for (int i = 1; i <= m_App.ActiveDisplay.Symbols.Count; i++)
                if (m_App.ActiveDisplay.Symbols.Item(i).Name == Name)
                    return (m_App.ActiveDisplay.Symbols.Item(i));
            return null;
        }

        ///<summary>
        /// ����������� �������������, �������� ����� �� ����������, ����� ��������.
        ///</summary> 
        private void getAVEAlarmObject()
        {
            try
            {
                for (int i = 1; i <= ((VBE)m_App.VBE).VBProjects.Count; i++)
                {
                    VBProject bufProject = ((VBE)m_App.VBE).VBProjects.Item(i);
                    if (bufProject.FileName.ToLower() == m_App.ActiveDisplay.Path.ToLower()) // ���� �� ��������� ����� ������
                    {
                        project = bufProject;
                        break;
                    }
                }

                if (project == null)
                {
                    MessageBox.Show("VBProject not found");
                    return;
                }
                List<string> listOfCase = new List<string>();
                string procQueryText = "";
                string procDisplayText = "";

                codeModule = project.VBComponents.Item("ThisDisplay").CodeModule;
                //TODO: ��� ������� ����� ����� �� "�������� �������", ���� ������� AVExtension1_QueryObjectInfo. �� �������� ��������!
                try
                {                    
                    string procName = "AVExtension1_QueryObjectInfo";
                    int procStart = codeModule.get_ProcBodyLine(procName, vbext_ProcKind.vbext_pk_Proc);
                    int procCountLine = codeModule.get_ProcCountLines(procName, vbext_ProcKind.vbext_pk_Proc);

                    procQueryText = codeModule.get_Lines(procStart, procCountLine);
                    if (procQueryText.IndexOf("Select Case") > 0)
                    {
                        int startSelect = procQueryText.IndexOf("Select Case") + ("End Select").Length + 1;
                        int EndSelect = procQueryText.IndexOf("End Select");
                        procQueryText = procQueryText.Substring(startSelect, EndSelect - startSelect);
                        string[] sfd = procQueryText.Split(new string[] { "Case " }, StringSplitOptions.None);
                        ListOfAVESymbol.Clear();
                        for (int i = 0; i < sfd.Length; i++)
                        {
                            if (sfd[i].IndexOf("objInfo") >= 0)
                            {
                                string objName = sfd[i].Split(new char[] { '"' }, StringSplitOptions.RemoveEmptyEntries)[0];
                                string mdbPath = sfd[i].Split(new char[] { '"' }, StringSplitOptions.RemoveEmptyEntries)[2];
                                Symbol symbol = getSymbolByName(objName);
                                
                                ListOfAVESymbol.Add(new AVEAlarmObject(objName, mdbPath, symbol));
                            }
                        }
                    }
                }
                catch{
                    string s = string.Format(Res2.DefaultQueryProc, "AVExtension1");
                    codeModule.AddFromString(s);
                    procQueryText = s;
                }

                try
                {
                    string procName = "Display_Open";
                    int procStart = codeModule.get_ProcBodyLine(procName, vbext_ProcKind.vbext_pk_Proc);
                    int procCountLine = codeModule.get_ProcCountLines(procName, vbext_ProcKind.vbext_pk_Proc);

                    procDisplayText = codeModule.get_Lines(procStart, procCountLine);

                    for (int j = procStart; j <= procCountLine; j++)
                        listOfCase.Add(codeModule.get_Lines(j, 1));
                    procDisplayText = procDisplayText.Substring(procDisplayText.IndexOf("Initialize") + ("Initialize").Length + 1);
                    procDisplayText = procDisplayText.Substring(0, procDisplayText.IndexOf('\n'));
                    string[] prop = procDisplayText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    ServerName = prop[0].Split(new char[] { '"' }, StringSplitOptions.RemoveEmptyEntries)[0];
                    ModuleRoot = prop[1].Split(new char[] { '"' }, StringSplitOptions.RemoveEmptyEntries)[0];
                }
                catch {
                    string s = string.Format(Res2.DefaultDisplyaOpen, "AVExtension1");
                    codeModule.AddFromString(s);
                    procDisplayText = s;

                }
            }
            catch { }
        }

        private void updateAlarms()
        {
            ServerManagerClass srvMgr = new ServerManagerClass();
            PIModuleDB ModuleDb = srvMgr.PISDK.Servers[ServerName].PIModuleDB;
            PIModule module = ModuleDb.PIModules[ModuleRoot];
            PIProperty property;
            string[] arrayModule = textBox1.Text.Split('\\');
            for (int i = 0; i < arrayModule.Length; i++)
            {
                module = module.PIModules[arrayModule[i]];
            }
            try {
                property = module.PIProperties["%meta"];
            }
            catch {
                property = module.PIProperties.Add("%meta", null);
                property.PIProperties.Add("Class", "������������� ������");
                property.PIProperties.Add("ModeVisualization", null);
                property.PIProperties.Add("Position", "Position");
                property.PIProperties.Add("ShowInAlarmList", "True");
                property.PIProperties.Add("StateSource", "Manual");
                property.PIProperties.Add("Tags", null);
                property.PIProperties.Add("��������", "��������");
            }
            try
            {
                AlarmsNode = property.PIProperties["Alarms"];
            }
            catch
            {
                AlarmsNode = property.PIProperties.Add("Alarms", null);
            }
            listBox1.Items.Clear();
            for (int i = 1; i <= AlarmsNode.PIProperties.Count; i++)
            {

                listBox1.Items.Add(new gPIProperty(AlarmsNode.PIProperties[i]));
            }       
        }
        
        private void AlarmManager_Load(object sender, EventArgs e)
        {
            if (m_App.ActiveDisplay.SelectedSymbols.Count > 0)
            {
                getAVEAlarmObject();

                currentSymbol = (Symbol)m_App.ActiveDisplay.SelectedSymbols.Item(1); // � �� ���t ��� �� ����� ���� ������ ��� ��� ����� ���� ������ �� ������ =)
                currentSymbol.EnableScript = true;
                AVE = getSymbolByName("AVExtension1");// TODO: ����� ����� AVE �� ������ 1

                if (AVE == null){
                    MessageBox.Show("��� AVExtension1");
                    this.Dispose(false);
                }

                if (getQueryObjectInfo(currentSymbol.Name) == null)
                    addQueryObjectInfo(currentSymbol.Name, "", 0);

                textBox1.Text = this.getQueryObjectInfo(currentSymbol.Name);

                if (textBox1.Text != "")
                    updateAlarms();
                button3_Click(null, null);
                //mdbTreeNet1.se
                
            }
            else
                this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            try
            {
                mdbTreeNet1.serverList[0] = ServerName;
                mdbTreeNet1.resetTree();
                mdbTreeNet1.setRootModules(new string[] {@"\\" + ServerName + @"\" + ModuleRoot});
                //mdbTreeNet1.sel

                if (false)
                {
                    ServerManagerClass srvMgr = new ServerManagerClass();

                    PIModuleDB ModuleDb = srvMgr.PISDK.Servers[ServerName].PIModuleDB;

                    //treeView1.Nodes.Clear();
                    PIModule module = ModuleDb.PIModules[ModuleRoot];
                    connect.treeNode = new TreeNode(module.Name);
                    //int i = treeView1.Nodes.Add(connect.treeNode);
                    TreeNode tNode = new TreeNode();
                    //tNode = treeView1.Nodes[i];
                    AddNode(module, tNode);
                }
            }
            catch { this.Enabled = true; }
            this.Enabled = true;
        }

        private static void AddNode(PIModule inNode, TreeNode inTreeNode)
        {
            PIModule xNode;
            TreeNode tNode;
            PIModules nodeList;
            int i;
            if (inNode.PIModules.Count > 0)
            {
                nodeList = inNode.PIModules;
                for (i = 0; i < nodeList.Count; i++)
                {
                    xNode = inNode.PIModules[i+1];
                    inTreeNode.Nodes.Add(new TreeNode(xNode.Name));
                    tNode = inTreeNode.Nodes[i];
                    AddNode(xNode, tNode);
                }
            }
            else
            {
                inTreeNode.Text = (inNode.Name).Trim();
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            PISDKDlg.TagSearch dialog = new PISDKDlg.TagSearch();
            PISDKCommon.NamedValuesClass n = new PISDKCommon.NamedValuesClass();
            
            PointList results = dialog.Show(n, TagSearchOptions.tsoptSingleSelect);

            if (results.Count > 0)
            {
                object index = 1;
                textBox3.Text = string.Format(@"{1}", results.get_Item(ref index).Server.Name, results.get_Item(ref index).Name);
                button2_Click(null, null);
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = ((OSIsoft.PISDK.Controls.IMDBNode)mdbTreeNet1.SelectedNodes.GetByIndex(0)).FullPathName.ToString().Replace(@"\\" + ServerName + @"\" + ModuleRoot + @"\", "");//treeView1.SelectedNode.FullPath.Replace(ModuleRoot+"\\", "");
            this.editQueryObjectInfo(currentSymbol.Name, textBox1.Text, 0);
            updateAlarms();
            if (listBox1.Items.Count > 0)
                listBox1.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                editedAlarm.Name = textBox2.Text;
                editedAlarm.PIProperties["TagName"].Value = textBox3.Text;
                editedAlarm.PIProperties["Enabled"].Value = Convert.ToBoolean(textBox4.Text);
                editedAlarm.PIProperties["AcknowledgmentUserGroup"].Value = textBox5.Text;
                editedAlarm.PIProperties["TraceInterval"].Value = Convert.ToInt32(textBox6.Text);
                editedAlarm.PIProperties["Type"].Value = textBox7.Text;
                //editedAlarm.PIProperties["Position"].Value = textBox8.Text;
                //MessageBox.Show("successful", "saved", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception ex){ 
                MessageBox.Show(string.Format("failed: {0}", ex.Message), "saved", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //return;
            if (MessageBox.Show("delete item from db?", "delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                AlarmsNode.PIProperties.Remove(((gPIProperty)listBox1.Items[listBox1.SelectedIndex]).self.Name);
                AlarmManager_Load(null, null);
            }
        }


        
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                PIProperty property = AlarmsNode.PIProperties.Add(string.Format("�������{0}", AlarmsNode.PIProperties.Count), null);
                property.PIProperties.Add("TagName", "");
                property.PIProperties.Add("Enabled", true);
                property.PIProperties.Add("AcknowledgmentUserGroup", null);
                property.PIProperties.Add("TraceInterval", "1000");
                property.PIProperties.Add("Type", "����������");
                //property.PIProperties.Add("Position", "�������");
                updateAlarms();
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                //AlarmManager_Load(null, null);
                //MessageBox.Show("successful", "saved", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception ex) {
                MessageBox.Show(string.Format("failed: {0}", ex.Message), "saved", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void editQueryObjectInfo(string objectName, string objInfo, int objType)
        {//TODO: �������� objType
            int countOfCase = 0; // ���� 1 ������ ����� ��� ���� 2 ������ ����� ������ ������ 

            for (int j = 1; j <= codeModule.CountOfLines; j++)
            {
                string bufString = codeModule.get_Lines(j, 1);
                if (bufString.IndexOf(string.Format("Case \"{0}\"", objectName)) > 0)
                    countOfCase++; // ����� ���
                if (countOfCase > 0)
                    if (bufString.IndexOf("objInfo") > 0)
                        countOfCase++; // ����� ������ � objInfo
                if (countOfCase > 1)
                {
                    // �������� ������
                    string[] array = bufString.Split(new char[] { '"' });
                    array[1] = objInfo;
                    codeModule.ReplaceLine(j, string.Join("\"", array));
                    return;
                }
            }   
        }

        private void addQueryObjectInfo(string objectName, string objInfo, int objType)
        {
            if (getQueryObjectInfo(objectName) == null)
            {
                string procName = string.Format("{0}_QueryObjectInfo", AVE.Name);
                int procStart = codeModule.get_ProcBodyLine(procName, vbext_ProcKind.vbext_pk_Proc);
                int procCountLine = codeModule.get_ProcCountLines(procName, vbext_ProcKind.vbext_pk_Proc);
                for (int endLoop = procStart + procCountLine; procStart <= endLoop; procStart++)
                {
                    if (codeModule.get_Lines(procStart, 1).IndexOf("Case Else") > 0)
                        break;
                }
                codeModule.InsertLines(procStart, string.Format(@"        Case ""{0}""
            objType = {1}
            objInfo = ""{2}""", objectName, objType, objInfo));
            }
        }

        private string getQueryObjectInfo(string objectName)
        {
            int countOfCase = 0; // ���� 1 ������ ����� ��� ���� 2 ������ ����� ������ ������ 

            for (int j = 1; j <= codeModule.CountOfLines; j++)
            {
                string bufString = codeModule.get_Lines(j, 1);
                if (bufString.IndexOf(string.Format("Case \"{0}\"", objectName)) > 0)
                    countOfCase++; // ����� ���
                if (countOfCase > 0)
                    if (bufString.IndexOf("objInfo") > 0)
                        countOfCase++; // ����� ������ � objInfo
                if (countOfCase > 1)
                    return bufString.Split(new char[] { '"' })[1];
            }
            return null;
        }

        private void textBox3_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 13)
                button2_Click(null, null);
            if ((int)e.KeyChar == 22)
            {
                // ������� ��� � ������) 
                IDataObject d = Clipboard.GetDataObject();

                if (d.GetDataPresent(DataFormats.Text))
                {
                    string s = (String)d.GetData(DataFormats.UnicodeText);
                    /*
                     //����� :(
                    ServerManagerClass srvMgr = new ServerManagerClass();
                    PI3PIPointClass point = srvMgr.PISDK.Servers[ServerName].GetPointsSQL(string.Format("PIpoint.SourceTag = '{0}'", s));
                     */
                    string[] b = s.Split(new char[] { ':' });
                    s = "";
                    for (int i = b.Length-1, count = 0; i >= 0; i--)
                    {
                        count++;
                        if (i == 0)
                            s = b[i] + s;
                        else
                            s = (count == 6) ? ".ALARM:" + b[i] + s : ":" + b[i] + s;                       
                    }
                    // ��������� ���� �� ���
                    ServerManagerClass srvMgr = new ServerManagerClass();
                    try
                    {
                        PIPoint point = srvMgr.PISDK.Servers[ServerName].PIPoints[s];
                        Clipboard.SetDataObject(s);
                        textBox3.Text = s;
                        button2_Click(null, null);
                        textBox3.Text = "";
                        
                    }
                    catch
                    {
                        MessageBox.Show(string.Format("tag not found '{0}'", s));
                    }
                }
                
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                editedAlarm = ((gPIProperty)listBox1.Items[listBox1.SelectedIndex]).self;
                textBox2.Text = editedAlarm.Name;
                textBox3.Text = editedAlarm.PIProperties["TagName"].Value.ToString();
                textBox4.Text = editedAlarm.PIProperties["Enabled"].Value.ToString();
                textBox5.Text = editedAlarm.PIProperties["AcknowledgmentUserGroup"].Value.ToString();
                textBox6.Text = editedAlarm.PIProperties["TraceInterval"].Value.ToString();
                textBox7.Text = editedAlarm.PIProperties["Type"].Value.ToString();
                //textBox8.Text = editedAlarm.PIProperties["Position"].Value.ToString();
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            updateAlarms();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {//��������� ��� � ������ ��� ��������� �������
            if ((int)e.KeyChar == 13)
            {
                this.editQueryObjectInfo(currentSymbol.Name, textBox1.Text, 0);
                updateAlarms();
            }
        }

        private void mdbTreeNet1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            MessageBox.Show("1");
            button5_Click(null, null);
        }

        private void mdbTreeNet1_MouseClick(object sender, MouseEventArgs e)
        {
            MessageBox.Show("2");
        }

        private void mdbTreeNet1_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show("3");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }


    class gPIProperty 
    {
        public PIProperty self = null;

        public gPIProperty(PIProperty p)
        {
            self = p;
        }
        public override string ToString()
        {
            return self.Name;
        }
    }
}