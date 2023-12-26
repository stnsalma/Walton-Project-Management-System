using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class LineInformationModel
    {
        public long Id { get; set; }
        public string ProjectType { get; set; }
        public string LineNumber { get; set; }
        public long? Charger_SMT_Line_Capacity { get; set; }
        public long? Charger_Housing_Line_Capacity { get; set; }
        public long? Charger_Assembly_Line_Capacity { get; set; }
        public long? CKD_SMT_Line_Capacity { get; set; }
        public long? CKD_Housing_Line_Capacity { get; set; }
        public long? CKD_Battery_Line_Capacity { get; set; }
        public long? CKD_Assembly_Line_Capacity { get; set; }
    }
}