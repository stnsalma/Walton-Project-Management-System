function hwChipset(chipsetJsonObj) {
    $('#ddlChipset').val($('.hdnChipsetSelectedValue').val());
    $('#ddlChipset').chosen();

    //---------------------------------------------
    //var chipsetModel = '@Html.Raw(chipsetModelJson)';
    //var chipsetJsonObj = $.parseJSON(chipsetModel);
    //console.log(chipsetJsonObj);


    //=========Chipset dropdown on select change actions============
    $('#ddlChipset').on('change', function () {
        //Passing value to PcbAModel
        var selectedIcNoSize = $('#ddlChipset option:selected').text();
        $('#hdnIcNoSize').val(selectedIcNoSize);
        //---------------------
        console.log(chipsetJsonObj);
        for (var i in chipsetJsonObj) {
            if ($('#ddlChipset option:selected').val() == chipsetJsonObj[i].ChipsetId) {
                var id = chipsetJsonObj[i].ChipsetId;
                var vendor = chipsetJsonObj[i].ChipsetVendor;
                var core = chipsetJsonObj[i].ChipsetCore;
                var speed = chipsetJsonObj[i].ChipsetSpeed;
                var pintype = chipsetJsonObj[i].PinType;
                var pinnumber = chipsetJsonObj[i].PinNumber;
                var newitemno = chipsetJsonObj[i].NewItemNo;
                var itemcode = chipsetJsonObj[i].ItemCode;
                var remarks = chipsetJsonObj[i].Remarks;
                console.log(id);
                $('#txtChipset_Core').val(core);
                $('#txtChipset_Speed').val(speed);
                $('#hdnChipsetVendor').val(vendor);
                $('#HwTestPcbAModel_Chipset_PinNumber').val(pinnumber);
                $('#HwTestPcbAModel_Chipset_PinType').val(pintype);
                $('#HwTestPcbAModel_Chipset_Remark').val(remarks);
            }
        }
    });


    //=========Chipset Save============
    $('#btnSaveChipset').on('click', function (e) {

        e.preventDefault();

        var obj = {};
        obj.HwQcAssignId = $('#hdnHwQcAssignId').val();
        //assign value to obj from chipset modal
        obj.IcNoSize = $.trim($('#txtIcNoSize').val());
        obj.ChipsetVendor = $('#txtChipsetVendor').val();
        obj.PinType = $('#txtPinType').val();
        obj.ChipsetCore = $('#txtChipsetCore').val();
        obj.ChipsetSpeed = $('#txtChipsetSpeed').val();
        obj.PinNumber = $('#txtPinNumber').val();
        obj.NewItemNo = $('#HwChipsetModel_NewItemNo').val();
        obj.ItemCode = $('#HwChipsetModel_ItemCode').val();
        obj.Remarks = $('#txtRemarks').val();

        if (obj.IcNoSize != "") {
            var url = 'PostChipsetIc';

            $.ajax({
                url: url,
                type: 'POST',
                data: obj,
                async: false,
                success: function(response) {
                    console.log(response);
                    $('#chipsetModal').modal('hide');
                    alertify.alert("Chipset saved");
                    var test = JSON.parse(response);
                    if (test != null) {
                        //assign latest saved chipset data to respective pcba table column here
                        var newOption = $('<option value="' + test.ChipsetId + '">' + test.IcNoSize + '</option>');
                        $('#ddlChipset').append(newOption);
                        $('#ddlChipset').val(test.ChipsetId);
                        $('#hdnIcNoSize').val(test.IcNoSize);
                        $("#ddlChipset").trigger("chosen:updated");
                        $('#txtChipset_Core').val(test.ChipsetCore);
                        $('#txtChipset_Speed').val(test.ChipsetSpeed);
                        $('#HwTestPcbAModel_Chipset_PinNumber').val(test.PinNumber);
                        $('#HwTestPcbAModel_Chipset_PinType').val(test.PinType);
                        $('#HwTestPcbAModel_Chipset_Remark').val(test.Remarks);

                        var pushObj = { ChipsetId: test.ChipsetId, ChipsetCore: test.ChipsetCore, ChipsetSpeed: test.ChipsetSpeed, IcNoSize: test.IcNoSize };
                        chipsetJsonObj.push(pushObj);
                        //----other works
                        $('#txtIcNoSize').val("");
                        console.log(chipsetJsonObj);
                    } else {
                        alertify.alert("This Chipset IC already exists");
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
            $('#chipsetModal').modal('hide');
            alertify.confirm("please enter IC No/Size", function () { $('#chipsetModal').modal('show'); });
        }
    });
}

