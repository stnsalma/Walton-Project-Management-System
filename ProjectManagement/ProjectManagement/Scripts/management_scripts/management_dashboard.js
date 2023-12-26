var clickCount = 0;
var parentName = "";
var childName = "";
var childValue = 0;
var overallGanttObject=[];
$(function () {
    $('#ProjectName').on('change', function () {
        var projectname = $('#ProjectName option:selected').val();
        loadOrderNumbers(projectname);
        
    });
    
    function loadOrderNumbers(projectname) {
        $('#ProjectMasterId').empty();
        $('#ProjectMasterId').append($('<option></option>').html('Loading...'));
        var url = '../Common/GetOrderNumbersByProjectName';
        $.post(url, { projectName: projectname }, function (data) {
            console.log(data);
            var orders = JSON.parse(data);
            console.log(orders);
            var mySelect;
            if (projectname != "") {
                mySelect = $('#ProjectMasterId');
                mySelect.empty();
                mySelect.append($('<option></option>').val("").html("Select Order"));
                for (var i = 0; i < orders.length; i++) {
                    mySelect.append($('<option></option>').val(orders[i].Value).html(orders[i].Text));
                }
            } else {
                makeAllprojectGanttChart();
                mySelect = $('#ProjectMasterId');
                mySelect.empty();
                mySelect.append($('<option></option>').val("").html("Select Order"));
            }
        });
    }
    
    
    $('#ProjectMasterId').on('change', function () {
        var projectId = $('#ProjectMasterId option:selected').val();
        makeProjectWiseGnattChart(projectId);
    });
    
   
    
    var isMobile = detectBrowser();
    if (isMobile) {
        $('#ov_container').removeClass();
        $('#ov_container').addClass("overlay-content-mobile");
    } else {
        $('#ov_container').removeClass();
        $('#ov_container').addClass("overlay-content-pc");
    }
    //$(document).on({
    //    ajaxStart: function () { $body.addClass("loading"); },
    //    ajaxStop: function () { $body.removeClass("loading"); }
    //});
});
function get_chart(ths) {
    var v = document.getElementById(ths).value;
    var url = '../Management/MakeAllprojectGanttChart';
    $('#ganttloaderdiv').addClass("loader");
    $.post(url, {}, function (data) {
        //---------------------------------------------
        overallGanttObject = $.parseJSON(data);
        console.log(overallGanttObject);
        //------------------------------------------
        //build_ganttchart(overallGanttObject);
        console.log(ths);
        var len = overallGanttObject.length;
       
        if (v == 0) {
            build_ganttchart(overallGanttObject);
        } else {
            var filteredList = [];
            if (v == 1) {
                for (var i = 0; i < len; i++) {
                    if (overallGanttObject[i].IsCompleted == false) {
                        filteredList.push(overallGanttObject[i]);
                    }

                }
                build_ganttchart(filteredList);
            } else {
                for (var i = 0; i < len; i++) {
                    if (overallGanttObject[i].IsCompleted == true) {
                        filteredList.push(overallGanttObject[i]);
                    }

                }
                build_ganttchart(filteredList);
            }
        }
    });
    //$('#ganttloaderdiv').addClass("loader");
    
}

function makeAllprojectGanttChart() {
    
    var url = '../Management/MakeAllprojectGanttChart';
    $('#ganttloaderdiv').addClass("loader");
    $.post(url, {}, function (data) {
        //---------------------------------------------
        overallGanttObject = $.parseJSON(data);
        console.log(overallGanttObject);
        //------------------------------------------
        build_ganttchart(overallGanttObject);

    });

}

