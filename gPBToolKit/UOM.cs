
using System;
using System.Collections.Generic;
using System.Text;
using PBObjLib;
using PBSymLib;

namespace gPBToolKit
{
    class UOM
    {
        public static void Execute(PBObjLib.Application app)
        {
            Display ThisDisplay = app.ActiveDisplay;
            for (int i = 1; i <= ThisDisplay.SelectedSymbols.Count; i++)
            {
                try
                {
                    Symbol s = ThisDisplay.SelectedSymbols.Item(i);
                    if (s.Type == 7)
                    {
                        Value obj = (Value)ThisDisplay.SelectedSymbols.Item(i);
                        //((PBSymLib.Text)obj).CanonicalNumberFormat = 
                        obj.NumberFormat = "0.0";
                        obj.ShowUOM = true;
                        //((Symbol)obj).Font. = pbLeft;

                        //string[] strArray = obj.GetTagName(1).Split('\\');
                        //obj.SetTagName(@"\\10.53.87.143\\" + (strArray[strArray.Length - 1]));
                    }
                    /*
                    if (ThisDisplay.Symbols.Item(i).IsMultiState == true)
                    {
                        PBObjLib.MultiState mState;
                        string tagN;//As String
                        mState = ThisDisplay.Symbols.Item(i).GetMultiState();
                        tagN = mState.GetPtTagName();
                        mState.SetPtTagName(tagN);
                    }
                    bool AVEAlignProp = true;
                    
                    if (AVEAlignProp)
                    {
                        if (ThisDisplay.Symbols.Item(i).Type == 10)
                        {// узнать id trend
                            Trend objTrend = (Trend)ThisDisplay.Symbols.Item(i);
                            if (objTrend.Name == "Trend1")
                            {
                                objTrend.Top = 15000;
                                objTrend.Left = -12651;
                                objTrend.Width = 289;
                                objTrend.Height = 93;
                            }
                        }
                        
                        if (ThisDisplay.Symbols.Item(i).Name.IndexOf("AVExtension") >= 0)
                        {
                            ThisDisplay.Symbols.Item(i).Top = 15000;
                            ThisDisplay.Symbols.Item(i).Left = -13023;
                            ThisDisplay.Symbols.Item(i).Width = 373;
                            ThisDisplay.Symbols.Item(i).Height = 93;
                        }
                    }
                    */
                }
                catch{ }
            }
        }
    }
}
