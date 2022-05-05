--Other Updates
insert into email_template (template_syscode,template_name, template_subject,from_email_display,from_email_id,is_active,link_url,created_on,created_by,template_body)
values(5,'Other_Updates','Task Updated (#wf_level#)','Stride Build','StrideBuild@jmfl.com',1,'http://localhost:57101?returnValue=Tasks/Task/ViewTask/',getdate(),3986,
'<!DOCTYPE html>
<html lang="en">

<head>
    <style>
        body {
            font-family: Verdana;
        }

        .subject {
            font-size: 13px;
        }

        .email-body {
            text-align: justify;
            font-size: 12px;
        }

            .email-body p {
                line-height: 25px;
                margin-bottom: 7px;
            }

        .email-attachments {
            padding: 17px 0px;
            border-top: 1px solid #e6e5e5;
        }

        table th, td {
            text-align: left;
            font-size: 12px;
        }
    </style>
</head>
<body>
    <div class="main-content container-fluid p-0">
        <div class="email-body">
            <p>Hi,</p>
            <div class="subject">
                <p>The following #wf_level# task has been modified by #emp_name#</p> <br /><br />
                <p>
                    <table border="1" cellspacing="0" cellpadding="6">
                        <tbody>
                            <tr>
                                <td>Project</td>
                                <td>#project#</td>
                            </tr>
                            <tr>
                                <td>Category</td>
                                <td>#category#</td>
                            </tr>
                            <tr>
                                <td>Module</td>
                                <td>#module#</td>
                            </tr>
                            <tr>
                                <td>Task Subject</td>
                                <td>#subject#</td>
                            </tr>
                            <tr>
                                <td>Status</td>
                                <td>#status#</td>
                            </tr>
                            <tr>
                                <td>Workflow level/Step</td>
                                <td>#wf_level#</td>
                            </tr>
                        </tbody>
                    </table>
                </p>
                <br />
                <p><a href="#url#">Click Here</a> to know more.</p>                  <br /><br />
                <p>-This is a system generated mail. Please do not reply to this mail.</p>
            </div>
        </div>
    </div>
</body>
</html>'
)
GO

--Task Initiated
insert into email_template (template_syscode,template_name, template_subject,from_email_display,from_email_id,is_active,link_url,created_on,created_by,template_body)
values(6,'Task_Initiated','Task Assigned (#wf_level#)','Stride Build','StrideBuild@jmfl.com',1,'http://localhost:57101?returnValue=Tasks/Task/ViewTask/',getdate(),3986,
'<!DOCTYPE html>
<html lang="en">

<head>
    <style>
        body {
            font-family: Verdana;
        }

        .subject {
            font-size: 13px;
        }

        .email-body {
            text-align: justify;
            font-size: 12px;
        }

            .email-body p {
                line-height: 25px;
                margin-bottom: 7px;
            }

        .email-attachments {
            padding: 17px 0px;
            border-top: 1px solid #e6e5e5;
        }

        table th, td {
            text-align: left;
            font-size: 12px;
        }
    </style>
</head>
<body>
    <div class="main-content container-fluid p-0">
        <div class="email-body">
            <p>Hi,</p>
            <div class="subject">
                <p>The following #wf_level# task has been assigned to you.</p> <br /><br />
                <p>
                    <table border="1" cellspacing="0" cellpadding="6">
                        <tbody>
                            <tr>
                                <td>Project</td>
                                <td>#project#</td>
                            </tr>
                            <tr>
                                <td>Category</td>
                                <td>#category#</td>
                            </tr>
                            <tr>
                                <td>Module</td>
                                <td>#module#</td>
                            </tr>
                            <tr>
                                <td>Task Subject</td>
                                <td>#subject#</td>
                            </tr>
                            <tr>
                                <td>Status</td>
                                <td>#status#</td>
                            </tr>
                            <tr>
                                <td>Workflow level/Step</td>
                                <td>#wf_level#</td>
                            </tr>
                            <tr>
                                <td>Comments</td>
                                <td>#comment#</td>
                            </tr>

                        </tbody>
                    </table>
                </p>
                <br />
                <p><a href="#url#">Click Here</a> to know more.</p>                  <br /><br />
                <p>-This is a system generated mail. Please do not reply to this mail.</p>
            </div>
        </div>
    </div>
</body>
</html>'
)
GO

