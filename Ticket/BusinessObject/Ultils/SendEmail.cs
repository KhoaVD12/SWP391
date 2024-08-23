using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using QRCoder;
using System.Drawing.Imaging;

namespace BusinessObject.Ultils;

public class SendEmail
{
    public static async Task SendRegistrationEmail(string recipientEmail,
        string recipientName, string checkInCode)
    {
        const string userName = "FUEventTicket";
        const string emailFrom = "phongdinh930@gmail.com";
        const string password = "idom wmfd dczl ccoq";

        const string subject = "Your Event Registration";
        var now = DateTime.UtcNow;
        var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(now, localTimeZone);

        // Generate QR code
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(checkInCode, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new QRCode(qrCodeData);
        var qrCodeImage = qrCode.GetGraphic(20);

        using var ms = new MemoryStream();
        qrCodeImage.Save(ms, ImageFormat.Png);
        ms.Position = 0;

        var imageAttachment = new MimePart("image", "png")
        {
            Content = new MimeContent(ms),
            ContentDisposition = new ContentDisposition(ContentDisposition.Inline),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = "qr.png"
        };
        imageAttachment.ContentId = "QRCodeImage";
        var body = $@"
    <div style='font-family: Arial, sans-serif; font-size: 16px; color: #333;'>
        <p>Dear {recipientName},</p>
        <p>We are delighted to confirm your registration for our upcoming event.</p>
        <p>
            <strong>Event Date:</strong> {localDateTime}<br>
            <strong>Check-In Code:</strong> <span style='font-size: 18px; color: #007BFF;'>{checkInCode}</span>
        </p>
        <p>Please save this email as you will need the check-in code to gain entry to the event. We recommend arriving at the venue a few minutes early to ensure a smooth check-in process.</p>
        <p>Alternatively, you can scan the following QR code during check-in:</p>
        <p><img src=""cid:QRCodeImage"" alt=""Check-In Code"" /></p>
        <p>If you have any questions or need further assistance, feel free to contact our support team.</p>
        <p>We look forward to seeing you at the event!</p>
        <p>Best Regards,<br>The Event Team</p>
        <hr>
        <p style='font-size: 14px; color: #555;'>This email was sent to {recipientEmail}. If you did not register for this event, please disregard this email.</p>
    </div>";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(userName, emailFrom));
        message.To.Add(new MailboxAddress("", recipientEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = body };
        bodyBuilder.LinkedResources.Add(imageAttachment);
        message.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            client.Authenticate(emailFrom, password);

            try
            {
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}