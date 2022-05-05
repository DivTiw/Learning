UPDATE email_template
SET template_body = '<!DOCTYPE html>
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
                            <th>Project Name</th>
                            <th>Module Name</th>
                            <th>Parent Task</th>
                            <th>Task Subject</th>
                            <th>Status</th>
                            <th>Hours</th>
                            <th>Days</th>
                        </tr>
                    </thead>
                    <tbody>#rows#</tbody>
                </table>       <br />

                <p><strong>Attendance Report</strong></p>    <br />
                <table border="1" cellspacing="0" cellpadding="6">
                    <thead>
                        <tr>
                            <th>Employee Code</th>
                            <th>Employee Name</th>
                            <th>Day</th>
                            <th>Date</th>
                            <th>Actual In Time</th>
                            <th>Actual Out Time</th>
                            <th>Regularized In Time</th>
                            <th>Regularized Out Time</th>
                            <th>Hours</th>
                            <th>Attendance Description</th>
                            <th>Leave Type</th>
                            <th>Attendance Source</th>
                        </tr>
                    </thead>
                    <tbody>#rows1#</tbody>
                </table>
                <br /> <br /><br />
                <p>-This is a system generated mail. Please do not reply to this mail.</p>
            </div>
        </div>
    </div>
</body>
</html>'

WHERE template_name = 'Sch_UsrWklyActivity_Rpt'