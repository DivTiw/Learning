var WorkflowLevelJson;
$(document).ready(function () {
    var hidWFLevelList = $("#hid_WFLevelList").val();

    if (hidWFLevelList === "" || hidWFLevelList === "[]" || hidWFLevelList === null) {
        WorkflowLevelJson = [{
            row_Id: "1",
            workflow_syscode: "0",
            level_syscode: "0",
            level_name: "",
            level_order: "0",
            is_active: "true",
            is_deleted: "false"
        }];
    } else {
        WorkflowLevelJson = JSON.parse(hidWFLevelList);
    }
});

function AddRow() {
    var rowCount = 0;
    rowCount = WorkflowLevelJson.length + 1;
    WorkflowLevelJson.push({
        row_Id: rowCount,
        workflow_syscode: "0",
        level_syscode: "0",
        level_name: "",
        level_order: "0",
        is_active: "true",
        is_deleted: "false"
    });

    $('#StepsTable').append(returnWFTableRow(rowCount));
}

function returnWFTableRow(rowCount) {
    var colCounter = 0;
    var strIdPart = "WFLevelTableRow" + rowCount + "_Col";
    var strTableRow = '<tr id="WFLevelTableRow_"' + rowCount + '>' +
                            '<td id="' + strIdPart + colCounter++ + '" class="text-center">' + rowCount + '</td>' +
                            '<td id="' + strIdPart + ++colCounter + '"><input id="' + strIdPart + colCounter + '_level_name" class="form-control" type="text" /></td>' +
                            '<td id="' + strIdPart + ++colCounter + '"><input id="' + strIdPart + colCounter + '_level_order" class="form-control" type="text" /></td>' +
                            '<td id="' + strIdPart + ++colCounter + '" class="text-center"><input id="' + strIdPart + colCounter + '_is_active" type="checkbox" checked="checked" /></td>' +
                            '<td id="' + strIdPart + ++colCounter + '" class="text-center">' +
                                '<button id="' + strIdPart + colCounter + '_btnAdd" type="button" class="border-0 btn-transition btn btn-outline-primary" onclick="AddRow();"><i class="fa fa-plus"></i></button>' +
                                '<input id="' + strIdPart + colCounter + '_level_syscode" name="' + strIdPart + colCounter + '_level_syscode" type="hidden" value="0">' +
                             '</td>' +
                       '</tr>';
    return strTableRow;
}

function Save(obj) {

    SaveWorkflowName(obj);

    var SaveWorkflowJson = [];

    var intWorkflowSyscode = $("#hid_workflow_syscode").val();
    if (intWorkflowSyscode == 0) {
        intWorkflowSyscode = $("#hid_workflow_syscode_BKUP").val();
        $("#hid_workflow_syscode").val(intWorkflowSyscode);
    }

    for (var i = 1; i <= WorkflowLevelJson.length; i++) {
        //i = i + 1;
        SaveWorkflowJson.push({
            row_Id: i,
            workflow_syscode: intWorkflowSyscode,
            level_syscode: $("#WFLevelTableRow" + i + "_Col5_level_syscode").val(),
            level_name: $("#WFLevelTableRow" + i + "_Col2_level_name").val(),
            level_order: $("#WFLevelTableRow" + i + "_Col3_level_order").val() == "" ? 0 : $("#WFLevelTableRow" + i + "_Col3_level_order").val(),
            is_active: $("#WFLevelTableRow" + i + "_Col4_is_active").is(":checked"),
            is_deleted: "false"
        });
    }

    $("#hid_WFLevelList").val(JSON.stringify(SaveWorkflowJson));
}

function SaveWorkflowName(obj) {
    event.preventDefault();
    if ($('#txtworkflow_name').val().length == 0) {
        bootbox.alert("Please enter Workflow name");
        return;
    }
    return Confirm(obj);
}

function Confirm(obj) {
    bootbox.confirm("Please confirm if you want to perform this action?", function (result) {
        if (result) {
            $('#' + obj.id).click(); //btnSaveWorkflow
            return true;
        }
    });
}