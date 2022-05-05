function confirmSave(saveButtonObj) {
    event.preventDefault();
    console.log(saveButtonObj.innerText);
    bootbox.confirm("Please confirm if you want to create this task?", function (result) {
        if (result) {
            $(saveButtonObj).closest("form").submit();
            return true;
        }
    });
}
function ValidateFile(fileObj) {
    if (fileObj === null) {
        return false;
    }
    var mUploadID = '#' + fileObj.id;//vControlClassName.replace('.', '#');
    //alert($("#file").val());
    if ($(mUploadID).val() == "") {
        bootbox.alert("<div class='text-danger'>Please select file to upload</div>");
        return false;
    }

    var mFile = $(mUploadID)[0].files[0];
    //alert(mFile);
    var mFileName = mFile.name;
    //alert(mFileName);
    var _validFileExtensions = ["pdf", "xls", "xlsx", "jpg", "png", "gif", "jpeg", "docx", "doc", "txt", "msg", "sql", "zip"]; //["xls", "xlsx", "pdf", "docx", "doc", "txt"];

    var fileType = $(mUploadID).val().split('\\').pop();
    //alert(fileType);
    var arr1 = new Array;
    arr1 = fileType.split("\\");
    var len = arr1.length;
    var img1 = arr1[len - 1];
    var filext = fileType.substring(fileType.lastIndexOf(".") + 1);

    var uploadedFile = document.getElementById(fileObj.id);//vControlClassName.replace('.', '')
    var fileSize = uploadedFile.files[0].size;

    var blnValid = false;
    for (var j = 0; j < _validFileExtensions.length; j++) {
        var sCurExtension = _validFileExtensions[j];
        if (_validFileExtensions[j] == filext.toLowerCase()) {
            blnValid = true;
            break;
        }
    }

    if (blnValid == false) {
        bootbox.alert("<div class='text-danger'>Invalid File !! Please select valid file format.('pdf', 'xls', 'xlsx', 'jpg', 'png', 'gif', 'jpeg', 'docx', 'doc', 'txt', 'msg', 'sql', 'zip')</div>");
        $(mUploadID).val('');
        return false;
    }

    if (fileSize > 2048576 && filext.toLowerCase != 'zip') {//FILE SIZE!! If File size is less than 4194304 it showes invalid file it means the file is required above 4194308
        bootbox.alert("<div class='text-danger'>Invalid File!! File size cannot be greater than 2 MB. </div>");
        $(mUploadID).val('');
        return false;
    }
    if (fileSize > 6291456 && filext.toLowerCase == 'zip') {
        bootbox.alert("<div class='text-danger'>Invalid File!! File size cannot be greater than 6 MB. </div>");
        $(mUploadID).val('');
        return false;
    }

    return mFileName;
}

