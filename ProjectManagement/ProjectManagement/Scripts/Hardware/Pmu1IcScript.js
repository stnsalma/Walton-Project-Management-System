function hwPmu1Icpmu1Ic(pmu1JsonObj) {
    $('#HwPmu1IcModel_Pmu_1_Id').val($('.hdnPmu1SelectedValue').val());
    $('#HwPmu1IcModel_Pmu_1_Id').chosen();
    
    $('#HwPmu1IcModel_Pmu_1_Id').on('change', function () {
        //Passing value to PcbAModel
        var selectedIcNoSize = $('#HwPmu1IcModel_Pmu_1_Id option:selected').text();
        $('#HwTestPcbAModel_PMU1IC').val(selectedIcNoSize);
        //---------------------
        console.log(pmu1JsonObj);
        for (var i in pmu1JsonObj) {
            if ($('#HwPmu1IcModel_Pmu_1_Id option:selected').val() == pmu1JsonObj[i].Pmu_1_Id) {
                var id = pmu1JsonObj[i].Pmu_1_Id;
                //var pmu1IcNoSize = pmu1JsonObj[i].IcNoSize;
                var pmu1Vendor = pmu1JsonObj[i].Pmu_1_Vendor;
                var pinNumber = pmu1JsonObj[i].PinNumber;
                var pinType = pmu1JsonObj[i].PinType;
                var newitemno = pmu1JsonObj[i].NewItemNo;
                var itemcode = pmu1JsonObj[i].ItemCode;
                var remark = pmu1JsonObj[i].Remarks;
                console.log(id);
                $('#HwTestPcbAModel_PMU1IC_Vendor').val(pmu1Vendor);
                $('#HwTestPcbAModel_PMU1IC_PinNumber').val(pinNumber);
                $('#HwTestPcbAModel_PMU1IC_PinType').val(pinType);
                $('#HwTestPcbAModel_PMU1IC_Remark').val(remark);
            }
        }
    });
    
    $('#btnSavePmu1Ic').on('click', function (e) {

        e.preventDefault();

        var obj = {};
        obj.HwQcAssignId = $('#hdnHwQcAssignId').val();
        obj.IcNoSize = $.trim($('#HwPmu1IcModel_IcNoSize').val());
        obj.Pmu_1_Vendor = $('#HwPmu1IcModel_Pmu_1_Vendor').val();
        obj.PinType = $('#HwPmu1IcModel_PinType').val();
        obj.PinNumber = $('#HwPmu1IcModel_PinNumber').val();
        obj.NewItemNo = $('#HwPmu1IcModel_NewItemNo').val();
        obj.ItemCode = $('#HwPmu1IcModel_ItemCode').val();
        obj.Remarks = $('#HwPmu1IcModel_Remarks').val();

        if (obj.IcNoSize != "") {
            var url = 'PostPmu1Ic';

            $.ajax({
                url: url,
                type: 'POST',
                data: obj,
                async: false,
                success: function (response) {
                    console.log(response);
                    $('#pmu1Modal').modal('hide');
                    alertify.alert("pmu1 saved");
                    var test = JSON.parse(response);
                    if (test != null) {
                        var newOption = $('<option value="' + test.Pmu_1_Id + '">' + test.IcNoSize + '</option>');
                        $('#HwPmu1IcModel_Pmu_1_Id').append(newOption);
                        $('#HwPmu1IcModel_Pmu_1_Id').val(test.Pmu_1_Id);
                        $('#HwTestPcbAModel_PMU1IC').val(test.IcNoSize);
                        $("#HwPmu1IcModel_Pmu_1_Id").trigger("chosen:updated");
                        $('#HwTestPcbAModel_PMU1IC_Vendor').val(test.Pmu_1_Vendor);
                        $('#HwTestPcbAModel_PMU1IC_PinNumber').val(test.PinNumber);
                        $('#HwTestPcbAModel_PMU1IC_PinType').val(test.PinType);
                        $('#HwTestPcbAModel_PMU1IC_Remark').val(test.Remarks);

                        var pushObj = { Pmu_1_Id: test.Pmu_1_Id, IcNoSize: test.IcNoSize };
                        pmu1JsonObj.push(pushObj);
                        console.log(pmu1JsonObj);
                    } else {
                        alertify.alert("This PMU1 IC already exists");
                    }

                },
                error: function (jqXhr, textStatus, errorThrown) {
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
            $('#pmu1Modal').modal('hide');
            alertify.confirm("please enter IC No/Size", function () { $('#pmu1Modal').modal('show'); });
        }
        
    });
}
