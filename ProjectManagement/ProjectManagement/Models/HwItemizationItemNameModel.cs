using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models
{
    public class HwItemizationItemNameModel
    {
        public long HwItemizationItemNameId { get; set; }
        [Required]
        public string ItemName { get; set; }
        public bool? IsActive { get; set; }
    }
}