/*function CommentSection(){
//function OnAttachmentChange(vObj, page, section, Field) {
//    //debugger;
//    try {
//        var elemID = $(vObj).attr('id');
//        if ($(vObj).val() != '') {
//            AddAttachment(elemID, page, section, Field);

//            $(vObj).val('');
//        }
//    }
//    catch (e) {
//        alert(e.message);
//    }
//}

//function AddAttachment(elemId, page, section, Field) {
//    //debugger;
//    try {
//        var mUploadID = '#' + elemId;//vControlClassName.replace('.', '#');
//        //alert($("#file").val());
//        if ($(mUploadID).val() == "") {
//            bootbox.alert("<div class='text-danger'>Please select file to upload</div>");
//            return false;
//        }

//        var mFile = $(mUploadID)[0].files[0];
//        //alert(mFile);
//        var mFileName = mFile.name;
//        //alert(mFileName);
//        var _validFileExtensions = ["pdf"]; //["xls", "xlsx", "pdf", "docx", "doc", "txt"];

//        var fileType = $(mUploadID).val().split('\\').pop();
//        //alert(fileType);
//        var arr1 = new Array;
//        arr1 = fileType.split("\\");
//        var len = arr1.length;
//        var img1 = arr1[len - 1];
//        var filext = fileType.substring(fileType.lastIndexOf(".") + 1);

//        var uploadedFile = document.getElementById(elemId);//vControlClassName.replace('.', '')
//        var fileSize = uploadedFile.files[0].size;

//        var blnValid = false;
//        for (var j = 0; j < _validFileExtensions.length; j++) {
//            var sCurExtension = _validFileExtensions[j];
//            if (_validFileExtensions[j] == filext.toLowerCase()) {
//                blnValid = true;
//                break;
//            }
//        }

//        if (blnValid == false) {
//            bootbox.alert("<div class='text-danger'>Invalid Input !! Please select valid file format.('pdf')</div>");
//            return false;
//        }

//        if (fileSize > 1048576) {//FILE SIZE!! If File size is less than 4194304 it showes invalid file it means the file is required above 4194308
//            bootbox.alert("<div class='text-danger'>Invalid Input!! File size cannot be greater than 1 MB. </div>");
//            return false;
//        }

//        //var formData = new FormData($('form').get(0));
//        var formData = new FormData();
//        formData.append('file', mFile);
//        var AttachFieldDetails = "[{Page_Syscode : " + page + ", Section_Syscode : " + section + ", Field_Syscode : " + Field + ", SectionRow_PK : '', RowId : 0, PEDetails_Syscode : " + $('#hidd_PEDetails_Syscode').val() + "}]"; //
//        formData.append('AttachFieldDetails', AttachFieldDetails);
//        //alert(formData);
//        //var vJSONType = 'CancelledCheque';

//        ////Multiple Upload Test
//        //var files = $(mUploadID).get(0).files;
//        //formData = new FormData();
//        //for (var i = 0; i < files.length; i++) {
//        //    formData.append("fileInput", files[i]);
//        //}
//        ////Multiple Upload Test
//        //$(mUploadID).val('');

//        var saveType = "Mongo";
//        $.ajax({
//            url: _RootURL + 'FileService/UploadFiles/',
//            type: 'POST',
//            xhr: function () {
//                var myXhr = $.ajaxSettings.xhr();
//                if (myXhr.upload) {
//                    myXhr.upload.addEventListener('progress',
//                    {}, false);
//                }
//                return myXhr;
//            },
//            data: formData,
//            cache: false,
//            contentType: false,
//            processData: false,
//            success: function (response) {
//                //debugger;
//                var mIsSuccess = response.IsSuccess;
//                if (mIsSuccess == 'Y') {
//                    //var mFileContent = response.FileContent;
//                    var sysCode = response.SysCode_Temp;
//                    var mFileName = response.FileName;
//                    var MFileID = response.MFileID;
//                    var mOriginalFileName = response.OriginalFileName;
//                    //alert(mFileContent);
//                    //$("#hidd_ChequeContent").val(mFileContent);
//                    $("#hidd_ESICDisabRecType").val('N');
//                    //$("#txt_ChequeContent").val(mFileContent);
//                    $('#hidd_ESIC_Attach_Syscode').val(sysCode);
//                    $('#hidd_ESIC_Attach_FileID').val(MFileID);
//                    $('#hidd_ESIC_Attach_OrigFileName').val(mOriginalFileName);
//                    //$("#hidd_ChequeFileName").val(mFileName);
//                    //$("#hidd_ChequeOriginalFileName").val(mOriginalFileName);
//                    $("#divAttachment").css("display", "block");
//                    $("#ESICdownloadText").text(mOriginalFileName); //To be set later on...
//                }
//                else if (mIsSuccess == "SessionExpired") {
//                    $("#dialogSessionExpired").modal();
//                }
//                else {
//                    ShowError(response, "dvESICError");
//                }
//            },
//            error: function (response) {
//                alert(response.responseText);
//            },
//            failure: function (response) {
//                alert(response.responseText);
//            },
//        });
//    }
//    catch (e) {
//        alert(e.message);
//    }
//}

//function RemoveAttachment() {
//    //$("#hidd_ChequeContent").val('');
//    //$("#hidd_ChequeFileName").val('');
//    //$("#hidd_ChequeOriginalFileName").val('');
//    $('#hidd_ESIC_Attach_OrigFileName').val('');
//    $("#ESICdownloadText").text('');
//    $("#divAttachment").css("display", "none");
//    //$(".filesUploadCheque").val('');
//}

//function DownLoadAttachment(RecSyscode, FileID, FileName) {
//    try {        
//        var hidd_ESICDisabRecType = $('#hidd_ESICDisabRecType').val();
//        //alert(hidd_ESICDisabRecType);
//        //if (typeof hidd_ESICDisabRecType != 'undefined' && hidd_ESICDisabRecType != 'N') {
//            $.ajax(
//                 {
//                     url: _RootURL + 'FileService/DownloadFiles?RecSyscode=' + RecSyscode + '&MFileId=' + FileID + '&FileName=' + FileName,
//                     contentType: 'application/json; charset=utf-8',
//                     datatype: 'json',
//                     data: {
//                         syscode: RecSyscode
//                     },
//                     type: "GET",
//                     cache: false,
//                     success: function () {
//                         //debugger;
//                         window.location = _RootURL + 'FileService/DownloadFiles?RecSyscode=' + RecSyscode + '&MFileId=' + FileID + '&FileName=' + FileName;
//                     }
//                 });
//       // }

//    }
//    catch (e)
//    { alert(e.message); }
//}
}*/