--Task Acknowledged
insert into email_template (template_syscode,template_name, template_subject,from_email_display,from_email_id,is_active,link_url,created_on,created_by,template_body)
values(7,'Task_Acknowledged','Task Acknowledged (#wf_level#)','Stride Build','StrideBuild@jmfl.com',1,'http://localhost:57101?returnValue=Tasks/Task/ViewTask/',getdate(),3986,
'<!DOCTYPE html>
<html lang="en">

<head>
    <style>
        body {
            font-family: Verdana;
        }

        .subject {
            font-size: 13px;
        }

        .email-body {
            text-align: justify;
            font-size: 12px;
        }

            .email-body p {
                line-height: 25px;
                margin-bottom: 7px;
            }

        .email-attachments {
            padding: 17px 0px;
            border-top: 1px solid #e6e5e5;
        }

        table th, td {
            text-align: left;
            font-size: 12px;
        }
    </style>
</head>
<body>
    <div class="main-content container-fluid p-0">
        <div class="email-body">
            <p>Hi,</p>
            <div class="subject">
                <p>The following #wf_level# task has been acknowledged by #emp_name#.</p> <br /><br />
                <p>
                    <table border="1" cellspacing="0" cellpadding="6">
                        <tbody>
                            <tr>
                                <td>Project</td>
                                <td>#project#</td>
                            </tr>
                            <tr>
                                <td>Category</td>
                                <td>#category#</td>
                            </tr>
                            <tr>
                                <td>Module</td>
                                <td>#module#</td>
                            </tr>
                            <tr>
                                <td>Task Subject</td>
                                <td>#subject#</td>
                            </tr>
                            <tr>
                                <td>Status</td>
                                <td>#status#</td>
                            </tr>
                            <tr>
                                <td>Workflow level/Step</td>
                                <td>#wf_level#</td>
                            </tr>
                            <tr>
                                <td>Comments</td>
                                <td>#comment#</td>
                            </tr>

                        </tbody>
                    </table>
                </p>
                <br />
                <p><a href="#url#">Click Here</a> to know more.</p>                  <br /><br />
                <p>-This is a system generated mail. Please do not reply to this mail.</p>
            </div>
        </div>
    </div>
</body>
</html>'
)
GO

--Task In-Progress
insert into email_template (template_syscode,template_name, template_subject,from_email_display,from_email_id,is_active,link_url,created_on,created_by,template_body)
values(8,'Task_Inprogress','Task Started (#wf_level#)','Stride Build','StrideBuild@jmfl.com',1,'http://localhost:57101?returnValue=Tasks/Task/ViewTask/',getdate(),3986,
'<!DOCTYPE html>
<html lang="en">

<head>
    <style>
        body {
            font-family: Verdana;
        }

        .subject {
            font-size: 13px;
        }

        .email-body {
            text-align: justify;
            font-size: 12px;
        }

            .email-body p {
                line-height: 25px;
                margin-bottom: 7px;
            }

        .email-attachments {
            padding: 17px 0px;
            border-top: 1px solid #e6e5e5;
        }

        table th, td {
            text-align: left;
            font-size: 12px;
        }
    </style>
</head>
<body>
    <div class="main-content container-fluid p-0">
        <div class="email-body">
            <p>Hi,</p>
            <div class="subject">
                <p>The following #wf_level# task has been started by #emp_name#.</p> <br /><br />
                <p>
                    <table border="1" cellspacing="0" cellpadding="6">
                        <tbody>
                            <tr>
                                <td>Project</td>
                                <td>#project#</td>
                            </tr>
                            <tr>
                                <td>Category</td>
                                <td>#category#</td>
                            </tr>
                            <tr>
                                <td>Module</td>
                                <td>#module#</td>
                            </tr>
                            <tr>
                                <td>Task Subject</td>
                                <td>#subject#</td>
                            </tr>
                            <tr>
                                <td>Status</td>
                                <td>#status#</td>
                            </tr>
                            <tr>
                                <td>Workflow level/Step</td>
                                <td>#wf_level#</td>
                            </tr>
                            <tr>
                                <td>Comments</td>
                                <td>#comment#</td>
                            </tr>

                        </tbody>
                    </table>
                </p>
                <br />
                <p><a href="#url#">Click Here</a> to know more.</p>                  <br /><br />
                <p>-This is a system generated mail. Please do not reply to this mail.</p>
            </div>
        </div>
    </div>
</body>
</html>'
)
GO

--Task OnHold
insert into email_template (template_syscode,template_name, template_subject,from_email_display,from_email_id,is_active,link_url,created_on,created_by,template_body)
values(9,'Task_OnHold','Task On Hold (#wf_level#)','Stride Build','StrideBuild@jmfl.com',1,'http://localhost:57101?returnValue=Tasks/Task/ViewTask/',getdate(),3986,
'<!DOCTYPE html>
<html lang="en">

<head>
    <style>
        body {
            font-family: Verdana;
        }

        .subject {
            font-size: 13px;
        }

        .email-body {
            text-align: justify;
            font-size: 12px;
        }

            .email-body p {
                line-height: 25px;
                margin-bottom: 7px;
            }

        .email-attachments {
            padding: 17px 0px;
            border-top: 1px solid #e6e5e5;
        }

        table th, td {
            text-align: left;
            font-size: 12px;
        }
    </style>
</head>
<body>
    <div class="main-content container-fluid p-0">
        <div class="email-body">
            <p>Hi,</p>
            <div class="subject">
                <p>The following #wf_level# task has been put on hold by #emp_name#.</p> <br /><br />
                <p>
                    <table border="1" cellspacing="0" cellpadding="6">
                        <tbody>
                            <tr>
                                <td>Project</td>
                                <td>#project#</td>
                            </tr>
                            <tr>
                                <td>Category</td>
                                <td>#category#</td>
                            </tr>
                            <tr>
                                <td>Module</td>
                                <td>#module#</td>
                            </tr>
                            <tr>
                                <td>Task Subject</td>
                                <td>#subject#</td>
                            </tr>
                            <tr>
                                <td>Status</td>
                                <td>#status#</td>
                            </tr>
                            <tr>
                                <td>Workflow level/Step</td>
                                <td>#wf_level#</td>
                            </tr>
                            <tr>
                                <td>Comments</td>
                                <td>#comment#</td>
                            </tr>

                        </tbody>
                    </table>
                </p>
                <br />
                <p><a href="#url#">Click Here</a> to know more.</p>                  <br /><br />
                <p>-This is a system generated mail. Please do not reply to this mail.</p>
            </div>
        </div>
    </div>
</body>
</html>'
)
GO

