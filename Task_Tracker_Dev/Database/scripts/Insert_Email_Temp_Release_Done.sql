insert into email_template (template_syscode,template_name, template_subject,from_email_display,from_email_id,is_active,link_url,created_on,created_by,template_body)
values(14,'Release_Done','#env#  Release Done (#rel_ref#) : #project_name# | #module_name#[ | #wf_level#]','Stride Build.#emp_name#','StrideBuild@jmfl.com',1,'http://localhost:57101?returnValue=Tasks/Task/ViewTask/',getdate(),3986,
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
            <p>Hi #to_emp#,</p>
            <div class="subject">
                <p>The following release has been deployed by #emp_name#.</p> <br /><br />
                <p>
                    <table border="1" cellspacing="0" cellpadding="6">
                        <tbody>
                            <tr>
                                <td>Project</td>
                                <td>#project#</td>
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
                                <td>Release Ref</td>
                                <td>#rel_ref#</td>
                            </tr>
                            <tr>
                                <td>Environment</td>
                                <td>#env#</td>
                            </tr>
                            <tr>
                                <td>Remarks</td>
                                <td>#remarks#</td>
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