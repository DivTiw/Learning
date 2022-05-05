update email_template
set
template_subject = 'Task Created', 
template_body = '<!DOCTYPE html>
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
                <p>The following task has been created by #emp_name#</p> <br /><br />
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
                                <td>Workflow</td>
                                <td>#wf#</td>
                            </tr>                          
                            <tr>
                                <td>Status</td>
                                <td>#status#</td>
                            </tr>                            
                            <tr>
                                <td>Task Subject</td>
                                <td>#subject#</td>
                            </tr>
                            <tr>
                                <td>Task Description</td>
                                <td>#desc#</td>
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
</html>',
modified_on = GetDate(),
modified_by = 3986,
link_url = 'http://localhost:57101?returnValue=Tasks/Task/ViewTask/'
where template_name = 'WF_Task_Created'