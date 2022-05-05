function confirmSave(saveButtonObj) {
    event.preventDefault();
    console.log(saveButtonObj.innerText);
    var valCom = $("#txttask_subject").val();

    if (valCom == '' || valCom.length == 0) {
        bootbox.alert("Please enter Task Subject.");
        return false;
    }
    bootbox.confirm("Please confirm if you want to perform this action?", function (result) {
        if (result) {
            $(saveButtonObj).closest("form").submit();
            return true;
        }
    });
}
$(document).ready(function () {
   
    $(".ahrefViewRD").click(function () {        
        
        //alert(proj_syscode);
        var Json_RD = $("#hid_json").val();
        if (Json_RD) {            
            $("#dialogReleaseDetails").modal();
        }
        else{
            $.ajax({
                url: '/Project/ViewReleaseDetails',
                type: 'GET',
                data: {
                    "id": $.base64.encode(proj_syscode),
                    "projname": proj_name
                },
                success: function (res) {
                    $("#insideRDDialogTitle").html("Release Details- Task Reference: " + tskref);
                    $("#insideRDDialogContent").html(res);
                    $("#dialogReleaseDetails").modal();
                },
                error: function (xhr, status, error) {
                    var err = jQuery.parseJSON(xhr.responseText);
                    alert(err.Message);
                }
            });
        }

        return false;
    });

    $('.ChkPercent').change(function () {
        calculateTotalWtg();       
    });
});

function calculateTotalWtg() {
    var sum = 0;
    $('.ChkPercent').each(function () {
        sum += Number($(this).val());
    });

    //alert(sum);
    $("#lblTotalWtg").text(sum);
    if (sum > 100) {
        bootbox.alert("Sum of Percentage cannot be greater than 100");
    }

}