
///@file
///
/// @par Copyright © 2008 OSI Software, Inc.
/// All rights reserved.\n\n
/// <B>Unpublished</B> - rights reserved under the copyright law of the United
/// States.  Use of A copyright notice is precautionary only and does not imply
/// publication or disclosure.\n\n
/// This software contains confidential information and trade secrets of OSI
/// Software, Inc. use, disclosure, or reproduction is prohibited without the
/// prior express written permission of OSI Software, Inc.
///
/// @par Restricted rights legend
/// Use, duplication, or disclosure by the Government is subject to
/// restrictions as set forth in subparagraph (c)(1)(ii) of the Rights in
/// Technical Data and Computer Software clause at DFARS 252.227.7013
/// \n\n
/// <A href=http://www.OSIsoft.com>OSI Software, Inc.</A>
/// 777 Davis St., San Leandro CA 94577
/// \n\n
// Modification history at bottom of file.

using System;
using PBObjLib;
using PBSymLib;
using Extensibility;
using System.Collections;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace gPBToolKit
{
    [GuidAttribute("F097B2F4-32FD-4022-ABDB-6F69B82F0289"), ProgId("gPBToolKit.Connect")]
    public class Connect : IDTExtensibility2, IDisposable
    {
        private PBObjLib.Application m_theApp = null;
        public TreeNode treeNode;

        private string m_strAddInName = "About";
        private string m_strAddInName2 = "MultiState";
        private string m_strAddInName3 = "Set UOM";
        private string m_strAddInName4 = "Alarm";
        private string m_strAddInName5 = "ReplaceInValue";
        private string m_strAddInName6 = "SearchTag";
        private string m_strAddInName7 = "gScript";
        private string m_strAddInName8 = "AF Group Replace";

        #region command bar

        private PBCommandBarControl m_pbControl = null;
        private PBCommandBarControl m_pbControlAlign = null;
        private PBCommandBarControl m_pbControlUOM = null;
        private PBCommandBarControl m_pbControlAlarm = null;
        private PBCommandBarControl m_pbControlReplace = null;
        private PBCommandBarControl m_pbControlSearch = null;
        private PBCommandBarControl m_pbControlScript = null;
        private PBCommandBarControl m_pbControlAFGR = null;

        #endregion

        public Connect()
        {
        }

        public void Dispose()
        {
        }

        public void OnConnection(object Application, Extensibility.ext_ConnectMode ConnectMode, object AddInInst, ref System.Array custom)
        {
            try {
                m_theApp = (PBObjLib.Application)Application;

                #region command bar

                //this code attaches the add-in to the add-in command bar
                bool bPropertyExist = false;
                bool bMakeCoolExist = false;
                bool bUOMExist = false;
                bool bAlarmExist = false;
                bool bReplaceExist = false;
                bool bSearchTag = false;
                bool bScript = false;
                bool bAFGR = false;

                //delete this code if you do not desire to have the add-in added
                //to the commandbar as a button
                //add the command bar if it is not there.....
                PBObjLib.PBCommandBar pbCommandBar = m_theApp.CommandBars.Item("Add-Ins");
                if (pbCommandBar == null) {
                    m_theApp.CommandBars.Add("Add-Ins", 1, false, false);
                    pbCommandBar = m_theApp.CommandBars.Item("Add-Ins");
                }

                if (pbCommandBar != null) {

                    PBObjLib.PBCommandBarControls pbControls = (PBObjLib.PBCommandBarControls)pbCommandBar.Controls;
                    System.Collections.IEnumerator enumerator = pbControls.GetEnumerator();

                    while (enumerator.MoveNext()) {
                        PBObjLib.PBCommandBarControl cb = (PBObjLib.PBCommandBarControl)enumerator.Current;
                        //cb.Delete();
                        string strCurrName = cb.Caption.ToString();
                        if (strCurrName == m_strAddInName)
                            bPropertyExist = true;
                        if (strCurrName == m_strAddInName2)
                            bMakeCoolExist = true;
                        if (strCurrName == m_strAddInName3)
                            bUOMExist = true;
                        if (strCurrName == m_strAddInName4)
                            bAlarmExist = true;
                        if (strCurrName == m_strAddInName5)
                            bReplaceExist = true;
                        if (strCurrName == m_strAddInName6)
                            bSearchTag = true;
                        if (strCurrName == m_strAddInName7)
                            bScript = true;
                        if (strCurrName == m_strAddInName8)
                            bAFGR = true;
                    }

                    if (!bPropertyExist) {
                        m_pbControl = (PBObjLib.PBCommandBarControl)pbControls.Add((Object)PBObjLib.pbControlType.pbControlButton, 1, null, null, false);
                        m_pbControl.Caption = m_strAddInName;
                        m_pbControl.ToolTipText = Res2.m_pbControlSettingsToolTipText;
                        
                        ((ECommandBarButtonEvents_Event)m_pbControl).Click += new PBObjLib.ECommandBarButtonEvents_ClickEventHandler(this.AddInClicked);
                    } else
                        m_pbControl = (PBObjLib.PBCommandBarControl)pbControls.Item(m_strAddInName);

                    if (!bMakeCoolExist) {
                        m_pbControlAlign = (PBObjLib.PBCommandBarControl)pbControls.Add((Object)PBObjLib.pbControlType.pbControlButton, 1, null, null, false);
                        m_pbControlAlign.Caption = m_strAddInName2;
                        
                        ((ECommandBarButtonEvents_Event)m_pbControlAlign).Click += new PBObjLib.ECommandBarButtonEvents_ClickEventHandler(this.FillGood);
                    } else
                        m_pbControlAlign = (PBObjLib.PBCommandBarControl)pbControls.Item(m_strAddInName2);

                    if (!bUOMExist) {
                        m_pbControlUOM = (PBObjLib.PBCommandBarControl)pbControls.Add((Object)PBObjLib.pbControlType.pbControlButton, 1, null, null, false);
                        m_pbControlUOM.Caption = m_strAddInName3;

                        ((ECommandBarButtonEvents_Event)m_pbControlUOM).Click += new PBObjLib.ECommandBarButtonEvents_ClickEventHandler(this.SetUOM);
                    } else
                        m_pbControlUOM = (PBObjLib.PBCommandBarControl)pbControls.Item(m_strAddInName3);

                    if (!bAlarmExist) {
                        m_pbControlAlarm = (PBObjLib.PBCommandBarControl)pbControls.Add((Object)PBObjLib.pbControlType.pbControlButton, 1, null, null, false);
                        m_pbControlAlarm.Caption = m_strAddInName4;
                        ((ECommandBarButtonEvents_Event)m_pbControlAlarm).Click += new PBObjLib.ECommandBarButtonEvents_ClickEventHandler(this.ChangeAlarm);
                    } else
                        m_pbControlAlarm = (PBObjLib.PBCommandBarControl)pbControls.Item(m_strAddInName4);

                    if (!bReplaceExist) {
                        m_pbControlReplace = (PBObjLib.PBCommandBarControl)pbControls.Add((Object)PBObjLib.pbControlType.pbControlButton, 1, null, null, false);
                        m_pbControlReplace.Caption = m_strAddInName5;
                        ((ECommandBarButtonEvents_Event)m_pbControlReplace).Click += new PBObjLib.ECommandBarButtonEvents_ClickEventHandler(this.ReplaceInValue);
                    } else
                        m_pbControlReplace = (PBObjLib.PBCommandBarControl)pbControls.Item(m_strAddInName5);

                    if (!bSearchTag) {
                        m_pbControlSearch = (PBObjLib.PBCommandBarControl)pbControls.Add((Object)PBObjLib.pbControlType.pbControlButton, 1, null, null, false);
                        m_pbControlSearch.Caption = m_strAddInName6;
                        ((ECommandBarButtonEvents_Event)m_pbControlSearch).Click += new PBObjLib.ECommandBarButtonEvents_ClickEventHandler(this.SearchValue);
                    } else
                        m_pbControlSearch = (PBObjLib.PBCommandBarControl)pbControls.Item(m_strAddInName6);
                    
                    if (!bScript) {
                        m_pbControlScript = (PBObjLib.PBCommandBarControl)pbControls.Add((Object)PBObjLib.pbControlType.pbControlButton, 1, null, null, false);
                        m_pbControlScript.Caption = m_strAddInName7;
                        ((ECommandBarButtonEvents_Event)m_pbControlScript).Click += new PBObjLib.ECommandBarButtonEvents_ClickEventHandler(this.ShowScriptForm);
                    } else
                        m_pbControlScript = (PBObjLib.PBCommandBarControl)pbControls.Item(m_strAddInName7);

                    if (!bAFGR) {
                        m_pbControlAFGR = (PBObjLib.PBCommandBarControl)pbControls.Add((Object)PBObjLib.pbControlType.pbControlButton, 1, null, null, false);
                        m_pbControlAFGR.Caption = m_strAddInName8;
                        ((ECommandBarButtonEvents_Event)m_pbControlScript).Click += this.ShowAFGR;
                    } else
                        m_pbControlAFGR = (PBObjLib.PBCommandBarControl)pbControls.Item(m_strAddInName8);
                }

                #endregion
            } catch (Exception ex) {
                MessageBox.Show("Exception in OnConnection=" + ex.Message);
            }
        }

        public void OnDisconnection(Extensibility.ext_DisconnectMode RemoveMode, ref System.Array custom)
        {
            #region command bar

            m_pbControl.Delete();
            m_pbControlAlign.Delete();
            m_pbControlUOM.Delete();
            m_pbControlAlarm.Delete();
            m_pbControlReplace.Delete();
            m_pbControlSearch.Delete();
            m_pbControlScript.Delete();

            #endregion

            System.GC.Collect();
        }

        public void OnAddInsUpdate(ref System.Array custom)
        {
        }

        public void OnBeginShutdown(ref System.Array custom)
        {
        }

        public void OnStartupComplete(ref System.Array custom)
        {
        }

        #region command bar

        // Event Handler
        // Opens a Simple Form, Select cancel to close
        public void AddInClicked(PBCommandBarButton commandBarButton, ref bool b1)
        {
            frmAbout dlg = new frmAbout(m_theApp);
            dlg.ShowDialog();
        }

        public void FillGood(PBCommandBarButton commandBarButton, ref bool b1)
        {
            MultiStateMinMax frm = new MultiStateMinMax(m_theApp);
            frm.ShowDialog();
        }

        public void SetUOM(PBCommandBarButton commandBarButton, ref bool b1)
        {
            UOM.Execute(m_theApp);
        }

        public void ChangeAlarm(PBCommandBarButton commandBarButton, ref bool b1)
        {
            AlarmManager frm = new AlarmManager(m_theApp, this);
            frm.ShowDialog();
        }

        public void ReplaceInValue(PBCommandBarButton commandBarButton, ref bool b1)
        {
            ReplaceForm frm = new ReplaceForm(m_theApp);
            frm.ShowDialog();
        }

        public void SearchValue(PBCommandBarButton commandBarButton, ref bool b1)
        {
            SearchForm frm = new SearchForm(m_theApp);
            frm.ShowDialog();
        }

        public void ShowScriptForm(PBCommandBarButton commandBarButton, ref bool b1)
        {
            ScriptForm frm = new ScriptForm(m_theApp);
            frm.ShowDialog();
        }

        public void ShowAFGR(PBCommandBarButton commandBarButton, ref bool b1)
        {
            AFGroupReplace frm = new AFGroupReplace(m_theApp);
            frm.ShowDialog();
        }

        #endregion
    }
}
