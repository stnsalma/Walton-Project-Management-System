function hwFlashIc(flashIcJsonObj) {
    $('#ddlFlashIc').val($('.hdnFlashIcSelectedValue').val());
    $('#ddlFlashIc').chosen();
    //------------------------
    $('#ddlFlashIc').on('change', function () {
        //Passing value to PcbAModel
        var selectedFlashIcNoSize = $('#ddlFlashIc option:selected').text();
        $('#HwTestPcbAModel_Flash_IcNoSize').val(selectedFlashIcNoSize);
        console.log(selectedFlashIcNoSize);
        //----------------------------
        for (var i in flashIcJsonObj) {
            if ($('#ddlFlashIc option:selected').val() == flashIcJsonObj[i].FlashIcId) {
                var id = flashIcJsonObj[i].FlashIcId;
                var technology = flashIcJsonObj[i].FlashIcTechnology;
                var vendor = flashIcJsonObj[i].FlashIdVendor;
                var ram = flashIcJsonObj[i].FlashIcRam;
                var rom = flashIcJsonObj[i].FlashIcRom;
                var ball = flashIcJsonObj[i].FlashIcBall;
                var pinnumber = flashIcJsonObj[i].PinNumber;
                var pintype = flashIcJsonObj[i].PinType;
                var remark = flashIcJsonObj[i].Remarks;
                console.log(flashIcJsonObj[i]);
                $('#txtFlashIc_Ram').val(ram);
                $('#txtFlashIc_Rom').val(rom);
                $('#txtFlashIc_Technology').val(technology);
                $('#txtFlashIc_Ball').val(ball);
                $('#hdnFlashIc_Vendor').val(vendor);
                $('#HwTestPcbAModel_FlashIC_PinNumber').val(pinnumber);
                $('#HwTestPcbAModel_FlashIC_PinType').val(pintype);
                $('#HwTestPcbAModel_FlashIC_Remark').val(remark);
            }
        }
    });


    $('#btnSaveFlashIc').on('click', function (e) {
        e.preventDefault();

        var obj = {};
        obj.HwQcAssignId = $('#hdnHwQcAssignId').val();
        obj.IcNoSize = $.trim($('#txtFlashIcNoSize').val());
        obj.FlashIcVendor = $('#txtFlashIcVendor').val();
        obj.FlashIcRam = $('#txtFlashIcRam').val();
        obj.FlashIcRom = $('#txtFlashIcRom').val();
        obj.FlashIcBall = $('#txtFlashIcBallType').val();
        obj.FlashIcTechnology = $('#txtFlashIcTechnology').val();
        obj.PinType = $('#txtFlashIcPinType').val();
        obj.PinNumber = $('#txtFlashIcPinNumber').val();
        obj.Remarks = $('#txtFlashIcRemarks').val();
        console.log(obj);

        if (obj.IcNoSize != "") {

            var url = 'PostFlashIc';

            $.ajax({
                url: url,
                type: 'POST',
                data: obj,
                async: false,
                success: function(response) {
                    $('#flashIcModal').modal('hide');
                    alertify.alert("FlashIC saved");
                    var test = JSON.parse(response);
                    if (test != null) {
                        var newOption = $('<option value="' + test.FlashIcId + '">' + test.IcNoSize + '</option>');

                        $('#ddlFlashIc').append(newOption);
                        $('#ddlFlashIc').val(test.FlashIcId);
                        $("#ddlFlashIc").trigger("chosen:updated");
                        $('#HwTestPcbAModel_Flash_IcNoSize').val(test.IcNoSize);
                        $('#txtFlashIc_Technology').val(test.FlashIcTechnology);
                        $('#txtFlashIc_Ram').val(test.FlashIcRam);
                        $('#txtFlashIc_Rom').val(test.FlashIcRom);
                        $('#txtFlashIc_Ball').val(test.FlashIcBall);
                        $('#HwTestPcbAModel_FlashIC_PinNumber').val(test.PinNumber);
                        $('#HwTestPcbAModel_FlashIC_PinType').val(test.PinType);
                        $('#HwTestPcbAModel_FlashIC_Remark').val(test.Remarks);

                        var pushObjFlashIc = { FlashIcId: test.FlashIcId, FlashIdVendor: test.FlashIcVendor, FlashIcTechnology: test.FlashIcTechnology, IcNoSize: test.IcNoSize, FlashIcRam: test.FlashIcRam, FlashIcRom: test.FlashIcRom, FlashIcBall: test.FlashIcBall };
                        flashIcJsonObj.push(pushObjFlashIc);
                        //------other works
                        $('#txtFlashIcNoSize').val("");
                        console.log(flashIcJsonObj);
                    } else {
                        alertify.alert("This Flash IC already exists");
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
            $('#flashIcModal').modal('hide');
            alertify.confirm("please enter IC No/Size", function () { $('#flashIcModal').modal('show'); });
        }
        
    });
}