function build_ganttchart(overallStatObj) {
    var color = ["ggroupblack", "gtaskblue", "gtaskpurple", "gtaskred", "gtaskpink", "gtaskyellow", "gtaskgreen"];
    var g = new JSGantt.GanttChart(document.getElementById('GanttChartDIV'), 'week');

    g.setCaptionType('Complete');  // Set to Show Caption (None,Caption,Resource,Duration,Complete)
    g.setQuarterColWidth(36);
    g.setDateTaskDisplayFormat('day dd month yyyy'); // Shown in tool tip box
    g.setDayMajorDateDisplayFormat('mon yyyy - Week ww'); // Set format to display dates in the "Major" header of the "Day" view
    g.setWeekMinorDateDisplayFormat('dd mon'); // Set format to display dates in the "Minor" header of the "Week" view
    g.setShowTaskInfoLink(1); // Show link in tool tip (0/1)
    g.setShowEndWeekDate(0); // Show/Hide the date for the last day of the week in header for daily view (1/0)
    g.setUseSingleCell(10000); // Set the threshold at which we will only use one cell per table row (0 disables).  Helps with rendering performance for large charts.
    g.setShowComp(); // Controls whether the Percentage Complete column is displayed in the task list, defaults to 1 (show column)
    g.setShowRes();  //  Controls whether the Resource column is displayed in the task list, defaults to 1 (show column)

    g.setFormatArr('Week', 'Month', 'Quarter');
    g.setCaptionType('None'); // Valid parameter values are "None", "Caption", "Resource", "Duration", "Complete".Defaults to "None"


    for (var i = 0; i < overallStatObj.length; i++) {
        var randomcolor = color[Math.floor(Math.random() * color.length)];

        g.AddTaskItem(new JSGantt.TaskItem((1 + i), overallStatObj[i].ProjectName, '', '', 'ggroupblack', '', 0, '', 0, 2, 0, 1, '', '', '', g));
        if (overallStatObj[i].IsCompleted == null || overallStatObj[i].IsCompleted == false) {
            g.AddTaskItem(new JSGantt.TaskItem((10000 + i), 'Task done (%) bar of the Project: ' + overallStatObj[i].ProjectName, overallStatObj[i].StartDate, overallStatObj[i].EndDate, 'gtaskgreen', '', 0, '', overallStatObj[i].ActionCount, 0, (1 + i), 1, '', '', '', g));
        } else {
            g.AddTaskItem(new JSGantt.TaskItem((10000 + i), 'Task done (%) bar of the Project: ' + overallStatObj[i].ProjectName, overallStatObj[i].StartDate, overallStatObj[i].EndDate, 'gtaskgreen', '', 0, '', 100, 0, (1 + i), 1, '', '', 'PO Closing Date :' + moment(overallStatObj[i].PoClosingDate).format('llll'), g));
        }

        var enddate = new Date(overallStatObj[i].EndDate);
        var lastaction = new Date(overallStatObj[i].LastActionDate);
        if (lastaction > enddate) {
            g.AddTaskItem(new JSGantt.TaskItem((20000 + i), 'Last Action After End Date', overallStatObj[i].EndDate, overallStatObj[i].LastActionDate, 'gtaskred', '', 0, '', 0, 0, (1 + i), 1, (10000 + i), '', '', g));
        }
    };
    g.Draw();
    $('#ganttloaderdiv').removeClass("loader");
}

