function hwBackCameraIc(backCameraJsonObj) {
    
    $('#HwBackCameraIcModel_BackCameraIcId').val($('.hdnBackCameraSelectedValue').val());
    $('#HwBackCameraIcModel_BackCameraIcId').chosen();
    

    
    $('#HwBackCameraIcModel_BackCameraIcId').on('change', function () {
        //Passing value to PcbAModel
        var selectedIcNoSize = $('#HwBackCameraIcModel_BackCameraIcId option:selected').text();
        $('#HwTestCameraInfoModel_BackCamera_IcNoSize').val(selectedIcNoSize);
        //---------------------
        console.log(backCameraJsonObj);
        for (var i in backCameraJsonObj) {
            if ($('#HwBackCameraIcModel_BackCameraIcId option:selected').val() == backCameraJsonObj[i].BackCameraIcId) {
                var id = backCameraJsonObj[i].BackCameraIcId;
                //var pmu1IcNoSize = backCameraJsonObj[i].IcNoSize;
                var backCameraVendor = backCameraJsonObj[i].BackCamera_Vendor;
                var pinNumber = backCameraJsonObj[i].PinNumber;
                var pinType = backCameraJsonObj[i].PinType;
                console.log(id);
                $('#HwTestCameraInfoModel_BackCamera_Vendor').val(backCameraVendor);
                $('#HwTestCameraInfoModel_BackCamera_PinNumber').val(pinNumber);
                $('#HwTestCameraInfoModel_BackCamera_PinType').val(pinType);
            }
        }
    });

    $('#btnSaveBackCameraIc').on('click', function (e) {

        e.preventDefault();

        var obj = {};
        obj.HwQcAssignId = $('#hdnHwQcAssignId').val();
        obj.IcNoSize = $.trim($('#HwBackCameraIcModel_IcNoSize').val());
        obj.Vendor = $('#HwBackCameraIcModel_BackCameraVendor').val();
        obj.PinType = $('#HwBackCameraIcModel_PinType').val();
        obj.PinNumber = $('#HwBackCameraIcModel_PinNumber').val();
        obj.Remarks = $('#HwBackCameraIcModel_Remarks').val();


        if (obj.IcNoSize != "") {
            var url = 'PostHwBackCameraIc';

            $.ajax({
                url: url,
                type: 'POST',
                data: obj,
                async: false,
                success: function(response) {
                    console.log(response);
                    $('#backCameraIcModal').modal('hide');
                    alertify.alert("Back Camera IC saved");
                    var test = JSON.parse(response);
                    if (test != null) {
                        var newOption = $('<option value="' + test.BackCameraIcId + '">' + test.IcNoSize + '</option>');
                        $('#HwBackCameraIcModel_BackCameraIcId').append(newOption);
                        $('#HwBackCameraIcModel_BackCameraIcId').val(test.BackCameraIcId);
                        $('#HwTestCameraInfoModel_BackCamera_IcNoSize').val(test.IcNoSize);
                        $("#HwBackCameraIcModel_BackCameraIcId").trigger("chosen:updated");

                        var pushObj = { BackCameraIcId: test.BackCameraIcId, IcNoSize: test.IcNoSize };
                        backCameraJsonObj.push(pushObj);
                        console.log(backCameraJsonObj);
                    } else {
                        alertify.alert("This Back camera IC already exists");
                    }

                },
                error: function(jqXhr, textStatus, errorThrown) {
                    alert('error ..Check log for details');
                    console.log(textStatus, errorThrown);
                    var msg = '';
                    if (jqXhr.status === 0) {
                        msg = 'Not connect.\n Verify Network.';
                    } else if (jqXhr.status == 404) {
                        msg = 'Requested page not found. [404]';
                    } else if (jqXhr.status == 500) {
                        msg = 'Internal Server Error [500].';
                    } else if (errorThrown === 'parsererror') {
                        msg = 'Requested JSON parse failed.';
                    } else if (errorThrown === 'timeout') {
                        msg = 'Time out error.';
                    } else if (errorThrown === 'abort') {
                        msg = 'Ajax request aborted.';
                    } else {
                        msg = 'Uncaught Error.\n' + jqXhr.responseText;
                    }
                    console.log(msg);
                }
            });
        } else {
            $('#backCameraIcModal').modal('hide');
            alertify.confirm("please enter IC No/Size", function () { $('#backCameraIcModal').modal('show'); });
        }
        
    });
}