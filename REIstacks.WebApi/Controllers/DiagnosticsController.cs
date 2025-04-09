using Microsoft.AspNetCore.Mvc;
using REIstacks.Application.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace reistacks_api.Controllers;

[ApiController]
[Route("api/diagnostics")]
public class DiagnosticsController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly ILogger<DiagnosticsController> _logger;

    public DiagnosticsController(IEmailService emailService, ILogger<DiagnosticsController> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    [HttpGet("email-config")]
    public IActionResult CheckEmailConfig()
    {
        // Check AWS Credentials
        var awsAccessKeyId = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
        var awsSecretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
        var awsRegion = Environment.GetEnvironmentVariable("AWS_REGION");
        var sesVerifiedEmail = Environment.GetEnvironmentVariable("SES_VERIFIED_EMAIL");

        var result = new
        {
            HasAwsAccessKey = !string.IsNullOrEmpty(awsAccessKeyId),
            HasAwsSecretKey = !string.IsNullOrEmpty(awsSecretKey),
            AwsRegion = awsRegion,
            HasSesVerifiedEmail = !string.IsNullOrEmpty(sesVerifiedEmail),
            SesVerifiedEmail = sesVerifiedEmail
        };

        _logger.LogInformation("Email configuration check: {@Result}", result);

        return Ok(result);
    }

    [HttpPost("test-email")]
    public async Task<IActionResult> SendTestEmail([FromBody] TestEmailRequest request)
    {
        try
        {
            // Create a test invite ID (GUID)
            var testInviteId = Guid.NewGuid().ToString();

            // Attempt to send an email
            var success = await _emailService.SendInviteEmail(request.ToEmail, testInviteId);

            return Ok(new { Success = success, Message = success ? "Test email sent successfully" : "Failed to send test email" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending test email");
            return StatusCode(500, new { Success = false, Message = $"Error: {ex.Message}" });
        }
    }

    [HttpGet("invitation")]
    public IActionResult PreviewInvitationEmail([FromQuery] string teamName = "Test Organization")
    {
        // Generate your HTML email template
        var htmlBody = GenerateInvitationEmail(teamName);

        // Return as HTML content directly in browser
        return Content(htmlBody, "text/html");
    }

    private string GenerateInvitationEmail(string teamName)
    {
        // Your existing email template generation logic
        string inviteUrl = "https://reistacks.com/invite/test-invite-link";

        var htmlBody = $@"<!DOCTYPE html>
<html>
  <head>
    <!--
      Note: Media queries and advanced reset styles won't fully work when inlined.
      If you need responsive behavior, consider an approach like
      MJML or a tool like Premailer to partially preserve CSS or handle media queries.
    -->
    <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
    <title>Invitation Email</title>
  </head>
  <body
    style=""
      margin: 0;
      padding: 0;
      background-color: #eaebed;
      font-family: sans-serif;
      -webkit-font-smoothing: antialiased;
      font-size: 14px;
      line-height: 1.4;
      -ms-text-size-adjust: 100%;
      -webkit-text-size-adjust: 100%;
    ""
  >
    <table
      role=""presentation""
      border=""0""
      cellpadding=""0""
      cellspacing=""0""
      width=""100%""
      style=""
        border-collapse: separate;
        mso-table-lspace: 0pt;
        mso-table-rspace: 0pt;
        min-width: 100%;
        width: 100%;
        background-color: #eaebed;
      ""
    >
      <tr>
        <td style=""vertical-align: top;"">&nbsp;</td>
        <td
          class=""container""
          style=""
            display: block;
            margin: 0 auto;
            max-width: 580px;
            padding: 10px;
            width: 580px;
            vertical-align: top;
          ""
        >
          <!-- HEADER -->
          <div
            class=""header""
            style=""
              padding: 20px 0;
              text-align: center;
            ""
          >
            <table
              role=""presentation""
              border=""0""
              cellpadding=""0""
              cellspacing=""0""
              width=""100%""
              style=""
                border-collapse: separate;
                mso-table-lspace: 0pt;
                mso-table-rspace: 0pt;
                width: 100%;
              ""
            >
              <tr>
                <td
                  style=""
                    vertical-align: top;
                    text-align: center;
                  ""
                >
                  <a href=""https://reistacks.com"" style=""text-decoration: none;"">
                    <img
                      src=""https://myrei.reistacks.com/_next/image?url=%2Flayout%2Fimages%2Flogo-reistacks-color.png&w=640&q=75""
                      height=""40""
                      alt=""REIstacks""
                      style=""
                        border: none;
                        -ms-interpolation-mode: bicubic;
                        max-width: 100%;
                      ""
                    />
                  </a>
                </td>
              </tr>
            </table>
          </div>

          <!-- CONTENT -->
          <div
            class=""content""
            style=""
              box-sizing: border-box;
              display: block;
              margin: 0 auto;
              max-width: 580px;
              padding: 10px;
            ""
          >
            <!-- START CENTERED WHITE CONTAINER -->
            <span
              class=""preheader""
              style=""
                color: transparent;
                display: none;
                height: 0;
                max-height: 0;
                max-width: 0;
                opacity: 0;
                overflow: hidden;
                mso-hide: all;
                visibility: hidden;
                width: 0;
              ""
            >
              This is preheader text. Some clients will show this text as a preview.
            </span>

            <table
              role=""presentation""
              class=""main""
              style=""
                border-collapse: separate;
                mso-table-lspace: 0pt;
                mso-table-rspace: 0pt;
                background: #ffffff;
                border-radius: 8px;
                -webkit-border-radius: 8px;
                -moz-border-radius: 8px;
                width: 100%;
              ""
            >
              <!-- MAIN CONTENT AREA -->
              <tr>
                <td
                  class=""wrapper""
                  style=""
                    box-sizing: border-box;
                    padding: 20px;
                    vertical-align: top;
                  ""
                >
                  <table
                    role=""presentation""
                    border=""0""
                    cellpadding=""0""
                    cellspacing=""0""
                    style=""border-collapse: separate; width: 100%;""
                  >
                    <tr>
                      <td style=""vertical-align: top;"">
                        <p
                          style=""
                            margin: 0;
                            margin-bottom: 15px;
                            font-size: 14px;
                            text-align: center;
                            font-family: sans-serif;
                            color: #06090f;
                          ""
                        >
                          👋  You have been invited to join
                        </p>
                          
                        <h1
                          style=""
                            margin: 0;
                            margin-bottom: 30px;
                            font-weight: 300;
                            text-align: center;
                            text-transform: capitalize;
                            font-size: 35px;
                            line-height: 1.4;
                            color: #06090f;
                            font-family: sans-serif;
                          ""
                        >
                          {teamName}
                        </h1>
<p
                          style=""
                            margin: 0;
                            margin-bottom: 15px;
                            font-size: 14px;
                            text-align: center;
                            font-family: sans-serif;
                            color: #06090f;
                          ""
                        >
                          on REIstacks!
                        </p>
                        <table
                          role=""presentation""
                          border=""0""
                          cellpadding=""0""
                          cellspacing=""0""
                          style=""border-collapse: separate; width: 100%;""
                        >
                          <tr>
                            <td align=""center"" style=""vertical-align: top;"">
                              <table
                                role=""presentation""
                                border=""0""
                                cellpadding=""0""
                                cellspacing=""0""
                                style=""
                                  border-collapse: separate;
                                  margin-bottom: 15px;
                                ""
                              >
                                <tr>
                                  <td
                                    style=""
                                      background-color: #134be8;
                                      border-radius: 5px;
                                      text-align: center;
                                    ""
                                  >
 
                                    <a
                                      href='{inviteUrl}'
                                      style=""
                                        display: inline-block;
                                        background-color: #134be8;
                                        border: solid 1px #134be8;
                                        border-radius: 5px;
                                        box-sizing: border-box;
                                        color: #ffffff;
                                        cursor: pointer;
                                        font-size: 14px;
                                        font-weight: bold;
                                        margin: 0;
                                        padding: 12px 25px;
                                        text-decoration: none;
                                        text-transform: capitalize;
                                        font-family: sans-serif;
                                      ""
                                    >
                                      Accept Invite
                                    </a>
                                  </td>
                                </tr>
                              </table>
                            </td>
                          </tr>
                        </table>
                        <!-- END CTA -->

                      </td>
                    </tr>
                  </table>
                </td>
              </tr>
            </table>
            <!-- END MAIN CONTENT AREA -->

            <!-- FOOTER -->
            <div
              class=""footer""
              style=""
                clear: both;
                margin-top: 10px;
                text-align: center;
                width: 100%;
              ""
            >
              <table
                role=""presentation""
                border=""0""
                cellpadding=""0""
                cellspacing=""0""
                style=""border-collapse: separate; width: 100%;""
              >
                <tr>
                  <td
                    class=""content-block powered-by""
                    style=""
                      font-size: 12px;
                      color: #9a9ea6;
                      text-align: center;
                      vertical-align: top;
                      padding: 10px 0;
                    ""
                  >
                    Powered by
                    <a
                      href=""https://REIstacks.com""
                      style=""
                        color: #9a9ea6;
                        text-decoration: none;
                        font-family: sans-serif;
                        font-size: 12px;
                      ""
                      >REIstacks</a
                    >.
                  </td>
                </tr>
              </table>
            </div>
            <!-- END FOOTER -->
            <!-- END CENTERED WHITE CONTAINER -->
          </div>
        </td>
        <td style=""vertical-align: top;"">&nbsp;</td>
      </tr>
    </table>
  </body>
</html>         ";

        return htmlBody;
    }

    public class TestEmailRequest
    {
        [Required]
        [EmailAddress]
        public string ToEmail { get; set; }
    }
}
