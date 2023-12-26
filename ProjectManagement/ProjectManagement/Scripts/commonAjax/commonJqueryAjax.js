
function commonJqueryAjax(myUrl, myData) {
    $.extend({
        xResponse: function (url, data) {
            
            // local var
            var theResponse = null;
            // jQuery ajax
            $.ajax({
                url: url,
                type: 'POST',
                data: JSON.stringify(data),
                dataType: "json",
                async: false,
                success: function (respText) {
                    theResponse = respText;
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
            console.log("ajax function ----");
            console.log(data);
            // Return the response text
            return theResponse;
        }
    });

    // set ajax response in var
    var xData = $.xResponse(myUrl, myData);
    return xData;
}