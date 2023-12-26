using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class PmWalpaperModel
    {
        public long PmWalpaperId { get; set; }
        public long ProjectAssignId { get; set; }
        public long ProjectMasterId { get; set; }
        public string WalpaperUpload1 { get; set; }
        public long AssignUserId { get; set; }
        public HttpPostedFileBase WalpaperFile1 { get; set; }

        public string WalpaperUpload2 { get; set; }

        public HttpPostedFileBase WalpaperFile2 { get; set; }
        public string WalpaperUpload3 { get; set; }

        public HttpPostedFileBase WalpaperFile3 { get; set; }
        public string WalpaperUpload4 { get; set; }

        public HttpPostedFileBase WalpaperFile4 { get; set; }
        public string WalpaperUpload5 { get; set; }

        public HttpPostedFileBase WalpaperFile5 { get; set; }
        public string WalpaperUpload6 { get; set; }

        public HttpPostedFileBase WalpaperFile6 { get; set; }
        public string WalpaperUpload7 { get; set; }

        public HttpPostedFileBase WalpaperFile7 { get; set; }

        public string W1Extension { get; set; }
        public string W2Extension { get; set; }
        public string W3Extension { get; set; }
        public string W4Extension { get; set; }
        public string W5Extension { get; set; }
        public string W6Extension { get; set; }
        public string W7Extension { get; set; }

        public string Remarks { get; set; }
        public long? Added { get; set; }
         [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}")]
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
         [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}")]
        public DateTime? UpdatedDate { get; set; }

        public long CmnUserId { get; set; }
        public string UserFullName { get; set; }
        public string UserName { get; set; }
        public string EmployeeCode { get; set; }

        public string PONumber { get; set; }

    }
}