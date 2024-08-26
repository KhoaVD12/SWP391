using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace BusinessObject.Ultils;

public class SendEmail
{
    public static async Task SendRegistrationEmail(string recipientEmail, string recipientName, string eventTitle,
        DateTime eventStartDate, decimal amountPaid, string checkInCode)
    {
        const string userName = "FUEventTicket";
        const string emailFrom = "phongdinh930@gmail.com";
        const string password = "idom wmfd dczl ccoq";

        const string subject = "Your Event Registration";
        var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var localEventDate = TimeZoneInfo.ConvertTimeFromUtc(eventStartDate, localTimeZone);

        var body = $@"
            <div style='font-family: Arial, sans-serif; font-size: 16px; color: #333;'>
                <p>Dear {recipientName},</p>
                <p>We are delighted to confirm your registration for the following event:</p>
                <p>
                    <strong>Event Title:</strong> {eventTitle}<br>
                    <strong>Event Date:</strong> {localEventDate}<br>
                    <strong>Amount Paid:</strong> {amountPaid:C}<br>
                    <strong>Check-In Code:</strong> <span style='font-size: 18px; color: #007BFF;'>{checkInCode}</span>
                </p>
                <p>Please save this email as you will need the check-in code to gain entry to the event. We recommend arriving at the venue a few minutes early to ensure a smooth check-in process.</p>
                <p>If you have any questions or need further assistance, feel free to contact our support team.</p>
                <p>We look forward to seeing you at the event!</p>
                <p>Best Regards,<br>The Event Team</p>
                <hr>
                <p style='font-size: 14px; color: #555;'>Please note that once the ticket has been purchased, it is non-refundable.</p>
                <p style='font-size: 14px; color: #555;'>This email was sent to {recipientEmail}. If you did not register for this event, please disregard this email.</p>
            </div>";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(userName, emailFrom));
        message.To.Add(new MailboxAddress("", recipientEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = body };
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