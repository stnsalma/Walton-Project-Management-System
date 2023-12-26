function projectStatusForCm(projectId,url) {
    var obj = {};
    obj.projectMasterId = projectId;


    //var url = '../Common/ProjectStatusForHw';
    $.ajax({
        url: url,
        type: 'POST',
        data: obj,
        async: false,
        success: function (data) {

            //alertify.alert(data.projectMasterId);
        }

    });
}