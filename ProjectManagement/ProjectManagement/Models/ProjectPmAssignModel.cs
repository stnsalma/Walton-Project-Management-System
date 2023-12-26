using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models
{
    public class ProjectPmAssignModel
    {

        public long ProjectPmAssignId { get; set; }
        public long ProjectMasterId { get; set; }
        public string PONumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime AssignDate { get; set; }
        public long AssignUserId { get; set; }
        public long ProjectManagerUserId { get; set; }
        public string ProjectHeadRemarks { get; set; }
        public string UserFullName { get; set; }
        public string Status { get; set; }
        public string ProjectHeadInactiveRemarks { get; set; }
        public DateTime? InactiveDate { get; set; }
        public DateTime? ApproxPmInchargeToPmFinishDate { get; set; }
        public string PoCategory { get; set; }
        public string AssignedByName { get; set; }
        public string ProjectName { get; set; }
        public int? OrderNuber { get; set; }

    }
}