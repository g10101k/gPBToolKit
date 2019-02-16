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
using PBObjLib;
using PBSymLib;
 
namespace gPBToolKit
{
    public partial class SearchForm : Form
    {
        private PBObjLib.Application m_App = null;

        public SearchForm(PBObjLib.Application app)
        {
            m_App = app;
            InitializeComponent();
        }       

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text != "")
                {
                    this.Enabled = false;
                    Display ThisDisplay = m_App.ActiveDisplay;
                    int replaceCount = 0;
                    
                    for (int i = 1; i <= ThisDisplay.SelectedSymbols.Count; i++)
                        ThisDisplay.SelectedSymbols.Item(i).Selected = false;

                    for (int i = 1; i <= ThisDisplay.Symbols.Count; i++)
                    {

                        Symbol s = ThisDisplay.Symbols.Item(i);
                        if (s.Type == 7)
                        {
                            string tagName = s.GetTagName(1);
                            if ((tagName.ToLower().IndexOf(textBox1.Text.ToLower()) >= 0))
                            {
                                s.Selected = true;
                                replaceCount++;
                            }
                        }
                        if (s.IsMultiState)
                        {
                            string tagName = s.GetMultiState().GetPtTagName();
                            if ((tagName.ToLower().IndexOf(textBox1.Text.ToLower()) >= 0))
                            {
                                s.Selected = true;
                                replaceCount++;
                            }
                        }
                        if (s.Type == 10)
                        {
                            Trend t = (Trend)s;
                            int count = t.PtCount;

                            for (int j = 1; j <= count; j++)
                            {
                                string tagName = t.GetTagName(j);
                                if ((tagName.ToLower().IndexOf(textBox1.Text.ToLower()) >= 0))
                                {
                                    s.Selected = true;
                                    replaceCount++;
                                    break;
                                }
                            }
                        }

                        if (s.Type == 12)
                        {
                            string tagName = ((Bar)s).GetTagName(1);
                            if ((tagName.ToLower().IndexOf(textBox1.Text.ToLower()) >= 0))
                            {
                                s.Selected = true;
                                replaceCount++;
                            }
                        }
                    }
                    MessageBox.Show(string.Format("find {0} item(s)", replaceCount));
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }  
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == 27)
                this.Close();
            if ((int)e.KeyChar == 13)
                button1_Click(null, null);
        }
    }
}