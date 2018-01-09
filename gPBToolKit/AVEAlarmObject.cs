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
