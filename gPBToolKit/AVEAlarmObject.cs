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

namespace gPBToolKit
{
    class AVEAlarmObject
    {
        public string ObjectName;
        public string MDBPath;
        public PBObjLib.Symbol AVESymbol;

        public AVEAlarmObject()
        {
            ObjectName = "";
            MDBPath = "";
            AVESymbol = null;
        }

        public AVEAlarmObject(string _ObjectName, string _MDBPath, PBObjLib.Symbol _AVESymbol)
        {
            ObjectName = _ObjectName;
            MDBPath = _MDBPath;
            AVESymbol = _AVESymbol;
        }
    }
}
