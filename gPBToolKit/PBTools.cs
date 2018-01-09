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
