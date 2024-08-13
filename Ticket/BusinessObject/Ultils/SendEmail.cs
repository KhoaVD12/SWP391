using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Caching.Memory;

namespace BusinessObject.Ultils;

public class SendEmail
{
    public static async Task<bool> SendRegistrationEmail(IMemoryCache cache,string recipientEmail, string recipientName, DateTime registrationDate, string checkInCode)
    {
        const string userName = "FUEventTicket";
        const string emailFrom = "phongdinh930@gmail.com";
        const string password = "idom wmfd dczl ccoq";

        const string subject = "Your Event Registration";
        var formattedDate = registrationDate.ToString("f");
        var body = $@"
            <div style='font-family: Arial, sans-serif; font-size: 16px; color: #333;'>
                <p>Dear {recipientName},</p>
                <p>We are delighted to confirm your registration for our upcoming event.</p>
                <p>
                    <strong>Event Date:</strong> {formattedDate}<br>
                    <strong>Check-In Code:</strong> <span style='font-size: 18px; color: #007BFF;'>{checkInCode}</span>
                </p>
                <p>Please save this email as you will need the check-in code to gain entry to the event. We recommend arriving at the venue a few minutes early to ensure a smooth check-in process.</p>
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
        message.Body = new TextPart("html") { Text = body };
        // Lưu mã OTP vào cache
        var key = $"{recipientEmail}_OTP";
        cache.Set(key, checkInCode, TimeSpan.FromMinutes(1));
        // Thêm logic xóa key OTP khi hết hạn
        await Task.Delay(TimeSpan.FromMinutes(1)).ContinueWith(_ =>
        {
            cache.Remove(key);
        });

        using (var client = new SmtpClient())
        {
           await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            //authenticate account email
            client.Authenticate(emailFrom, password);

            try
            {
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                return true;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}