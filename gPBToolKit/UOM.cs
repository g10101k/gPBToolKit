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
                        {// ������ id trend
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
