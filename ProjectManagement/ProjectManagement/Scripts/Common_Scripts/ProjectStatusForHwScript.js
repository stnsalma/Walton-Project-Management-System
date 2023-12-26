function getProjectStatusForHw(projectId, url) {

    var obj = {};
    obj.projectMasterId = projectId;


    var dateTimeReviver = function (key, value) {
        var a;
        if (typeof value === 'string') {
            a = /\/Date\((\d*)\)\//.exec(value);
            if (a) {
                return new Date(+a[1]);
            }
        }
        return value;
    };


    //var url = '../Common/ProjectStatusForHw';
    $.ajax({
        url: url,
        type: 'POST',
        data: obj,
        async: false,
        success: function (data) {
            console.log(data);
            var projectstatHw = JSON.parse(data, dateTimeReviver);
            console.log("screening sent date:" + projectstatHw.ScreeningSampleSetSentDate.getFullYear());
            //----------------------------------------------------------------------------------------
            var timeline = [
    {
        time: projectstatHw.ScreeningSampleSetSentDate.getFullYear()+'-' + projectstatHw.ScreeningSampleSetSentDate.getMonth()+ '-' + projectstatHw.ScreeningSampleSetSentDate.getDate(),
        body: [{
            tag: 'h1',
            content: "Sample Sent for Screening Test from Commercial"
        },
		{
		    tag: 'p',
		    content: ''
		}]
    },
	{
	    time: projectstatHw.ScreeningSampleSetReceiveDate.getFullYear() + '-' + projectstatHw.ScreeningSampleSetReceiveDate.getMonth() + '-' + projectstatHw.ScreeningSampleSetReceiveDate.getDate(),
	    body: [{
	        tag: 'h1',
	        content: "Sample Received by Hardware for Screening Test"
	    },
		{
		    tag: 'p',
		    content: ''
		}]
	}
            ];
            
            $.fn.albeTimeline.languages = {
                "es-ES": {
                    days: ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"],
                    months: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
                    shortMonths: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"],
                    msgEmptyContent: "No hay información para mostrar."
                },
                "en-US": {
                    days: ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"],
                    months: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],
                    shortMonths: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
                    msgEmptyContent: "No information to display."
                },
                "fr-FR": {
                    days: ["Dimanche", "Lundi", "Mardi", "Mercredi", "Jeudi", "Vendredi", "Samedi"],
                    months: ["Janvier", "Février", "Mars", "Avril", "Mai", "Juin", "Juillet", "Août", "Septembre", "Octobre", "Novembre", "Décembre"],
                    shortMonths: ["Jan", "Fév", "Mar", "Avr", "Mai", "Juin", "Juil", "Août", "Sep", "Oct", "Nov", "Déc"],
                    msgEmptyContent: "Aucune information à afficher."
                }
            };

            //Internationalization
            $("#myTimeline").albeTimeline(timeline, {
                language: "en-US",  //default: pt-BR
                formatDate: 4		//default: 1
            });
            //--------------------------------------------------------------------------------------

            //alertify.alert(data.projectMasterId);
        }

    });
}