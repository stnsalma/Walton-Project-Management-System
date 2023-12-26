/// <reference path="jquery-1.12.3.js" />


    function refresh() {
        $.blockUI({ message: '<h1><img src="/img/loading-spinner-grey.gif" /> LOADING..</h1>' });
        setTimeout(function () {
            window.location.reload();

        }, 1000);
    }
 ////Total quantity check///////
   function batterySmtTotalQuantityCheck() {
       var materialReceiveStartDateSmt = $('#materialReceiveStartDateSmt').val();

       if (materialReceiveStartDateSmt == "") {
            alertify.dialog('alert').set({
                'title': '   ',
                'transition': 'zoom',
                'message': "Please must select SMT Mass Production Date 1st.",
            }).show();
            $('#batterySmt_TotalQuantity').val("");
            return false;

        }

    }
    function batteryHousingTotalQuantityCheck() {
        var materialReceiveStartDateHousing = $('#materialReceiveStartDateHousing').val();

        if (materialReceiveStartDateHousing == "") {
            alertify.dialog('alert').set({
                'title': '   ',
                'transition': 'zoom',
                'message': "Please must select Housing Mass Production Date 1st.",
            }).show();
            $('#batteryHousing_TotalQuantity').val("");
            return false;
        }

    }
    
    function batteryMassTotalQuantityCheck() {
        var batteryMassProductionStartDate = $('#batteryMassProductionStartDate').val();

        if (batteryMassProductionStartDate == "") {
            alertify.dialog('alert').set({
                'title': '   ',
                'transition': 'zoom',
                'message': "Please must select Battery Mass Production Date 1st.",
            }).show();
            $('#battery_TotalQuantity').val("");
            return false;
        }

    }
    function batteryAssemblyTotalQuantityCheck() {
        var assembStartDateBAssembly = $('#assembStartDateBAssembly').val();

        if (assembStartDateBAssembly == "") {
            alertify.dialog('alert').set({
                'title': '   ',
                'transition': 'zoom',
                'message': "Please must select Assembly Production Date 1st.",
            }).show();
            $('#batteryAssembly_TotalQuantity').val("");
            return false;
        }

    }
           
    function batteryPackingTotalQuantityCheck() {
        var packingMassProductionStartDateBAssembly = $('#packingMassProductionStartDateBAssembly').val();

        if (packingMassProductionStartDateBAssembly == "") {
            alertify.dialog('alert').set({
                'title': '   ',
                'transition': 'zoom',
                'message': "Please must select Packing Production Date 1st.",
            }).show();
            $('#batteryPacking_TotalQuantity').val("");
            return false;
        }

    }
    function isFloat(n) {
        return Number(n) === n && n % 1 !== 0;
    }