function makeProjectWiseGnattChart(id) {
    //alert(id);
    var url = '../Management/GetDataByModule';
    $.post(url, { id: id }, function (data) {
        console.log(data);
        var overallStatObj = JSON.parse(data);
        var completion = 0;
        console.log('===============================');
        console.log(overallStatObj);


        //Configuration Options Switches
        var g = new JSGantt.GanttChart(document.getElementById('GanttChartDIV'), 'week');
        if (g.getDivId() != null) {
            g.setCaptionType('Complete'); // Set to Show Caption (None,Caption,Resource,Duration,Complete)
            g.setQuarterColWidth(36);
            g.setDateTaskDisplayFormat('day dd month yyyy'); // Shown in tool tip box
            g.setDayMajorDateDisplayFormat('mon yyyy - Week ww'); // Set format to display dates in the "Major" header of the "Day" view
            g.setWeekMinorDateDisplayFormat('dd mon'); // Set format to display dates in the "Minor" header of the "Week" view
            g.setShowTaskInfoLink(1); // Show link in tool tip (0/1)
            g.setShowEndWeekDate(0); // Show/Hide the date for the last day of the week in header for daily view (1/0)
            g.setUseSingleCell(10000); // Set the threshold at which we will only use one cell per table row (0 disables).  Helps with rendering performance for large charts.
            g.setShowComp(); // Controls whether the Percentage Complete column is displayed in the task list, defaults to 1 (show column)
            g.setShowRes();  //  Controls whether the Resource column is displayed in the task list, defaults to 1 (show column)
            g.setShowTaskInfoComp(); //	Controls whether the Percentage Complete information is displayed in the task tool tip, defaults to 1 (show information)

            //Key Values
            g.setFormatArr('Hour','Day','Week', 'Month', 'Quarter'); // Valid parameter values are "Hour", "Day", "Week", "Month", "Quarter".Defaults to all valid values.
            g.setCaptionType('None'); // Valid parameter values are "None", "Caption", "Resource", "Duration", "Complete".Defaults to "None"


            //TaskItem(pID, pName, pStart, pEnd, pColor, pLink, pMile, pRes, pComp, pGroup, pParent, pOpen, pDepend, pCaption, pNotes, pGantt)
            g.AddTaskItem(new JSGantt.TaskItem(1, overallStatObj.CmStatusObject.ProjectName, overallStatObj.CmStatusObject.ProjectInitialize, overallStatObj.CmStatusObject.ApproxProjectFinishDate, 'ggroupblack', '', 0, 'WPMS', 0, 1, 0, 1, '', '', '', g));
            //CM Module
            g.AddTaskItem(new JSGantt.TaskItem(10, 'Commercial', '', '', 'ggroupblack', '', 0, 'WPMS', 0, 1, 1, 1, '', '', '', g));
            g.AddTaskItem(new JSGantt.TaskItem(11, 'Project Initialize', overallStatObj.CmStatusObject.ProjectInitialize, overallStatObj.CmStatusObject.ProjectInitialize, 'gmilestone', '', 1, 'WPMS', 100, 0, 10, 1, '', '', '', g));
            if (overallStatObj.CmStatusObject.IsCompleted == true) {
                g.AddTaskItem(new JSGantt.TaskItem(15, 'PO Closing date', overallStatObj.CmStatusObject.PoClosingDate, overallStatObj.CmStatusObject.PoClosingDate, 'gmilestone', '', 1, 'WPMS', 100, 0, 10, 1, '', '', '', g));
            }
            //MM Module
            g.AddTaskItem(new JSGantt.TaskItem(100, 'Management', '', '', 'ggroupblack', '', 0, 'WPMS', 0, 1, 1, 1, '', '', '', g));
            completion = overallStatObj.CmStatusObject.InitialApprovalDate == null ? 0 : 100;
            g.AddTaskItem(new JSGantt.TaskItem(101, 'Initial Approval', overallStatObj.CmStatusObject.ProjectInitialize, overallStatObj.CmStatusObject.InitialApprovalDate = overallStatObj.CmStatusObject.InitialApprovalDate == null ? overallStatObj.CmStatusObject.ProjectInitialize : overallStatObj.CmStatusObject.InitialApprovalDate, 'gtaskpurple', '', 0, 'WPMS', completion, 0, 100, 1, '', '', '', g));
            if (overallStatObj.HwScreeningStatusObject != null && overallStatObj.HwScreeningStatusObject.ScreeningForward != null) {
                g.AddTaskItem(new JSGantt.TaskItem(102, 'Final Approval', overallStatObj.HwScreeningStatusObject.ScreeningForward, overallStatObj.CmStatusObject.FinalApprovalDate = overallStatObj.CmStatusObject.FinalApprovalDate == null ? overallStatObj.HwScreeningStatusObject.ScreeningForward : overallStatObj.CmStatusObject.FinalApprovalDate, 'gtaskpurple', '', 0, 'WPMS', completion, 0, 100, 1, 1006, '', '', g));
            }
            
            //CM module
            if (overallStatObj.HwScreeningStatusObject != null) {
                completion = overallStatObj.HwScreeningStatusObject.ScreeningSampleSent == null ? 0 : 100;
                g.AddTaskItem(new JSGantt.TaskItem(12, 'Screening Sample Sent', overallStatObj.CmStatusObject.InitialApprovalDate, overallStatObj.HwScreeningStatusObject.ScreeningSampleSent = overallStatObj.HwScreeningStatusObject.ScreeningSampleSent == null ? overallStatObj.CmStatusObject.InitialApprovalDate : overallStatObj.HwScreeningStatusObject.ScreeningSampleSent, 'gtaskgreen', '', 0, 'WPMS', completion, 0, 10, 1, 101, '', '', g));
            }
            
            
            //HW Module
            g.AddTaskItem(new JSGantt.TaskItem(1000, 'Hardware', '', '', 'ggroupblack', '', 0, 'WPMS', 0, 1, 1, 1, '', '', '', g));
            //HW screening
            if (overallStatObj.CmStatusObject.OrderNuber == 1 && (overallStatObj.CmStatusObject.SourcingType == null || overallStatObj.CmStatusObject.SourcingType == 'OEM'))
            g.AddTaskItem(new JSGantt.TaskItem(1001, 'Screening', '', '', 'ggroupblack', '', 0, 'WPMS', 0, 1, 1000, 1, '', '', '', g));
            if (overallStatObj.HwScreeningStatusObject != null) {
                completion = overallStatObj.HwScreeningStatusObject.ScreeningSampleReceive == null ? 0 : 100;
                g.AddTaskItem(new JSGantt.TaskItem(1002, 'Screening Sample Receive', overallStatObj.HwScreeningStatusObject.ScreeningSampleSent, overallStatObj.HwScreeningStatusObject.ScreeningSampleReceive = overallStatObj.HwScreeningStatusObject.ScreeningSampleReceive == null ? overallStatObj.HwScreeningStatusObject.ScreeningSampleSent : overallStatObj.HwScreeningStatusObject.ScreeningSampleReceive, 'gtaskred', '', 0, 'WPMS', completion, 0, 1001, 1, 12, '', '', g));
                completion = overallStatObj.HwScreeningStatusObject.ScreeningAssign == null ? 0 : 100;
                g.AddTaskItem(new JSGantt.TaskItem(1003, 'Screening Engineer Assign', overallStatObj.HwScreeningStatusObject.ScreeningSampleReceive, overallStatObj.HwScreeningStatusObject.ScreeningAssign = overallStatObj.HwScreeningStatusObject.ScreeningAssign == null ? overallStatObj.HwScreeningStatusObject.ScreeningSampleReceive : overallStatObj.HwScreeningStatusObject.ScreeningAssign, 'gtaskred', '', 0, 'WPMS', completion, 0, 1001, 1, '', '', '', g));
                completion = overallStatObj.HwScreeningStatusObject.ScreeningSubmit == null ? 0 : 100;
                g.AddTaskItem(new JSGantt.TaskItem(1004, 'Screening Report Submit', overallStatObj.HwScreeningStatusObject.ScreeningAssign, overallStatObj.HwScreeningStatusObject.ScreeningSubmit = overallStatObj.HwScreeningStatusObject.ScreeningSubmit == null ? overallStatObj.HwScreeningStatusObject.ScreeningAssign : overallStatObj.HwScreeningStatusObject.ScreeningSubmit, 'gtaskred', '', 0, 'WPMS', completion, 0, 1001, 1, '', '', '', g));
                completion = overallStatObj.HwScreeningStatusObject.ScreeningVerified == null ? 0 : 100;
                g.AddTaskItem(new JSGantt.TaskItem(1005, 'Screening Report Verified', overallStatObj.HwScreeningStatusObject.ScreeningSubmit, overallStatObj.HwScreeningStatusObject.ScreeningVerified = overallStatObj.HwScreeningStatusObject.ScreeningVerified == null ? overallStatObj.HwScreeningStatusObject.ScreeningSubmit : overallStatObj.HwScreeningStatusObject.ScreeningVerified, 'gtaskred', '', 0, 'WPMS', completion, 0, 1001, 1, '', '', '', g));
                completion = overallStatObj.HwScreeningStatusObject.ScreeningForward == null ? 0 : 100;
                g.AddTaskItem(new JSGantt.TaskItem(1006, 'Screening Report Forwarded', overallStatObj.HwScreeningStatusObject.ScreeningVerified, overallStatObj.HwScreeningStatusObject.ScreeningForward = overallStatObj.HwScreeningStatusObject.ScreeningForward == null ? overallStatObj.HwScreeningStatusObject.ScreeningVerified : overallStatObj.HwScreeningStatusObject.ScreeningForward, 'gtaskred', '', 0, 'WPMS', completion, 0, 1001, 1, '', '', '', g));
            }
            //HW Running
            g.AddTaskItem(new JSGantt.TaskItem(4, 'Running', '', '', 'ggroupblack', '', 0, 'WPMS', 0, 1, 1000, 1, '', '', '', g));
            if (overallStatObj.HwRunningStatusObject != null) {
                completion = overallStatObj.HwRunningStatusObject.RunningSampleReceive == null ? 0 : 100;
                g.AddTaskItem(new JSGantt.TaskItem(402, 'Running Sample Receive', overallStatObj.HwRunningStatusObject.RunningSampleSent, overallStatObj.HwRunningStatusObject.RunningSampleReceive = overallStatObj.HwRunningStatusObject.RunningSampleReceive == null ? overallStatObj.HwRunningStatusObject.RunningSampleSent : overallStatObj.HwRunningStatusObject.RunningSampleReceive, 'gtaskred', '', 0, 'WPMS', completion, 0, 4, 1, 22, '', '', g));
                completion = overallStatObj.HwRunningStatusObject.RunningAssign == null ? 0 : 100;
                g.AddTaskItem(new JSGantt.TaskItem(403, 'Running Engineer Assign', overallStatObj.HwRunningStatusObject.RunningSampleReceive, overallStatObj.HwRunningStatusObject.RunningAssign = overallStatObj.HwRunningStatusObject.RunningAssign == null ? overallStatObj.HwRunningStatusObject.RunningSampleReceive : overallStatObj.HwRunningStatusObject.RunningAssign, 'gtaskred', '', 0, 'WPMS', completion, 0, 4, 1, '', '', '', g));
                completion = overallStatObj.HwRunningStatusObject.RunningSubmit == null ? 0 : 100;
                g.AddTaskItem(new JSGantt.TaskItem(404, 'Running Report Submit', overallStatObj.HwRunningStatusObject.RunningAssign, overallStatObj.HwRunningStatusObject.RunningSubmit = overallStatObj.HwRunningStatusObject.RunningSubmit == null ? overallStatObj.HwRunningStatusObject.RunningAssign : overallStatObj.HwRunningStatusObject.RunningSubmit, 'gtaskred', '', 0, 'WPMS', completion, 0, 4, 1, '', '', '', g));
                completion = overallStatObj.HwRunningStatusObject.RunningVerified == null ? 0 : 100;
                g.AddTaskItem(new JSGantt.TaskItem(405, 'Running Report Verified', overallStatObj.HwRunningStatusObject.RunningSubmit, overallStatObj.HwRunningStatusObject.RunningVerified = overallStatObj.HwRunningStatusObject.RunningVerified == null ? overallStatObj.HwRunningStatusObject.RunningSubmit : overallStatObj.HwRunningStatusObject.RunningVerified, 'gtaskred', '', 0, 'WPMS', completion, 0, 4, 1, '', '', '', g));
                completion = overallStatObj.HwRunningStatusObject.RunningForward == null ? 0 : 100;
                g.AddTaskItem(new JSGantt.TaskItem(406, 'Running Report Forwarded', overallStatObj.HwRunningStatusObject.RunningVerified, overallStatObj.HwRunningStatusObject.RunningForward = overallStatObj.HwRunningStatusObject.RunningForward == null ? overallStatObj.HwRunningStatusObject.RunningVerified : overallStatObj.HwRunningStatusObject.RunningForward, 'gtaskred', '', 0, 'WPMS', completion, 0, 4, 1, '', '', '', g));
            }
            //HW Finished 
            g.AddTaskItem(new JSGantt.TaskItem(5, 'Finished', '', '', 'ggroupblack', '', 0, 'WPMS', 0, 1, 1000, 1, '', '', '', g));
            if (overallStatObj.HwFinishedStatusObject != null) {
                completion = overallStatObj.HwFinishedStatusObject.FinishedSampleReceive == null ? 0 : 100;
                g.AddTaskItem(new JSGantt.TaskItem(600, 'Finished Sample Receive', overallStatObj.HwFinishedStatusObject.RunningSampleSent, overallStatObj.HwFinishedStatusObject.FinishedSampleReceive = overallStatObj.HwFinishedStatusObject.FinishedSampleReceive == null ? overallStatObj.HwFinishedStatusObject.RunningSampleSent : overallStatObj.HwFinishedStatusObject.FinishedSampleReceive, 'gtaskred', '', 0, 'WPMS', completion, 0, 5, 1, '', '', '', g));
                g.AddTaskItem(new JSGantt.TaskItem(601, 'Finished Engineer Assign', overallStatObj.HwFinishedStatusObject.FinishedSampleReceive, overallStatObj.HwFinishedStatusObject.FinishedAssign = overallStatObj.HwFinishedStatusObject.FinishedAssign == null ? overallStatObj.HwFinishedStatusObject.FinishedSampleReceive : overallStatObj.HwFinishedStatusObject.FinishedAssign, 'gtaskred', '', 0, 'WPMS', completion, 0, 5, 1, '', '', '', g));
            }
            //CM Module
            if (overallStatObj.HwScreeningStatusObject != null) {
                if (overallStatObj.CmStatusObject.ScreeningIssueReview != null) {
                    completion = overallStatObj.CmStatusObject.ScreeningIssueReview == null ? 0 : 100;
                    overallStatObj.HwScreeningStatusObject.ScreeningForward = overallStatObj.HwScreeningStatusObject.ScreeningForward == null ? overallStatObj.HwScreeningStatusObject.ScreeningVerified : overallStatObj.HwScreeningStatusObject.ScreeningForward;
                    g.AddTaskItem(new JSGantt.TaskItem(13, 'Screening Issue Review', overallStatObj.HwScreeningStatusObject.ScreeningForward, overallStatObj.CmStatusObject.ScreeningIssueReview = overallStatObj.CmStatusObject.ScreeningIssueReview == null ? overallStatObj.HwScreeningStatusObject.ScreeningForward : overallStatObj.CmStatusObject.ScreeningIssueReview, 'gtaskgreen', '', 0, 'WPMS', completion, 0, 10, 1, 1006, '', '', g));
                    completion = overallStatObj.CmStatusObject.PurchaseOrder == null ? 0 : 100;
                    g.AddTaskItem(new JSGantt.TaskItem(14, 'Purchase Order', overallStatObj.CmStatusObject.ScreeningIssueReview, overallStatObj.CmStatusObject.PurchaseOrder = overallStatObj.CmStatusObject.PurchaseOrder == null ? overallStatObj.CmStatusObject.ScreeningIssueReview : overallStatObj.CmStatusObject.PurchaseOrder, 'gtaskgreen', '', 0, 'WPMS', completion, 0, 10, 1, 102, '', '', g));
                } else {
                    completion = overallStatObj.CmStatusObject.PurchaseOrder == null ? 0 : 100;
                    if (overallStatObj.CmStatusObject.FinalApprovalDate == null) {
                        g.AddTaskItem(new JSGantt.TaskItem(14, 'Purchase Order', overallStatObj.HwScreeningStatusObject.ScreeningForward, overallStatObj.CmStatusObject.PurchaseOrder = overallStatObj.CmStatusObject.PurchaseOrder == null ? overallStatObj.HwScreeningStatusObject.ScreeningForward : overallStatObj.CmStatusObject.PurchaseOrder, 'gtaskgreen', '', 0, 'WPMS', completion, 0, 10, 1, 102, '', '', g));
                    } else {
                        g.AddTaskItem(new JSGantt.TaskItem(14, 'Purchase Order', overallStatObj.CmStatusObject.FinalApprovalDate, overallStatObj.CmStatusObject.PurchaseOrder = overallStatObj.CmStatusObject.PurchaseOrder == null ? overallStatObj.HwScreeningStatusObject.ScreeningForward : overallStatObj.CmStatusObject.PurchaseOrder, 'gtaskgreen', '', 0, 'WPMS', completion, 0, 10, 1, 102, '', '', g));
                    }
                }
            }
            
            
            //PM Module
            g.AddTaskItem(new JSGantt.TaskItem(2, 'ProjectManager', '', '', 'ggroupblack', '', 0, 'WPMS', 0, 1, 1, 1, '', '', '', g));
            if (overallStatObj.CmStatusObject.PurchaseOrder != null) {
                if (overallStatObj.PmStatusObject != null) {
                    completion = overallStatObj.PmStatusObject.PmAssignDate == null ? 0 : 100;
                    g.AddTaskItem(new JSGantt.TaskItem(20, 'PM Assign', overallStatObj.CmStatusObject.PurchaseOrder, overallStatObj.PmStatusObject.PmAssignDate = overallStatObj.PmStatusObject.PmAssignDate == null ? overallStatObj.CmStatusObject.PurchaseOrder : overallStatObj.PmStatusObject.PmAssignDate, 'gtaskyellow', '', 0, 'WPMS', completion, 0, 2, 1, 14, '', '', g));
                    if (overallStatObj.SwStatusObjects != null) {
                        for (var j = 0; j < overallStatObj.SwStatusObjects.length; j++) {
                            completion = overallStatObj.SwStatusObjects[j].ProjectManagerAssignToQcInTime == null ? 0 : 100;
                            if (j == 0) {
                                g.AddTaskItem(new JSGantt.TaskItem(21 + j, 'Sofware QC forward (version' + (j + 1) + ' )', overallStatObj.PmStatusObject.PmAssignDate, overallStatObj.SwStatusObjects[j].ProjectManagerAssignToQcInTime, 'gtaskyellow', '', 0, 'WPMS', completion, 0, 2, 1, '', '', '', g));
                            } else {
                                g.AddTaskItem(new JSGantt.TaskItem(21 + j, 'Sofware QC forward (version ' + (j + 1) + ' )', overallStatObj.SwStatusObjects[j - 1].ProjectManagerAssignToQcInTime, overallStatObj.SwStatusObjects[j].ProjectManagerAssignToQcInTime, 'gtaskyellow', '', 0, 'WPMS', completion, 0, 2, 1, '', '', '', g));
                            }

                        }
                    }
                    completion = overallStatObj.PmStatusObject.RunningForwardDate == null ? 0 : 100;
                    g.AddTaskItem(new JSGantt.TaskItem(22, 'HW Running Test forward', overallStatObj.PmStatusObject.PmAssignDate, overallStatObj.PmStatusObject.RunningForwardDate = overallStatObj.PmStatusObject.RunningForwardDate == null ? overallStatObj.PmStatusObject.PmAssignDate : overallStatObj.PmStatusObject.RunningForwardDate, 'gtaskyellow', '', 0, 'WPMS', completion, 0, 2, 1, '', '', '', g));
                }
            }
            
            //SW Module
            g.AddTaskItem(new JSGantt.TaskItem(3, 'Software QC', '', '', 'ggroupblack', '', 0, 'WPMS', 0, 1, 1, 1, '', '', '', g));
            if (overallStatObj.PmStatusObject!=null && overallStatObj.PmStatusObject.SwQcForwardDate != null) {
                for (var i = 0; i < overallStatObj.SwStatusObjects.length; i++) {
                    g.AddTaskItem(new JSGantt.TaskItem(30+i, 'Software QC Assign(version ' + (i + 1)+' )', overallStatObj.SwStatusObjects[i].ProjectManagerAssignToQcInTime, overallStatObj.SwStatusObjects[i].QcInchargeToQcAssignTime = overallStatObj.SwStatusObjects[i].QcInchargeToQcAssignTime == null ? overallStatObj.SwStatusObjects[i].ProjectManagerAssignToQcInTime : overallStatObj.SwStatusObjects[i].QcInchargeToQcAssignTime, 'gtaskblue', '', 0, 'WPMS', completion, 0, 3, 1, 21+i, '', '', g));
                    completion = overallStatObj.SwStatusObject.QcInchargeToPmProjectSubmitTime == null ? 0 : 100;
                    g.AddTaskItem(new JSGantt.TaskItem(300+i, 'Forward QC to PM(version ' + (i + 1) + ' )', overallStatObj.SwStatusObjects[i].QcInchargeToQcAssignTime, overallStatObj.SwStatusObjects[i].QcInchargeToPmProjectSubmitTime = overallStatObj.SwStatusObjects[i].QcInchargeToPmProjectSubmitTime == null ? overallStatObj.SwStatusObjects[i].QcInchargeToQcAssignTime : overallStatObj.SwStatusObjects[i].QcInchargeToPmProjectSubmitTime, 'gtaskblue', '', 0, 'WPMS', completion, 0, 3, 1, '', '', '', g));
                }
                
            }
            
            
            g.Draw();
        } else {
            alert("Error, unable to create Gantt Chart");
        }
    });
}

