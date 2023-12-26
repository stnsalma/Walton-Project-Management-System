function hwRfIc(rfIcJsonObj) {
    $('#ddlRfic').val($('.hdnRfSelectedValue').val());
    $('#ddlRfic').chosen();

    


    //=========Chipset dropdown on select change actions============
    $('#ddlRfic').on('change', function () {
        //Passing value to PcbAModel
        var selectedIcNoSize = $('#ddlRfic option:selected').text();
        $('#hdnRfIcNoSize').val(selectedIcNoSize);
        //---------------------
        console.log(rfIcJsonObj);
      
    });


    //=========Chipset Save============
    $('#btnSaveRfIc').on('click', function (e) {

        e.preventDefault();
        //document.getElementById("btnSaveRfIc").disabled = true;
        var obj = {};
        obj.HwQcAssignId = $('#hdnHwQcAssignId').val();
        obj.IcNoSize = $.trim($('#HwRfModel_IcNoSize').val());
        obj.RfVendor = $('#HwRfModel_RfVendor').val();
        obj.PinType = $('#HwRfModel_PinType').val();
        obj.PinNumber = $('#HwRfModel_PinNumber').val();
        obj.Remarks = $('#HwRfModel_Remarks').val();

        if (obj.IcNoSize != "") {
            var url = 'PostRfIc';

            $.ajax({
                url: url,
                type: 'POST',
                data: obj,
                async: false,
                success: function (response) {
                    console.log(response);
                    $('#rfModal').modal('hide');

                    var test = JSON.parse(response);
                    if (test != null) {
                        var newOption = $('<option value="' + test.RfId + '">' + test.IcNoSize + '</option>');
                        $('#ddlRfic').append(newOption);
                        $('#ddlRfic').val(test.RfId);
                        $('#hdnRfIcNoSize').val(test.IcNoSize);
                        $("#ddlRfic").trigger("chosen:updated");

                        var pushObj = { RfId: test.RfId, IcNoSize: test.IcNoSize };
                        rfIcJsonObj.push(pushObj);
                        console.log(rfIcJsonObj);

                        //-----other extra operation
                        $('#HwRfModel_IcNoSize').val("");
                        alertify.alert("RFIC saved");
                    } else {
                        alertify.alert("This RFIC already exists");
                    }

                    //document.getElementById("btnSaveRfIc").disabled = false;

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
            $('#rfModal').modal('hide');
            alertify.confirm("please enter IC No/Size", function () { $('#rfModal').modal('show'); });
        }
        
    });
}

