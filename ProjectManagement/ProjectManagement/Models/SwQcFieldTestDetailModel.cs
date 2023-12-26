using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SwQcFieldTestDetailModel
    {
        public SwQcFieldTestDetailModel()
        {
         
            FilesDetails = new List<FilesDetail>();
        }
        public List<FilesDetail> FilesDetails { get; set; }
        public List<HttpPostedFileBase> FileId { get; set; }
        public List<HttpPostedFileBase> IssueAttachmentIds { get; set; }
       // public HttpPostedFileBase FileId { get; set; }
        public int IsRemoved { get; set; }
        public long FieldTestId { get; set; }
        public long SwQcHeadAssignId { get; set; }
        public long SwQcAssignId { get; set; }
        public long ProjectPmAssignId { get; set; }
        public long ProjectMasterId { get; set; }
        public int? OrderNumber { get; set; }
        public string ProjectName { get; set; }
        public string AssignedPerson { get; set; }
        public string FrequencyBand { get; set; }
        public string ProjectType { get; set; }
        public string Issue { get; set; }
        public string ExpectedOutcome { get; set; }
        public string IssueType { get; set; }
        public long? TestPhaseID { get; set; }
        public string SoftwareVersionName { get; set; }
        public int? SoftwareVersionNo { get; set; }
        public string FieldTestFrom { get; set; }
        public string UploadedFile { get; set; }
        public string Attachment { get; set; }
        public string IssueAttachment { get; set; }
        public string BenchmarkPhone { get; set; }
        public string Route { get; set; }
        public string Region { get; set; }
        public string FieldTestResult { get; set; }
        public string Remarks { get; set; }
        public string OperatorName { get; set; }
        
        public string Location { get; set; }
        public string SpeedLimit { get; set; }
        public string TRssiBars { get; set; }
        public string TCallDrop { get; set; }
        public string TNoiseInterference { get; set; }
        public string TLongMute { get; set; }
        public string BRssiBars { get; set; }
        public string BCallDrop { get; set; }
        public string BNoiseInterference { get; set; }
        public string BLongMute { get; set; }
        public DateTime? EntryDate { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
        //
        public string AirtelMt { get; set; }
        public string AirtelMtTRssiBars { get; set; }
        public string AirtelMtTCallDrop { get; set; }
        public string AirtelMtTNoiseInterference { get; set; }
        public string AirtelMtTLongMute { get; set; }
        public string AirtelMtBRssiBars { get; set; }
        public string AirtelMtBCallDrop { get; set; }
        public string AirtelMtBNoiseInterference { get; set; }
        public string AirtelMtBLongMute { get; set; }

        public string AirtelMo { get; set; }
        public string AirtelMoTRssiBars { get; set; }
        public string AirtelMoTCallDrop { get; set; }
        public string AirtelMoTNoiseInterference { get; set; }
        public string AirtelMoTLongMute { get; set; }
        public string AirtelMoBRssiBars { get; set; }
        public string AirtelMoBCallDrop { get; set; }
        public string AirtelMoBNoiseInterference { get; set; }
        public string AirtelMoBLongMute { get; set; }

        public string TeletalkMt { get; set; }
        public string TeletalkMtTRssiBars { get; set; }
        public string TeletalkMtTCallDrop { get; set; }
        public string TeletalkMtTNoiseInterference { get; set; }
        public string TeletalkMtTLongMute { get; set; }
        public string TeletalkMtBRssiBars { get; set; }
        public string TeletalkMtBCallDrop { get; set; }
        public string TeletalkMtBNoiseInterference { get; set; }
        public string TeletalkMtBLongMute { get; set; }

        public string TeletalkMo { get; set; }
        public string TeletalkMoTRssiBars { get; set; }
        public string TeletalkMoTCallDrop { get; set; }
        public string TeletalkMoTNoiseInterference { get; set; }
        public string TeletalkMoTLongMute { get; set; }
        public string TeletalkMoBRssiBars { get; set; }
        public string TeletalkMoBCallDrop { get; set; }
        public string TeletalkMoBNoiseInterference { get; set; }
        public string TeletalkMoBLongMute { get; set; }

        public string RobiMt { get; set; }
        public string RobiMtTRssiBars { get; set; }
        public string RobiMtTCallDrop { get; set; }
        public string RobiMtTNoiseInterference { get; set; }
        public string RobiMtTLongMute { get; set; }
        public string RobiMtBRssiBars { get; set; }
        public string RobiMtBCallDrop { get; set; }
        public string RobiMtBNoiseInterference { get; set; }
        public string RobiMtBLongMute { get; set; }

        public string RobiMo { get; set; }
        public string RobiMoTRssiBars { get; set; }
        public string RobiMoTCallDrop { get; set; }
        public string RobiMoTNoiseInterference { get; set; }
        public string RobiMoTLongMute { get; set; }
        public string RobiMoBRssiBars { get; set; }
        public string RobiMoBCallDrop { get; set; }
        public string RobiMoBNoiseInterference { get; set; }
        public string RobiMoBLongMute { get; set; }


        public string BanglalinkMt { get; set; }
        public string BanglalinkMtTRssiBars { get; set; }
        public string BanglalinkMtTCallDrop { get; set; }
        public string BanglalinkMtTNoiseInterference { get; set; }
        public string BanglalinkMtTLongMute { get; set; }
        public string BanglalinkMtBRssiBars { get; set; }
        public string BanglalinkMtBCallDrop { get; set; }
        public string BanglalinkMtBNoiseInterference { get; set; }
        public string BanglalinkMtBLongMute { get; set; }

        public string BanglalinkMo { get; set; }
        public string BanglalinkMoTRssiBars { get; set; }
        public string BanglalinkMoTCallDrop { get; set; }
        public string BanglalinkMoTNoiseInterference { get; set; }
        public string BanglalinkMoTLongMute { get; set; }
        public string BanglalinkMoBRssiBars { get; set; }
        public string BanglalinkMoBCallDrop { get; set; }
        public string BanglalinkMoBNoiseInterference { get; set; }
        public string BanglalinkMoBLongMute { get; set; }

        public string GrameenphoneMt { get; set; }
        public string GrameenphoneMtTRssiBars { get; set; }
        public string GrameenphoneMtTCallDrop { get; set; }
        public string GrameenphoneMtTNoiseInterference { get; set; }
        public string GrameenphoneMtTLongMute { get; set; }
        public string GrameenphoneMtBRssiBars { get; set; }
        public string GrameenphoneMtBCallDrop { get; set; }
        public string GrameenphoneMtBNoiseInterference { get; set; }
        public string GrameenphoneMtBLongMute { get; set; }

        public string GrameenphoneMo { get; set; }
        public string GrameenphoneMoTRssiBars { get; set; }
        public string GrameenphoneMoTCallDrop { get; set; }
        public string GrameenphoneMoTNoiseInterference { get; set; }
        public string GrameenphoneMoTLongMute { get; set; }
        public string GrameenphoneMoBRssiBars { get; set; }
        public string GrameenphoneMoBCallDrop { get; set; }
        public string GrameenphoneMoBNoiseInterference { get; set; }
        public string GrameenphoneMoBLongMute { get; set; }
    }
}