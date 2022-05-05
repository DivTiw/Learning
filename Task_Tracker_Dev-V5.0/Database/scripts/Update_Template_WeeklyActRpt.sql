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
            <p>
                <strong>Dear #emp_name#,</strong>
            </p>
            <div class="subject">
                <p> Following is your <b>Task Activity Report</b> for the previous week starting #monday# and ending on #sunday#. </p>
                <br />
                <table border="1" cellspacing="0" cellpadding="6">
                    <thead>
                        <tr>
                            <th>Task Ref</th>
                            <th>Project Name</th>
                            <th>Module Name</th>
                            <th>Task Subject</th>
                            <th>Status</th>
                            <th>Hours</th>
                            <th>Days</th>
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
where template_name = 'Sch_UsrWklyActivity_Rpt'