using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PBObjLib;
using PBSymLib;
using PISDK;

namespace gPBToolKit
{
    public partial class MultiStateMinMax : Form
    {
        PBObjLib.Application m_App = null;

        public static void Execute(PBObjLib.Application app)
        {

        }

        public MultiStateMinMax(PBObjLib.Application app)
        {
            m_App = app;
            InitializeComponent();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            MainExecute(e.KeyChar);
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            MainExecute(e.KeyChar);
        }

        private void MainExecute(char Char)
        {
            int key = Convert.ToInt16(Char);
            if (key == 27)
                this.Close();
            if (key == 13)
            {
                if (m_App.ActiveDisplay.SelectedSymbols.Count > 0)
                {
                    for (int i = 1; i <= m_App.ActiveDisplay.SelectedSymbols.Count; i++)
                    {
                        Symbol BufValue = m_App.ActiveDisplay.SelectedSymbols.Item(i);
                        if (BufValue.Type == 7) // 7 it is Value
                        {
                            if (BufValue.GetTagName(1) != "")
                            {
                                MainExecuteEx(BufValue);
                            }
                        }
                    }
                }               
            }
        }

        private void MainExecuteEx(PBObjLib.Symbol SymbolValue)
        {
            string tagName;
            tagName = SymbolValue.GetTagName(1);

            double fMin = 0, fMax = 0;

            if (textBox1.Text != "")
            {
                fMin = Convert.ToDouble(textBox1.Text
                    .Replace(".", ",")
                    .Replace("/", ",")
                    .Replace("?", ",")
                    .Replace(">", ",")
                    .Replace("б", ","));
            }
            if (textBox2.Text != "")
            {
                fMax = Convert.ToDouble(textBox2.Text
                    .Replace(".", ",")
                    .Replace("/", ",")
                    .Replace("?", ",")
                    .Replace(">", ",")
                    .Replace("б", ","));
            }

            double tagZero = 0, tagSpan = 0;

            SymbolValue.BackgroundColor = -1;
            ServerManagerClass srvMgr = new ServerManagerClass();
            Server server = srvMgr.PISDK.Servers[(tagName.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries))[0]];
            PIPoint pt = server.PIPoints[(tagName.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries))[1]];
            if (fMin > 0)
            {
                tagZero = 0;
                tagSpan = fMax + 100;
            }
            else
            {
                tagZero = fMin - 100;
                tagSpan = fMax + (fMin * -1) + 200;// Math.Floor(((fMax - fMin) * 1.5));
            }
            pt.PointAttributes.ReadOnly = false;
            pt.PointAttributes["Zero"].Value = tagZero;
            pt.PointAttributes["Span"].Value = tagSpan;
            pt.PointAttributes.ReadOnly = true;

            //pt.s

            Symbol tmpRect;
            tmpRect = SymbolValue.Application.ActiveDisplay.Symbols.Add(PBObjLib.pbSYMBOLTYPE.pbSymbolRectangle, "");

            tmpRect.Width = SymbolValue.Width;
            tmpRect.Height = SymbolValue.Height;
            tmpRect.Top = SymbolValue.Top;
            tmpRect.Left = SymbolValue.Left;
            tmpRect.LineColor = -1;

            tmpRect.CreateMultiState(tagName);
            PBObjLib.MultiState tMState = tmpRect.GetMultiState();



            int stateCount = 1;// As Integer

            if (fMax != 0)
                stateCount = stateCount + 1;

            if (fMin != 0)
                stateCount = stateCount + 1;

            tMState.StateCount = stateCount;
            //double tagSpan = 100; // В итоге берется из тега))
            MSState MyState;// As MSState

            if (stateCount == 2)
            {
                if (textBox1.Text == "")
                {
                    tMState.DefineState(1, tagZero, fMax);
                    tMState.DefineState(2, fMax, tagZero + tagSpan); //'Максимальное значение тега (span)
                    MyState = tMState.GetState(1);
                    MyState.DefineState(tagZero, fMax, 32768); //'Красный
                    MyState = tMState.GetState(2);
                    MyState.DefineState(fMax, MyState.UpperValue, 255);//'Зелень
                }
                else
                {
                    tMState.DefineState(1, tagZero, fMin);
                    tMState.DefineState(2, fMin, tagSpan);//'Максимальное значение тега (span)
                    MyState = tMState.GetState(1);
                    MyState.DefineState(tagZero, fMin, 255);//'Красный
                    MyState = tMState.GetState(2);
                    MyState.DefineState(fMin, MyState.UpperValue, 32768);// 'Зелень
                }
            }

            if (stateCount == 3)
            {
                tMState.DefineState(1, 0, fMin);
                tMState.DefineState(2, fMin, fMax);
                tMState.DefineState(3, fMax, tagZero + tagSpan);
                MyState = tMState.GetState(1);
                MyState.Color = 255;//DefineState(tagZero, fMin, 255);//'Красный
                MyState = tMState.GetState(2);
                MyState.Color = 32768;//DefineState(fMin, fMax, 32768); //'Зелень
                MyState = tMState.GetState(3);
                MyState.Color = 255;
            }
            int lCnt = tmpRect.Layers.Count;
            if (lCnt > 0)
            {
                tmpRect.Layers.Item(lCnt).LayerSymbols.Remove(tmpRect.Name);
                SymbolValue.Layers.AddSymbol(SymbolValue.Name);
            }
            this.Close();
        }
    }
}