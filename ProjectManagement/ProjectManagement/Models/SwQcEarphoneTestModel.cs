using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SwQcEarphoneTestModel
    {
        public string DoneBy { get; set; }
        public int IsRemoved { get; set; }
        public long AccessId { get; set; }
        public long? ProjectMasterId { get; set; }
        public long? SwQcHeadAssignId { get; set; }
        public long? SwQcAssignId { get; set; }
        public long? TestPhaseID { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        public string HeadphoneModel { get; set; }
        public string MusicPlayerPlayback { get; set; }
        public string VideoPlayerPlayback { get; set; }
        public string VoiceCall { get; set; }
        public string VoiceCallController { get; set; }
        public string FmPlayback { get; set; }
        public string Controller { get; set; }
        public string Remarks { get; set; }
        public string MusicBase { get; set; }
        public string YoutubePlayback { get; set; }
        public string YoutubeController { get; set; }
        public string FmController { get; set; }
        public string VolumeController { get; set; }
        public string HighEndDevice { get; set; }
        public string MidRangeDevice { get; set; }
        public string LowerMidRangeDevice { get; set; }
        public string LowRangeDevice { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}