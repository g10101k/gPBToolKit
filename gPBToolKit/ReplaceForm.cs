using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PBObjLib;
using PBSymLib;

/* TODO: Запилить деактивацию пользовательского интерфейса формы
 * TODO: Попробовать запилить не модальную форму
 * TODO: Запилить выход по Esc
 */
 

namespace gPBToolKit
{
    public partial class ReplaceForm : Form
    {
        private PBObjLib.Application m_App = null;

        public ReplaceForm(PBObjLib.Application app)
        {
            m_App = app;
            InitializeComponent();
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            Display ThisDisplay = m_App.ActiveDisplay;
            int replaceCount = 0;
            for (int i = 1; i <= ThisDisplay.Symbols.Count; i++)
            {
                try
                {
                    Symbol s = ThisDisplay.Symbols.Item(i);
                    if (s.IsMultiState)
                    {
                        MultiState obj = s.GetMultiState();
                        if (obj.GetPtTagName() != obj.GetPtTagName().Replace(textBox1.Text, textBox2.Text))
                            replaceCount++;
                        obj.SetPtTagName(obj.GetPtTagName().Replace(textBox1.Text, textBox2.Text));
                    }

                    if (s.Type == 7 & textBox1.Text != "" & textBox2.Text != "")
                    {
                        Value obj = (Value)ThisDisplay.Symbols.Item(i);
                        //string tgNm = obj.GetTagName(1);
                        if (obj.GetTagName(1) != obj.GetTagName(1).Replace(textBox1.Text, textBox2.Text))
                            replaceCount++;
                        obj.SetTagName(obj.GetTagName(1).Replace(textBox1.Text, textBox2.Text));
                        ThisDisplay.Refresh();                        
                    }
                }
                catch (Exception ex){
                    MessageBox.Show(ex.Message);
                }
                
            }
            MessageBox.Show(string.Format("Replace {0} item(s)", replaceCount));
            this.Close();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 27)
                this.Close();
            if ((int)e.KeyChar == 13)
                button1_Click(null, null);
        }
    }
}