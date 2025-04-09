using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using Microsoft.Extensions.Configuration;
using REIstacks.Application.Repositories.Interfaces;
using REIstacks.Application.Services.Interfaces;


namespace REIstacks.Infrastructure.Services.Organizations;

public class EmailService : IEmailService
{
    private readonly AmazonSimpleEmailServiceV2Client _sesClient;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly IUnitOfWork _unitOfWork; // ✅ store the injected dependency
    public EmailService(IConfiguration configuration, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;// ✅ assign to the private field

        // Read AWS credentials from environment variables (set in Azure App Service)
        var awsAccessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
        var awsSecretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
        var awsRegion = Environment.GetEnvironmentVariable("AWS_REGION") ?? "us-east-1";

        // Use the environment variables for email configuration
        _fromEmail = Environment.GetEnvironmentVariable("SES_VERIFIED_EMAIL") ?? "casey@reistacks.com";
        _fromName = "REIstacks"; // You could add this as an env variable too if needed

        var region = Amazon.RegionEndpoint.GetBySystemName(awsRegion);
        _sesClient = new AmazonSimpleEmailServiceV2Client(awsAccessKey, awsSecretKey, region);
    }

    public async Task<bool> SendInviteEmail(string toEmail, string inviteToken)
    {
        try
        {
            // ✅ Use the injected UnitOfWork instance

            // 1. Fetch the invitation
            var invite = await _unitOfWork.Invitations.GetByTokenAsync(inviteToken);
            if (invite == null)
                throw new Exception("Invite not found.");

            // 2. Load the organization's/team name
            //    (Assuming your invitation record has 'OrganizationId' and
            //     you store the team's display name in the Organization table.)
            var org = await _unitOfWork.Organizations.GetByIdAsync(invite.OrganizationId);
            var teamName = org?.Name ?? "REIstacks"; // fallback if null

            // 3. Construct the final invite URL
            var inviteUrl = $"https://reistacks.com/join?inviteId={inviteToken}";

            var htmlBody = $@"
<!DOCTYPE html>
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
                      src=""https://kipgmksircieasquldkz.supabase.co/storage/v1/object/public/blogimages/public/logo-light.png""
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
                border-radius: 3px;
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
</html>         
            ";

            var request = new SendEmailRequest
            {
                FromEmailAddress = $"{_fromName} <{_fromEmail}>",
                Destination = new Destination { ToAddresses = new List<string> { toEmail } },
                Content = new EmailContent
                {
                    Simple = new Message
                    {
                        Subject = new Content { Data = $"You're Invited to Join {teamName} on REIstacks!" },
                        Body = new Body
                        {
                            Html = new Content { Data = htmlBody },
                            Text = new Content { Data = $"You've been invited to join {teamName} on REIstacks! Accept here: {inviteUrl}" }
                        }
                    }
                }
            };

            var response = await _sesClient.SendEmailAsync(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
            return false;
        }
    }
}