--Task Discarded
insert into email_template (template_syscode,template_name, template_subject,from_email_display,from_email_id,is_active,link_url,created_on,created_by,template_body)
values(10,'Task_Discarded','Task Discarded (#wf_level#)','Stride Build','StrideBuild@jmfl.com',1,'http://localhost:57101?returnValue=Tasks/Task/ViewTask/',getdate(),3986,
'<!DOCTYPE html>
<html lang="en">

<head>
    <style>
        body {
            font-family: Verdana;
        }

        .subject {
            font-size: 13px;
        }

        .email-body {
            text-align: justify;
            font-size: 12px;
        }

            .email-body p {
                line-height: 25px;
                margin-bottom: 7px;
            }

        .email-attachments {
            padding: 17px 0px;
            border-top: 1px solid #e6e5e5;
        }

        table th, td {
            text-align: left;
            font-size: 12px;
        }
    </style>
</head>
<body>
    <div class="main-content container-fluid p-0">
        <div class="email-body">
            <p>Hi,</p>
            <div class="subject">
                <p>The following #wf_level# task has been discarded by #emp_name#.</p> <br /><br />
                <p>
                    <table border="1" cellspacing="0" cellpadding="6">
                        <tbody>
                            <tr>
                                <td>Project</td>
                                <td>#project#</td>
                            </tr>
                            <tr>
                                <td>Category</td>
                                <td>#category#</td>
                            </tr>
                            <tr>
                                <td>Module</td>
                                <td>#module#</td>
                            </tr>
                            <tr>
                                <td>Task Subject</td>
                                <td>#subject#</td>
                            </tr>
                            <tr>
                                <td>Status</td>
                                <td>#status#</td>
                            </tr>
                            <tr>
                                <td>Workflow level/Step</td>
                                <td>#wf_level#</td>
                            </tr>
                            <tr>
                                <td>Comments</td>
                                <td>#comment#</td>
                            </tr>

                        </tbody>
                    </table>
                </p>
                <br />
                <p><a href="#url#">Click Here</a> to know more.</p>                  <br /><br />
                <p>-This is a system generated mail. Please do not reply to this mail.</p>
            </div>
        </div>
    </div>
</body>
</html>'
)
GO

--Task Completed
insert into email_template (template_syscode,template_name, template_subject,from_email_display,from_email_id,is_active,link_url,created_on,created_by,template_body)
values(11,'Task_Completed','Task Completed (#wf_level#)','Stride Build','StrideBuild@jmfl.com',1,'http://localhost:57101?returnValue=Tasks/Task/ViewTask/',getdate(),3986,
'<!DOCTYPE html>
<html lang="en">

<head>
    <style>
        body {
            font-family: Verdana;
        }

        .subject {
            font-size: 13px;
        }

        .email-body {
            text-align: justify;
            font-size: 12px;
        }

            .email-body p {
                line-height: 25px;
                margin-bottom: 7px;
            }

        .email-attachments {
            padding: 17px 0px;
            border-top: 1px solid #e6e5e5;
        }

        table th, td {
            text-align: left;
            font-size: 12px;
        }
    </style>
</head>
<body>
    <div class="main-content container-fluid p-0">
        <div class="email-body">
            <p>Hi,</p>
            <div class="subject">
                <p>The following #wf_level# task has been completed by #emp_name#.</p> <br /><br />
                <p>
                    <table border="1" cellspacing="0" cellpadding="6">
                        <tbody>
                            <tr>
                                <td>Project</td>
                                <td>#project#</td>
                            </tr>
                            <tr>
                                <td>Category</td>
                                <td>#category#</td>
                            </tr>
                            <tr>
                                <td>Module</td>
                                <td>#module#</td>
                            </tr>
                            <tr>
                                <td>Task Subject</td>
                                <td>#subject#</td>
                            </tr>
                            <tr>
                                <td>Status</td>
                                <td>#status#</td>
                            </tr>
                            <tr>
                                <td>Workflow level/Step</td>
                                <td>#wf_level#</td>
                            </tr>
                            <tr>
                                <td>Comments</td>
                                <td>#comment#</td>
                            </tr>

                        </tbody>
                    </table>
                </p>
                <br />
                <p><a href="#url#">Click Here</a> to know more.</p>                  <br /><br />
                <p>-This is a system generated mail. Please do not reply to this mail.</p>
            </div>
        </div>
    </div>
</body>
</html>'
)
GO

-- 