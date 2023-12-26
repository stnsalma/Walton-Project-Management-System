using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class PmPhnAccessoriesModel
    {

        public long PmPhnAccessoriesID { get; set; }
        public long ProjectAssignId { get; set; }
        public long ProjectMasterId { get; set; }
        public string PmPhnAccessoriesEarphone { get; set; }
        public long AssignUserId { get; set; }
        public string PmPhnAccessoriesEarphoneExtension { get; set; }

        public HttpPostedFileBase PmPhnAccessoriesEarphoneFile { get; set; }

        public string PmPhnAccessoriesUSBCable { get; set; }
        public string PmPhnAccessoriesUSBCableExtension { get; set; }
        public HttpPostedFileBase PmPhnAccessoriesUSBCableFile { get; set; }

        public string PmPhnAccessoriesCharger { get; set; }
        public string PmPhnAccessoriesChargerExtension { get; set; }

        public HttpPostedFileBase PmPhnAccessoriesChargerFile { get; set; }
        public string PmPhnAccessoriesOTGCable { get; set; }
        public string PmPhnAccessoriesOTGCableExtension { get; set; }


        public HttpPostedFileBase PmPhnAccessoriesOTGCableFile { get; set; }

        public string PmPhnAccessoriesBackCover { get; set; }
        public string PmPhnAccessoriesBackCoverExtension { get; set; }
        public HttpPostedFileBase PmPhnAccessoriesBackCoverFile { get; set; }



        public string PmPhnAccessoriesFlipCover { get; set; }
        public string PmPhnAccessoriesFlipCoverExtension { get; set; }

        public HttpPostedFileBase PmPhnAccessoriesFlipCoverFile { get; set; }
        public string Remarks { get; set; }
        public Nullable<long> Added { get; set; }
         [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}")]
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
         [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}")]
        public Nullable<System.DateTime> UpdatedDate { get; set; }

        public long CmnUserId { get; set; }
        public string UserFullName { get; set; }
        public string UserName { get; set; }
        public string EmployeeCode { get; set; }

        public string PONumber { get; set; }
    }
}