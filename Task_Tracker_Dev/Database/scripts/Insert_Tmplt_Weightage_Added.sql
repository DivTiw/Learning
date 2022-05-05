insert into email_template (template_syscode,template_name, template_subject,from_email_display,from_email_id,is_active,link_url,created_on,created_by,template_body)
values(13,'Weightage_Added','Task Updated - Weightage Added','Stride Build.#emp_name#','StrideBuild@jmfl.com',1,'http://localhost:57101?returnValue=Tasks/Task/SetWeightage/',getdate(),3986,
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
    <div class="main-content container-fluid  p-0">
        <div class="email-body">
            <p><strong>Hi,</strong> </p>
            <div class="subject">
                <p>
                    Following Task has been updated by #emp_name#. <br />
                    Task Activity: #activity#
                </p>
                <p>
                    <table border="1" cellspacing="0" cellpadding="6">
                        <thead>
                            <tr>
                                <th>Task Ref</th>
                                <th>Task Subject</th>
                            </tr>
                        </thead>
                        <tbody>
                            #rows#
                        </tbody>
                    </table>
                </p>
                <br />                                   <br /><br />
                <p>-This is a system generated mail. Please do not reply to this mail.</p>
            </div>
        </div>
    </div>
</body>
</html>'
)
GO