using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SupplierModel
    {
        public long SupplierId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string SupplierName { get; set; }
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Address is required")]
        public string SupplierAddress { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; }
        public DateTime? EstablishmentDate { get; set; }
        public bool? HasCompanyIdh { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}