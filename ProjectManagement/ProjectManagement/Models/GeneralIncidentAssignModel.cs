using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class GeneralIncidentAssignModel
    {
        public long AssignIncidentId { get; set; }
        public long? GeneralIncidentId { get; set; }
        public string AssignRemarks { get; set; }
        public long? AssignedBy { get; set; }
        public string AssignedByName { get; set; }
        public string AssignByRole { get; set; }
        public DateTime? AssignDate { get; set; }
        public long? AssignedTo { get; set; }
        public string AssignedToName { get; set; }
        public string AssignedToRole { get; set; }
        public string Status { get; set; }
    }
}