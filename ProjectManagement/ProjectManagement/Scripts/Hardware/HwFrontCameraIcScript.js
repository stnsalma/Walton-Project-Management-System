function hwFrontCameraIc(frontCameraJsonObj) {
    
    //-------------------------------------------------\
    $('#HwFrontCameraIcModel_FrontCameraIcId').val($('.hdnFrontCameraSelectedValue').val());
    $('#HwFrontCameraIcModel_FrontCameraIcId').chosen();
    

    $('#HwFrontCameraIcModel_FrontCameraIcId').on('change', function () {
        //Passing value to PcbAModel
        var selectedIcNoSize = $('#HwFrontCameraIcModel_FrontCameraIcId option:selected').text();
        $('#HwTestCameraInfoModel_FrontCamera_IcNoSize').val(selectedIcNoSize);
        //---------------------
        console.log(frontCameraJsonObj);
        for (var i in frontCameraJsonObj) {
            if ($('#HwFrontCameraIcModel_FrontCameraIcId option:selected').val() == frontCameraJsonObj[i].FrontCameraIcId) {
                var id = frontCameraJsonObj[i].FrontCameraIcId;
                //var pmu1IcNoSize = frontCameraJsonObj[i].IcNoSize;
                var frontCameraVendor = frontCameraJsonObj[i].FrontCamera_Vendor;
                var pinNumber = frontCameraJsonObj[i].PinNumber;
                var pinType = frontCameraJsonObj[i].PinType;
                console.log(id);
                $('#HwTestCameraInfoModel_FrontCamera_Vendor').val(frontCameraVendor);
                $('#HwTestCameraInfoModel_FrontCamera_PinNumber').val(pinNumber);
                $('#HwTestCameraInfoModel_FrontCamera_PinType').val(pinType);
            }
        }
    });

    $('#btnSaveFrontCameraIc').on('click', function (e) {

        e.preventDefault();

        var obj = {};
        obj.HwQcAssignId = $('#hdnHwQcAssignId').val();
        obj.IcNoSize = $.trim($('#HwFrontCameraIcModel_IcNoSize').val());
        obj.Vendor = $('#HwFrontCameraIcModel_FrontCameraVendor').val();
        obj.PinType = $('#HwFrontCameraIcModel_PinType').val();
        obj.PinNumber = $('#HwFrontCameraIcModel_PinNumber').val();
        obj.Remarks = $('#HwFrontCameraIcModel_Remarks').val();

        if (obj.IcNoSize != "") {
            var url = 'PostHwFrontCameraIc';

            $.ajax({
                url: url,
                type: 'POST',
                data: obj,
                async: false,
                success: function(response) {
                    console.log(response);
                    $('#frontCameraIcModal').modal('hide');
                    alertify.alert("Front Camera IC saved");
                    var test = JSON.parse(response);
                    if (test != null) {
                        var newOption = $('<option value="' + test.FrontCameraIcId + '">' + test.IcNoSize + '</option>');
                        $('#HwFrontCameraIcModel_FrontCameraIcId').append(newOption);
                        $('#HwFrontCameraIcModel_FrontCameraIcId').val(test.FrontCameraIcId);
                        $('#HwTestCameraInfoModel_FrontCamera_IcNoSize').val(test.IcNoSize);
                        $("#HwFrontCameraIcModel_FrontCameraIcId").trigger("chosen:updated");
                        //$('#txtChipset_Core').val(test.ChipsetCore);
                        //$('#txtChipset_Speed').val(test.ChipsetSpeed);

                        var pushObj = { FrontCameraIcId: test.FrontCameraIcId, IcNoSize: test.IcNoSize };
                        frontCameraJsonObj.push(pushObj);
                        console.log(frontCameraJsonObj);
                        $('#HwFrontCameraIcModel_IcNoSize').val("");
                    } else {
                        alertify.alert("This Front camera IC already exists");
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
            $('#frontCameraIcModal').modal('hide');
            alertify.confirm("please enter IC No/Size", function () { $('#frontCameraIcModal').modal('show'); });
        }
        
    });
}