function make_work_progress_chart(projectId) {
    var url = 'GetWorkProgress';
    $.post(url, { projectId: projectId }).done(function (data) {
        var parsed = JSON.parse(data);
        console.log(parsed);

        var d = new Date();
        var n = d.getFullYear();
        var y = n.toString();
        var options = Highcharts.chart('container', {
            chart: {
                type: ''
            },
            title: {
                text: '',
                //x: -20 //center 
                style: {
                    display: 'none'
                }
            },
            subtitle: {
                text: 'Source: WPMS',
                x: -20
            },
            xAxis: {
                categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
                    'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
            },
            yAxis: {
                title: {
                    text: 'For the year' + y
                },
                plotLines: [{
                    value: 0,
                    width: 1,
                    color: '#808080'
                }]
            },
            tooltip: {
                valueSuffix: 'Actions'
            },

            series: [

            ]
        });
        for (var i = 0; i < parsed.length; i++) {
            options.addSeries({
                name: parsed[i].name,
                data: parsed[i].data
            });
        };


    });
} 

function make_issues_chart() {
    
    var url = 'GetProjectsByIssueCount';
    $.get(url).done(function (data) {
        console.log(data);
        var parsed = JSON.parse(data);
        var serieseArray = [];
        var obj;//= { name: "", id: "", data: [] };
        if (parsed.IssueGraphModels[0].name === "No Data Found") {
            console.log("True");
        } else {
            for (var it = 0; it < parsed.IssueGraphDrillDownModels.length; it++) {
                obj = new Object();
                obj.name = parsed.IssueGraphDrillDownModels[it].name;
                obj.id = parsed.IssueGraphDrillDownModels[it].id;
                var s = ["Solved", parsed.IssueGraphDrillDownModels[it].data[0].Solved];
                var u = ["Unsolved", parsed.IssueGraphDrillDownModels[it].data[1].Unsolved];
                var f = ["Forwarded", parsed.IssueGraphDrillDownModels[it].data[2].Forwarded];
                var ar = [];
                ar.push(s);
                ar.push(u);
                ar.push(f);
                obj.data = ar;
                serieseArray.push(obj);
            }
        }


        Highcharts.chart('issueGraph', {
            chart: {
                type: 'pie',
                options3d: {
                    enabled: true,
                    alpha: 45,
                    beta: 0
                },
                events: {
                    drillup: function (e) {
                        clickCount = 0;
                    }
                }
            },
            title: {
                text: ''
            },
            subtitle: {
                text: ''
            },
            plotOptions: {
                series: {
                    events:{
                        click: function (event) {
                            clickCount = clickCount + 1;
                            if (clickCount < 2) {
                                parentName = event.point.name;
                            }
                            else if (clickCount > 1) {
                                childName = event.point.name;
                                childValue = event.point.y;
                            }
                            
                            if (clickCount >= 2 && childValue > 0) {
                                make_a_pop_up(childName, parentName);
                            }
                            if (childValue <= 0 && clickCount > 1) alertify.alert("No Data Available");
                        }
                    }
                },
                pie: {
                    innerSize: '30%',
                    //allowPointSelect: true,
                    cursor: 'pointer',
                    depth: 35,
                    dataLabels: {
                        enabled: true,
                        format: '{point.name}: {point.y}'
                    }
                }
            },

            tooltip: {
                headerFormat: '',
                pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>{point.y}</b> of total<br/>'
            },
            series: [{

                data: parsed.IssueGraphModels
            }],
            drilldown: {
                series: serieseArray
            }
        });

    });



}

