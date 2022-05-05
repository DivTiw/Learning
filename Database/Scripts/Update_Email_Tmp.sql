update email_template
set from_email_display = from_email_display + '.#emp_name#'
WHERE template_name not in ('Sch_StopTask','Sch_UsrWklyActivity_Rpt')


update email_template
set template_subject = 'Task Updated : #project_name# | #module_name#[ | #wf_level#] | #task_ref#'
where template_name = 'Task_Updated'

update email_template
set template_subject = 'Task Created : #project_name# | #module_name#[ | #wf_level#] | #task_ref#'
where template_name = 'WF_Task_Created'

update email_template
set template_subject = 'Task InformTo : #project_name# | #module_name#[ | #wf_level#] | #task_ref#'
where template_name = 'Inform_To'

update email_template
set template_subject = 'Task Assigned : #project_name# | #module_name#[ | #wf_level#] | #task_ref#'
where template_name = 'Task_Initiated'


update email_template
set template_subject = 'Task Acknowledged : #project_name# | #module_name#[ | #wf_level#] | #task_ref#'
where template_name = 'Task_Acknowledged'


update email_template
set template_subject = 'Task Started : #project_name# | #module_name#[ | #wf_level#] | #task_ref#'
where template_name = 'Task_Inprogress'


update email_template
set template_subject = 'Task OnHold : #project_name# | #module_name#[ | #wf_level#] | #task_ref#'
where template_name = 'Task_OnHold'

update email_template
set template_subject = 'Task Discarded : #project_name# | #module_name#[ | #wf_level#] | #task_ref#'
where template_name = 'Task_Discarded'

update email_template
set template_subject = 'Task Completed : #project_name# | #module_name#[ | #wf_level#] | #task_ref#'
where template_name = 'Task_Completed'

update email_template
set template_subject = 'Task Created : Standalone | #task_ref#'
where template_name = 'ST_Task_Created'


update email_template
set template_body = '<!DOCTYPE html>
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
            <p>Hi #to_emp#,</p>
            <div class="subject">
                <!--<p>The following #wf_level# task has been modified by #emp_name#</p> <br /><br />-->
                <p>#emp_name# wishes to inform you about following.</p> <br />
                <p>#comment# </p><br />
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
where template_name = 'Inform_To'