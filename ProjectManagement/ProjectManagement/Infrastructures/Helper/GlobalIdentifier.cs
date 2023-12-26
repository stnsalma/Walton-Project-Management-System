using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Infrastructures.Helper
{
    public class GlobalIdentifier
    {
        public enum CommercialTabType
        {
            Pcba = 1, TpLcd, Housing, Camera, Chipset, Memory, Sensor, Accessories, Os, Network, Battery, Color
        }
        public enum MessageReturnType
        {
            Success = 1, Error, Warning
        }

        public enum ProjectManagerTabIdentifier
        {
            BootAnimation = 1, GiftBox, Label, IdModel, Sprotector, Walpaper, SwCustomization, Accessories, ServiceDoc,Camera,Color
        
        }

        //public enum SoftwareTabType
        //{
        //    StartSw = 1, Callset, MessageSw, ToolsSw, CameraSw, DisplayLoopSw, DisplaySw, SettingsSw, MultimediaSw, GoogleSw, StorageSw, GameSw, TestingSw, FileSw, ConnectivitySw, ShutSw
        //}

        public enum Role
        {
            PMHEAD = 1, PM, QCHEAD, QC, CM
        }

    }
}