function make_comments_chart() {
    var url = 'GetProjectsByCommentCount';
    $.get(url).done(function (data) {
        console.log(data);
        var parsed = JSON.parse(data);
        var serieseArray = [];
        Highcharts.chart('commentGraph', {
            chart: {
                type: 'pie',
                options3d: {
                    enabled: true,
                    alpha: 45,
                    beta: 0
                }
            },
            title: {
                text: ''
            },
            subtitle: {
                text: ''
            },
            plotOptions: {
                pie: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    depth: 35,
                    dataLabels: {
                        enabled: true,
                        format: '{point.name}: {point.y}'
                    }
                }
            },

            tooltip: {
                headerFormat: '',
                pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>{point.y}</b> of total<br/>'
            },
            series: [{

                data: parsed,
                point: {
                    events: {
                        click: function (event) {
                            //alert(this.id);
                        }
                    }
                }
            }],
            
        });

    });
}

function get_recent_notifications_as_feed() {
    var url = 'GetRecentNofificationAsFeed';
    $.post(url, {}).done(function (data) {
        $('.dashboardFeed').empty();
        var response = jQuery.parseJSON(data);
        $.each(response, function (index, value) {
            var additionalInformation = "";
            if (value.AdditionalMessage) {
                additionalInformation = "Additional Info: " + value.AdditionalMessage;
            }
            var liVariable = '<li><div class=col1><div class=cont><div class=cont-col1><div class="label label-info label-sm"><i class="fa fa-check"></i></div></div><div class=cont-col2><div class=desc>' + value.Message + '<br/>' + additionalInformation + '</div></div></div></div><div class=col2><div class=date>' + value.NotificationTime + '</div></div>';
            $('.dashboardFeed').append($(liVariable));

        });
    });
}

