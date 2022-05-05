
var TrailObj = [], TrailActivity = {}//, TaskSyscode = 0;
//var emp_name = '@Request.RequestContext.HttpContext.Session["emp_name"]';



$(document).ready(function () {
   
});

function InitializeTrail(_TrailActivity)
{
    TrailActivity = _TrailActivity;
}

function CreateTrailJson(obj, activityDesc, activityCode, taskSyscode,createdby) {
    //alert(createdby);
    debugger;
    var comments = "";
    switch (activityCode) {
        case TrailActivity.Created+"":
            break;
        case TrailActivity.Acknowledged+"":
            break;
        case TrailActivity.Started+"":
            break;
        case TrailActivity.Updated+"":
            break;
        case TrailActivity.Created_For+"":
            break;
        case TrailActivity.Forwarded+"":
            break;
        case TrailActivity.Completed+"":
            break;
        case TrailActivity.Closed+"":
            break;
        case TrailActivity.Changed_Status+"":
            comments = "To " + obj.options[obj.selectedIndex].text;
            break;
        case TrailActivity.Added_Member+"":
            comments = $("#" + obj.id + " option:selected").toArray().map(item => item.text).join(", ");
            break;
        case TrailActivity.Added_File+"":
            var fileName = GetFileNames(obj);
            if (fileName == false) {
                return false;
            }
            comments = "File(s) : " + fileName;
            break;
        case TrailActivity.Added_Comments+"":
            comments = obj.value;
            break;
        case TrailActivity.Created_Subtask+"":
            break;
    }

    var curTrail = {
        task_syscode: taskSyscode,
        activity_syscode: activityCode,
        trail_start_datetime: returnJSONDate(),
        trail_description: createdby + ' ' + activityDesc + ' on ' + returnJSONDate(),
        trail_comments:comments,
        created_on: returnJSONDate()
    };
    var curTrailIndex;
    if (TrailObj.findTrail(activityCode, function (i) { curTrailIndex = i; })) {
        TrailObj[curTrailIndex] = curTrail;
    } else {
        TrailObj.push(curTrail);
    }
    $("#hid_trailJson").val(JSON.stringify(TrailObj));
}

Array.prototype.findTrail = function (_trailActivity, collector) {
    //debugger;
    for (var i = 0; i < this.length; i++) 
    {
        if (this[i].activity_syscode === _trailActivity)
        {
            if (collector !== 'undefined') {
                collector(i);
            }
            return true;
        }
    }
    return false;
};

function returnJSONDate() {
    var date = new Date();
    var day = date.getDate();       // yields date
    var month = date.getMonth() + 1;    // yields month (add one as '.getMonth()' is zero indexed)
    var year = date.getFullYear();  // yields year
    var hour = date.getHours();     // yields hours 
    var minute = date.getMinutes(); // yields minutes
    var second = date.getSeconds(); // yields seconds

    // After this construct a string with the above results as below
    var time = retDoubleDigitNum(day) + "/" + retDoubleDigitNum(month) + "/" + year + " " + retDoubleDigitNum(hour) + ':' + retDoubleDigitNum(minute) + ':' + retDoubleDigitNum(second);
    return time;
}

function retDoubleDigitNum(digit)
{
    return ((digit + "").length == 1 ? "0" + digit : digit)
}

