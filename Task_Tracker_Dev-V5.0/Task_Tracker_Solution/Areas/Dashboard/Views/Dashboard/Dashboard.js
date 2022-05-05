$(document).ready(function () {
    try {
    
        //prepareCharts();
        callUsers();

        //$('[data-toggle="datepicker"]').on('click', function (e) {
        //    e.preventDefault();
        //    $(this).attr("autocomplete", "off");
        //});
} catch (e) {
        alert(e.message);
}
});

function callProjects()
{
    $("#btnUser").removeClass("active");
    $("#btnProject").addClass("active");
    
    $("#tblUsers").hide();
    $("#tblProjects").show();

}

function callUsers()
{
    $("#btnProject").removeClass("active");
    $("#btnUser").addClass("active");
    
    $("#tblUsers").show();
    $("#tblProjects").hide();

}

//function filteredData()
//{
//    debugger;
//    var startDate = $("#txtstart_date").val();
//    var endDate = $("#txtend_date").val();
//    var data = { startdate: startDate, enddate: endDate };
//    var _RootURL = '@Url.Content("~/")';

//    $.ajax({
//        type: 'POST',
//        dataType: 'json',
//        url: '/GetAdminDashboardNew',
//        data: JSON.stringify(data),
//        success: function (data) {
//            var IsSuccess = data.IsSuccess;
//            var Msg = data.SuccessMsg;
//            if (IsSuccess == 'Y') {
//                //bootbox.alert("<div style='word-wrap: break-word;'> You Have Sucessfully Added Employee(s) </div>", function () {
//                //    window.location.href = window.location.href;
//                //});


//            }
//            else {
//                bootbox.alert("<div class='text text-danger'>Error occurred!!'" + Msg + "'</div>");
//            }
//        },
//        error: function (XMLHttpRequest, textStatus, errorThrown, data) {
//            alert(errorThrown);
//        },
//        failure: function (data) {
//            alert(data);
//        }
//    });
    
//}

function prepareCharts()
{
    
    var ctx = document.getElementById('pieData').getContext('2d');
    var chart = new Chart(ctx, {
        // The type of chart we want to create
        type: 'pie',

        // The data for our dataset
        data: {
            labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
            datasets: [{
                label: 'My First dataset',
                backgroundColor: 'rgb(255, 99, 132)',
                borderColor: 'rgb(255, 99, 132)',
                data: [0, 10, 5, 2, 20, 30, 45]
            }]
        },

        // Configuration options go here
        options: {
            "hover": {
                "animationDuration": 0
            },
            "animation": {
                "duration": 1
            }

        }
    });
}
