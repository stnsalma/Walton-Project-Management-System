using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class GovernmentHolidayTableModel
    {
        public long? Id { get; set; }
        public string GovernmentHoliday { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? HolidayDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? HolidayStartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? HolidayEndDate { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
        //Aftersales Holiday//

        public string HolidayName { get; set; }
        public string Month { get; set; }
        public int? MonNum { get; set; }
        public int? Year { get; set; }
        public DateTime? Holiday_SDate { get; set; }
        public DateTime? Holiday_EDate { get; set; }
      
    }
}