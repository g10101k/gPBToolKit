using System;
using System.Windows.Forms;
using OSIsoft.AF;
using OSIsoft.AF.Asset;
using OSIsoft.AF.UI;
using PBObjLib;
using PBSymLib;

namespace gPBToolKit
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AFGroupReplace : Form
    {
        private PBObjLib.Application m_App = null;
        private static PISystem m_System;
        private static AFDatabase m_Database;

        public AFGroupReplace(PBObjLib.Application app)
        {
            InitializeComponent();
            m_App = app;

            PISystems systems = new PISystems();  
            m_System = systems.DefaultPISystem;
            if (m_Database == null) {
                DialogResult dialogResult;
                m_Database = AFOperations.ConnectToDatabase(this, m_System.Name, "", true, out dialogResult);
            }
        }

        private void Button1Click(object sender, EventArgs e)
        {
            AFElement foundElement = null;
            bool res = AFOperations.BrowseElement(this, m_Database, null, ref foundElement);
            if (!res || foundElement == null) {
                MessageBox.Show("Element not selected", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string afelemName = foundElement.Name;
            
            Display disp = m_App.ActiveDisplay;
            var selectedSyms = disp.SelectedSymbols;

            int replaceCount = 0;
            for (int i = 1; i <= selectedSyms.Count; i++) {
                try {
                    Symbol sym = selectedSyms.Item(i);

                    if (sym.IsMultiState) {
                        MultiState obj = sym.GetMultiState();

                        string tagName = obj.GetPtTagName();
                        string newName = ReplaceElementName(tagName, afelemName);

                        if (tagName != newName) {
                            obj.SetPtTagName(newName);
                            replaceCount++;
                        }
                    }

                    if (sym.Type == (int)PBObjLib.pbSYMBOLTYPE.pbSymbolValue) {
                        Value obj = (Value)disp.Symbols.Item(i);

                        string tagName = obj.GetTagName(1);
                        string newName = ReplaceElementName(tagName, afelemName);

                        if (tagName != newName) {
                            obj.SetTagName(newName);
                            replaceCount++;
                        }
                    }
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
            }

            MessageBox.Show(string.Format("Replace {0} item(s)", replaceCount));
            disp.Refresh();                        
            Close();
        }

        private string ReplaceElementName(string tagName, string newElementName)
        {
            if (tagName.IndexOf('|') < 0)
                return tagName;

            string[] parts = tagName.Split('|');

            string result = newElementName;
            for (int i = 1; i < parts.Length; i++) {
                result += "|" + parts[i];
            }

            return result;
        }
    }
}
