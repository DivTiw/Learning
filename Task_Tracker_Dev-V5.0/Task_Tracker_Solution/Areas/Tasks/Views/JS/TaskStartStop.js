var t;
var userrec;
//var minVer = false;
var HMS = { h: 0, m: 0, s: 0 };

function CheckTimeNull(_DateTime) {
    if (new Date(_DateTime) < new Date(2019, 0, 1)) {//'1/1/0001 12:00:00 AM'
        return "";
    } else {
        return _DateTime;
    }
}

function setHRS(date1, date2) {
    if (($.type(date1) === "date" || $.type(date1) === "string") && ($.type(date2) === "date" || $.type(date2) === "string")) {
        var strtTime = new Date(Date.parse(date1));
        var stpTime = $.type(date2) === "date" ? date2 : new Date(Date.parse(date2));
        var DTDiff = datediff(strtTime, stpTime);
        HMS.h = DTDiff.h;
        HMS.m = DTDiff.m;
        HMS.s = DTDiff.s;
    } else {
        return;
    }
}

function datediff(dt1, dt2) {
    if ($.type(dt1) !== "date" && $.type(dt2) !== "date")
        return;

    var milSec = Math.abs(dt2 - dt1);

    var hr = Math.floor(milSec / (1000 * 60 * 60));
    var hrMod = milSec % (1000 * 60 * 60);
    var min = Math.floor(hrMod / (1000 * 60));
    var minMod = hrMod % (1000 * 60);
    var sec = Math.floor(minMod / 1000);

    return { h: hr, m: min, s: sec };
}

function setElapsedTime() {
    var h = makeDoubleDigit(HMS.h);
    var m = makeDoubleDigit(HMS.m);
    var s = makeDoubleDigit(HMS.s);
    document.getElementById('txtClock').innerHTML = "Elapsed Time:" + "<br />" + h + ":" + m + ":" + s;
}

function startTime(isFirstCall) {
    if ($.type(isFirstCall) === "undefined") {
        HMS.s = HMS.s + 1;
        if (HMS.s > 60) {
            HMS.s = 0;
            HMS.m = HMS.m + 1
            if (HMS.m > 60) {
                HMS.m = 0;
                HMS.h = HMS.h + 1;
            }
        }
    }
    setElapsedTime();
    t = setTimeout(startTime, 1000);
}
function makeDoubleDigit(i) {
    if (i < 10) { i = "0" + i };
    return i;
}

function startClock() {
    HMS.h = 0;
    HMS.m = 0;
    HMS.s = 0;
    startTime(true);
}

function stopClock() {
    clearTimeout(t);
}

function ToggleStartStop(EnableStart) {
    if (EnableStart) {
        $("#start").prop('disabled', false);    //Start is enabled
        $("#stop").prop('disabled', true);
    } else {
        $("#start").prop('disabled', true);     //Start is disabled
        $("#stop").prop('disabled', false);
    }
}

function DisableStartStop(disable) {
    $("#start").prop('disabled', disable);
    $("#stop").prop('disabled', disable);
}

function SaveTaskTime(obj, _taskSyscode) {
    var strAction = $(obj).attr('name'); // start/stop;
    var ConfirmMsg = "";
    var startedTaskRefNo = $("#hidStartedTaskRefNo").val();
    if (strAction == "start") {
        if ($.type(startedTaskRefNo) === "string" && startedTaskRefNo !== "")
            ConfirmMsg = "This will Stop Task with reference "+startedTaskRefNo+". Please confirm if you want to start this task?";        
        else
            ConfirmMsg = "Please confirm if you want to start this task?";
    } else {
        ConfirmMsg = "Please confirm if you want to stop this task?";
    }
    bootbox.confirm(ConfirmMsg, function (result) {
        if (result) {        
            var task_syscode;
            if ($.type(_taskSyscode) === "number") {
                task_syscode = _taskSyscode;
            } else {
                task_syscode = $("#hid_task_syscode").val();
            }
            task_syscode = $.base64.encode(task_syscode);

            var str = "{task_syscode:" + JSON.stringify(task_syscode) + ",action:" + JSON.stringify(strAction) + "}";
            var dataObject = "{jsonData: " + JSON.stringify(str) + "}";

            $.ajax({
                type: "POST",
                url: "/Task/SaveUserActivity",
                data: dataObject,
                contentType: "application/json; charset=utf-8",
                datatype: "json",
                success: function (response) {
                    //var mIsSuccess = response;

                    if (response.startsWith('Y')) {
                        var strTime = response.substring(1, response.length);
                        bootbox.alert("Task Activity Recorded on " + strTime);
                        setTimeout(function () { location.reload(); }, 2000);
                        //if (strAction == "start") {
                        //    startClock();
                        //} else {
                        //    stopClock();
                        //}
                        //var strTime = response.substring(1, response.length);
                        //bootbox.alert("Task Activity Recorded on " + strTime);
                        //if (obj.id.includes('start')) {
                        //    document.getElementById('txtSSTime').innerHTML = "<i>Started On - " + strTime + "</i>";
                        //    ToggleStartStop(false); //Disable Start and enable Stop.
                        //} else {
                        //    document.getElementById('txtSSTime').innerHTML = "<i>Stopped On - " + strTime + "</i>";
                        //    ToggleStartStop(true); //Enable Start and disable Stop
                        //}
                    }
                    else if (response == "SessionExpired") {
                        $("#dialogSessionExpired").modal();
                    }
                    else {
                        //if (response.includes('already started')) {
                        //    ToggleStartStop(false); //Disable Start and enable Stop.
                        //} else if (response.includes('Open Task Record not found')) {
                        //    ToggleStartStop(true); //Enable Start and disable Stop
                        //}
                        bootbox.alert(response);
                        setTimeout(function () { location.reload(); }, 2000);
                    }
                },
                error: function (e) {
                    alert("Dynamic content load failed. " + e);
                }
            });
            //$(obj).removeAttr("disabled");  
        }
    });
}