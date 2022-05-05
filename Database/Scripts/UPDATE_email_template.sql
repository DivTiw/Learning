UPDATE [dbo].[email_template]
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
    <div class="main-content container-fluid  p-0">
        <div class="email-body">
            <p><strong>Dear Team,</strong> </p>

            <div class="subject">
                <p>
                    Following Task has been created by #emp_name#. <br />
                </p>
                <br/>
                <table border="1" cellspacing="0" cellpadding="6">
                    <thead>
                        <tr>
                            <th>Task Ref</th>
                            <th>Task Subject</th>
                            <th>Members</th>
                        </tr>
                    </thead>
                    <tbody>
                        #rows#
                    </tbody>
                </table>
                <br />
                <a href="#Link#">Click Here</a> to login.
                <br /><br />
                <p>-This is a system generated mail. Please do not reply to this mail.</p>
            </div>
        </div>
    </div>
</body>
</html>'
where template_syscode = 1



UPDATE [dbo].[email_template]
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
    <div class="main-content container-fluid  p-0">
        <div class="email-body">
            <p><strong>Dear Team,</strong> </p>
            <div class="subject">
                <p>
                    Following Task has been created by #emp_name#. <br /><br/>
                    <strong>Project:</strong>#project_name# <br />
                    <strong>Module:</strong>#module_name#  <br />
                </p>
                <br/>
                <table border="1" cellspacing="0" cellpadding="6">
                    <thead>
                        <tr>
                            <th>Task Ref</th>
                            <th>Task Subject</th>
                            <th>Members</th>
                        </tr>
                    </thead>
                    <tbody>#rows#</tbody>
                </table>
                <br />

                <br /><br />
                <p>-This is a system generated mail. Please do not reply to this mail.</p>
            </div>
        </div>
    </div>
</body>
</html>'
where template_syscode = 3