function get_project_wise_recent_notifications_as_feed(id) {
    var url = 'GetProjectWiseRecentNofificationAsFeed';
    $.post(url, { projectId: id }).done(function (data) {
        $('.dashboardFeed').empty();
        var response = jQuery.parseJSON(data);
        $.each(response, function (index, value) {
            var additionalInformation = "";
            if (value.AdditionalMessage) {
                additionalInformation = "Additional Info: " + value.AdditionalMessage;
            }
            
            var liVariable = '<li><div class=col1><div class=cont><div class=cont-col1><div class="label label-info label-sm"><i class="fa fa-check"></i></div></div><div class=cont-col2><div class=desc>' + value.Message + '<br/>' + additionalInformation + '</div></div></div></div><div class=col2><div class=date>' + value.NotificationTime + '</div></div>';
            $('.dashboardFeed').append($(liVariable));

        });
    });
}

function get_time_for_notification(value) {
    var justNow = '';
    var currentDateTime = new Date();
    var dbDateTime = new Date(value);
    //dbDateTime.setHours(dbDateTime.getHours());

    var diff = currentDateTime.getTime() - dbDateTime.getTime();
    var days = Math.floor(diff / (1000 * 60 * 60 * 24));
    diff -= days * (1000 * 60 * 60 * 24);

    var hours = Math.floor(diff / (1000 * 60 * 60));
    diff -= hours * (1000 * 60 * 60);

    var mins = Math.floor(diff / (1000 * 60));
    diff -= mins * (1000 * 60);

    var seconds = Math.floor(diff / (1000));
    diff -= seconds * (1000);
    if (days > 0) {
        justNow = days + " D. Ago";
    }
    else if (days <= 0 && hours > 0) {
        justNow = hours + " H. Ago";
    }
    else if (days <= 0 && hours <= 0 && mins > 0) {
        justNow = mins + " M. Ago";
    }
    else if (days <= 0 && hours <= 0 && mins <= 0 && seconds > 0) {
        justNow = seconds + " S. Ago";
    } else {
        justNow = "Now";
    }
    return justNow;

}

