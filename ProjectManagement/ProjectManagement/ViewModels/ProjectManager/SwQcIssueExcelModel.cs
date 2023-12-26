using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;

namespace ProjectManagement.ViewModels.ProjectManager
{
    public class SwQcIssueExcelModel
    {
        [Required(ErrorMessage = "Please Select Excel File."), RegularExpression(@"([a-zA-Z0-9\s_\\.\-:\+-])+(.xlsx|.xls)$", ErrorMessage = "Only Excel File allowed.")]
        public HttpPostedFileBase ExcelFile { get; set; }

        [Required(ErrorMessage = "Please enter Software Version No."), Range(1, 100, ErrorMessage = "Please Select Software Version")]

        public long SoftVersionNo { get; set; }

        [Required(ErrorMessage = "Please Select Project.")]
        public string SelectedProjectName { get; set; }
        public string CombinedTestPhaseIds { get; set; }
        public long SelectedProjectId { get; set; }
    }
}