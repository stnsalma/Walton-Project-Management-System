using System.Collections.Generic;
using ProjectManagement.Models;

namespace ProjectManagement.Infrastructures.Helper
{
    public class SwBatteryStaticListGenerator
    {

        //public List<SwQcAssignIssueModel> GetPreList(long projectId)
        //{
        //    var issueModels1 = new List<SwQcAssignIssueModel>();
        //    return issueModels1;
        //}

        public List<SwQcBatteryAssignIssueModel> GetStaticList(long projectId)
        {
            var issueModels = new List<SwQcBatteryAssignIssueModel>();
            var model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                //SwQcAssignId = 2,
                ModuleName = "Standby",
                CheckingOption = "",
                Decreased = "100-0",
                Time = ""

            };
            issueModels.Add(model);
           
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Standby",
                CheckingOption = "",
                Decreased = "100-95",
                Time = ""
            };
            // models.Add(model);
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Standby",
                CheckingOption = "",
                Decreased = "100-90",
                Time = ""
            };
            // models.Add(model);
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Standby",
                CheckingOption = "",
                Decreased = "90-80",
                Time = ""
            };
            //  models.Add(model);
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Standby",
                CheckingOption = "",
                Decreased = "80-70",
                Time = ""
            };
            //models.Add(model);
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Standby",
                CheckingOption = "",
                Decreased = "70-60",
                Time = ""
            };
            // models.Add(model);
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Standby",
                CheckingOption = "",
                Decreased = "60-50",
                Time = ""
            };
            // models.Add(model);
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Standby",
                CheckingOption = "",
                Decreased = "50-40",
                Time = ""
            };
            //models.Add(model);
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Standby",
                CheckingOption = "",
                Decreased = "40-30",
                Time = ""
            };
            // models.Add(model);
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Standby",
                CheckingOption = "",
                Decreased = "30-20",
                Time = ""
            };
            // models.Add(model);
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Standby",
                CheckingOption = "",
                Decreased = "20-10",
                Time = ""
            };
            // models.Add(model);
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Standby",
                CheckingOption = "",
                Decreased = "10-0",
                Time = ""
            };
            // models.Add(model);
            issueModels.Add(model);

            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Usability",
                CheckingOption = "Youtube Video Playback (720p)",
                Decreased = "",
                Time = ""
            };
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Usability",
                CheckingOption = "HD Video Playback",
                Decreased = "",
                Time = ""
            };
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Usability",
                CheckingOption = "3D Game Play (Asphalt 8 Airborn)",
                Decreased = "",
                Time = ""
            };
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Usability",
                CheckingOption = "Music Playback",
                Decreased = "",
                Time = ""
            };
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Usability",
                CheckingOption = "Camera Use",
                Decreased = "",
                Time = ""
            };
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Usability",
                CheckingOption = "Calling",
                Decreased = "",
                Time = ""
            };
            issueModels.Add(model);

            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Power On/Off",
                Charging = "0-10",
                Time = "",
                Voltage = ""
            };
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Power On/Off",
                Charging = "10-20",
                Time = "",
                Voltage = ""
            };
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Power On/Off",
                Charging = "20-30",
                Time = "",
                Voltage = ""
            };
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Power On/Off",
                Charging = "30-40",
                Time = "",
                Voltage = ""
            };
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Power On/Off",
                Charging = "40-50",
                Time = "",
                Voltage = ""
            };
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Power On/Off",
                Charging = "50-60",
                Time = "",
                Voltage = ""
            };
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Power On/Off",
                Charging = "60-70",
                Time = "",
                Voltage = ""
            };
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Power On/Off",
                Charging = "70-80",
                Time = "",
                Voltage = ""
            };
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Power On/Off",
                Charging = "80-90",
                Time = "",
                Voltage = ""
            };
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                ModuleName = "Power On/Off",
                Charging = "90-100",
                Time = "",
                Voltage = ""
            };
            issueModels.Add(model);

            // model = new SwQcBatteryAssignIssueModel { IsIssueChecked = false, ModuleName = "Others", Issues = "Measure heat of charger during charging", IssueComment = "", ScreenShot1FilePath = " ", ScreenShot2FilePath = "", VideoUpload1FilePath = "", VideoUpload2FilePath = "" };
            //models.Add(model);


            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                IsIssueChecked = false,
                ModuleName = "Others",
                Issues = "Measure heat of charger during charging",
                IssueComment = "",
                ScreenShot1FilePath = "",
                ScreenShot2FilePath = "",
                VideoUpload1FilePath = "",
                VideoUpload2FilePath = ""
            };
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                IsIssueChecked = false,
                ModuleName = "Others",
                Issues = "Measure heat of the device during Game after 1hour of play : Asphalt 8",
                IssueComment = "",
                ScreenShot1FilePath = "",
                ScreenShot2FilePath = "",
                VideoUpload1FilePath = "",
                VideoUpload2FilePath = ""
            };
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                IsIssueChecked = false,
                ModuleName = "Others",
                Issues = "Measure heat of the device during Game after 1hour of play : Clash of Clans",
                IssueComment = "",
                ScreenShot1FilePath = "",
                ScreenShot2FilePath = "",
                VideoUpload1FilePath = "",
                VideoUpload2FilePath = ""
            };
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                IsIssueChecked = false,
                ModuleName = "Others",
                Issues = " Measure heat of the device during Game after 1hour of play : Modern combat",
                IssueComment = "",
                ScreenShot1FilePath = "",
                ScreenShot2FilePath = "",
                VideoUpload1FilePath = "",
                VideoUpload2FilePath = ""
            };
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                IsIssueChecked = false,
                ModuleName = "Others",
                Issues = "Charger test with spec & ensure proper charging",
                IssueComment = "",
                ScreenShot1FilePath = "",
                ScreenShot2FilePath = "",
                VideoUpload1FilePath = "",
                VideoUpload2FilePath = ""
            };
            issueModels.Add(model);
            model = new SwQcBatteryAssignIssueModel
            {
                SwQcBatteryAssignIssuesId = 0,
                ProjectMasterId = projectId,
                IsIssueChecked = false,
                ModuleName = "Others",
                Issues = "Operate handset with charger connected",
                IssueComment = "",
                ScreenShot1FilePath = "",
                ScreenShot2FilePath = "",
                VideoUpload1FilePath = "",
                VideoUpload2FilePath = ""
            };
            issueModels.Add(model);
            return issueModels;
        }
    }
}