function make_a_pop_up(childName, parentName) {
    var url = "GetIssuePieData";
    openNav();
    $('#d_container').addClass("loading");
    //$('.slideFeed').empty();
    
    $.post(url,{projectName:parentName,status:childName}).done(function(data) {
        console.log(data);
        $('#d_container').removeClass("loading");
        var parsed = JSON.parse(data);
        var len = parsed.length;
        $('.issuePieFeeds').empty();
        for (var i = 0; i < len; i++) {
            console.log('-----------------' + i + '----------------');
            console.log(parsed[i]);
            var status = "";
            if (parsed[i].FlagStatus.match("^Complete")) {
                status = '<span class=item-status><span class="badge badge-empty badge-success"></span> ' + parsed[i].FlagStatus + '</span>';
            }
            else if (parsed[i].FlagStatus.match("^Pending")) {
                status = '<span class=item-status><span class="badge badge-empty badge-warning"></span> '+parsed[i].FlagStatus+'</span>';
            }
            else if (parsed[i].FlagStatus.match("^Forwarded")) {
                status = '<span class=item-status><span class="badge badge-empty badge-primary"></span> ' + parsed[i].FlagStatus + '</span>';
            }
            else if (parsed[i].FlagStatus.match("^Closed")) {
                status = '<span class=item-status><span class="badge badge-empty badge-danger"></span> ' + parsed[i].FlagStatus + '</span>';
            }
            
            var listElement = '<div class=item><div class=item-head><div class=item-details><img class=item-pic src=' + parsed[i].ProfilePicture + '> <a class="item-name primary-link"href=#> ' + parsed[i].CreatorName + '</a> <span class=item-label>' + get_time_for_notification(parsed[i].AddedDate)+ '</span></div>' + status + '</div><div class=item-body>Issue : ' + parsed[i].Description + '. </br>Component Names: ' + parsed[i].Component + '</div></div>';
            $('.issuePieFeeds').append(listElement);
        }
    });
}

//window.onclick = closeNav();
function openNav() {
    document.getElementById("myNav").style.width = "100%";
}

function closeNav() {
    document.getElementById("myNav").style.width = "0%";
}

window.onclick = function(event) {
    if (event.target == document.getElementById("myNav")) {
        closeNav();
    }
};

function detectBrowser() {
    var isMobile = false; //initiate as false
    // device detection
    if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|ipad|iris|kindle|Android|Silk|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino/i.test(navigator.userAgent)
        || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(navigator.userAgent.substr(0, 4))) isMobile = true;

    return isMobile;
}
