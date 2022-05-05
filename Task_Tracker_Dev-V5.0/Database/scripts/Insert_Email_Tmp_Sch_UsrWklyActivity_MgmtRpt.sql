insert into email_template (template_syscode,template_name, template_subject,from_email_display,from_email_id,is_active,created_on,created_by,template_body)
values(15,'Sch_UsrWklyActivity_MgmtRpt','Stride Weekly Task Activity Report','Stride Build','StrideBuild@jmfl.com',1,getdate(),3986,
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
            <p>
                Dear #emp_name#,
            </p>
            <div class="subject">
                <p> Please find attached CnAppTeam Stride Task Activity Report for the previous week starting #monday# and ending on #sunday#. </p>
                <br />

                <br /> <br /><br />
                <p>-This is a system generated mail. Please do not reply to this mail.</p>
            </div>
        </div>
    </div>
</body>
</html>'
)
GO
