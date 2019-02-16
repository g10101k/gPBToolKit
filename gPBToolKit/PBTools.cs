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
    class PBTools
    {
        public static void SearchSymbolByTagName(PBObjLib.Application app)
        {
            Display ThisDisplay = app.ActiveDisplay;
            for (int i = 1; i <= ThisDisplay.Symbols.Count; i++)
            {
                try
                {
                    Symbol s = ThisDisplay.Symbols.Item(i);
                    if (s.Type == 7)
                    {
                        ThisDisplay.Symbols.Item(i).Selected = true;
                    }

                }
                catch{ }
            }
        }
    }
}
