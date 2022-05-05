--Sch_UsrWklyActivity_Rpt
insert into email_template (template_syscode,template_name, template_subject,from_email_display,from_email_id,is_active,link_url,created_on,created_by,template_body)
values(12,'Sch_UsrWklyActivity_Rpt','Stride Weekly Task Activity','Stride Build','StrideBuild@jmfl.com',1,'http://localhost:57101?returnValue=Tasks/Task/ViewTask/',getdate(),3986,
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
            <p><strong>Dear #emp_name#,</strong> </p>
            <div class="subject">
                <p>
                    Please find below your <b>Task Activity Report</b> for the previous week.                   
                </p>
                <br />

                <table border="1" cellspacing="0" cellpadding="6">
                    <thead>
                        <tr>
                            <th>Task Ref</th>
                            <th>Status</th>
                            <th>Task Subject</th>
                            <th>Module Name</th>
                            <th>Project Name</th>
                            <th>Hours Worked in week</th>
                        </tr>
                    </thead>
                    <tbody>#rows#</tbody>
                </table>   
                <br /> <br /><br />
                <p>-This is a system generated mail. Please do not reply to this mail.</p>
            </div>
        </div>
    </div>
</body>
</html>'
)
GO
