using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.ViewModels.Commercial
{
    public class ModelListForIMEIDownload
    {
        public long SERIAL_NO { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public string IMEI_TAC_1 { get; set; }
        public string IMEI_TAC_2 { get; set; }
        public string IMEI_TAC_3 { get; set; }
        public string IMEI_TAC_4 { get; set; }
        public string IMEI1 { get; set; }
        public string IMEI2 { get; set; }
        public string IMEI3 { get; set; }
        public string IMEI4 { get; set; }
    }
}