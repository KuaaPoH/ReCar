using System.Net;
using System.Net.Mail;

namespace CarRental.Services
{
    public interface IEmailService
    {
        Task<(bool Success, string Message)> SendQuickRegisterPasswordAsync(string toEmail, string fullName, string tempPassword);
        Task<(bool Success, string Message)> SendResetPasswordAsync(string toEmail, string fullName, string newPassword);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<(bool Success, string Message)> SendQuickRegisterPasswordAsync(string toEmail, string fullName, string tempPassword)
        {
            return await SendPasswordEmailCoreAsync(
                toEmail,
                "[ReCar] Thông tin tài khoản & mật khẩu đăng nhập đặt xe",
                fullName,
                tempPassword,
                "Tài khoản ReCar của bạn đã được khởi tạo thành công. Vui lòng sử dụng thông tin dưới đây để đăng nhập và tiếp tục hoàn tất đặt cọc xe:",
                "Mật khẩu tạm thời:"
            );
        }

        public async Task<(bool Success, string Message)> SendResetPasswordAsync(string toEmail, string fullName, string newPassword)
        {
            return await SendPasswordEmailCoreAsync(
                toEmail,
                "[ReCar] Mật khẩu đăng nhập mới của bạn",
                fullName,
                newPassword,
                "Chúng tôi đã nhận được yêu cầu cấp lại mật khẩu cho tài khoản ReCar của bạn. Vui lòng sử dụng mật khẩu mới dưới đây để đăng nhập:",
                "Mật khẩu mới:"
            );
        }

        private async Task<(bool Success, string Message)> SendPasswordEmailCoreAsync(
            string toEmail,
            string subject,
            string fullName,
            string password,
            string introText,
            string passwordLabel)
        {
            var smtpHost = _config["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.TryParse(_config["EmailSettings:SmtpPort"], out var port) ? port : 587;
            var senderName = _config["EmailSettings:SenderName"] ?? "ReCar - Thuê Xe Tự Lái";
            var senderEmail = _config["EmailSettings:SenderEmail"]?.Trim();
            var appPassword = _config["EmailSettings:AppPassword"]?.Trim();
            var enableSsl = !bool.TryParse(_config["EmailSettings:EnableSsl"], out var ssl) || ssl;

            if (string.IsNullOrWhiteSpace(senderEmail) || string.IsNullOrWhiteSpace(appPassword))
            {
                return (false, "Hệ thống chưa cấu hình EmailSettings:SenderEmail và AppPassword trong appsettings.json.");
            }

            try
            {
                using var message = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    IsBodyHtml = true,
                    Body = BuildEmailBody(fullName, toEmail, password, introText, passwordLabel)
                };
                message.To.Add(new MailAddress(toEmail));

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(senderEmail, appPassword),
                    EnableSsl = enableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 15000
                };

                await client.SendMailAsync(message);
                return (true, "Đã gửi email thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi gửi email Gmail SMTP tới {Email}", toEmail);
                return (false, $"Không thể gửi email qua Gmail SMTP: {ex.Message}");
            }
        }

        private static string BuildEmailBody(string fullName, string email, string password, string introText, string passwordLabel)
        {
            var safeName = WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(fullName) ? "Quý khách" : fullName);
            var safeEmail = WebUtility.HtmlEncode(email);
            var safePass = WebUtility.HtmlEncode(password);
            var safeIntro = WebUtility.HtmlEncode(introText);
            var safePassLabel = WebUtility.HtmlEncode(passwordLabel);

            return $@"
<!DOCTYPE html>
<html lang='vi'>
<head><meta charset='utf-8'></head>
<body style='margin:0;padding:0;background:#f1f5f9;font-family:Arial,Helvetica,sans-serif;color:#0f172a;'>
  <div style='max-width:560px;margin:32px auto;background:#ffffff;border-radius:16px;overflow:hidden;border:1px solid #e2e8f0;box-shadow:0 10px 25px rgba(15,23,42,0.06);'>
    <div style='background:linear-gradient(135deg,#0f172a,#0284c7);padding:24px 28px;color:#ffffff;'>
      <div style='font-size:22px;font-weight:800;letter-spacing:0.3px;'>ReCar</div>
      <div style='font-size:13px;opacity:0.88;margin-top:4px;'>Nền tảng thuê xe tự lái &amp; chia sẻ xe trực tuyến</div>
    </div>
    <div style='padding:28px;'>
      <h2 style='margin:0 0 12px;font-size:19px;color:#0f172a;'>Xin chào {safeName},</h2>
      <p style='margin:0 0 16px;font-size:14px;line-height:1.6;color:#334155;'>
        {safeIntro}
      </p>
      <div style='background:#f8fafc;border:1px solid #cbd5e1;border-radius:12px;padding:16px 18px;margin-bottom:18px;'>
        <div style='font-size:13px;color:#64748b;margin-bottom:6px;'>Email đăng nhập:</div>
        <div style='font-size:15px;font-weight:700;color:#0f172a;margin-bottom:12px;'>{safeEmail}</div>
        <div style='font-size:13px;color:#64748b;margin-bottom:6px;'>{safePassLabel}</div>
        <div style='display:inline-block;background:#e0f2fe;color:#0369a1;font-family:Consolas,monospace;font-size:18px;font-weight:800;padding:8px 14px;border-radius:8px;letter-spacing:1px;'>{safePass}</div>
      </div>
      <p style='margin:0;font-size:13px;line-height:1.6;color:#64748b;'>
        Vì lý do bảo mật, bạn nên đổi lại mật khẩu mới trong mục <strong>Hồ sơ &amp; Bảo mật</strong> sau khi đăng nhập.
      </p>
    </div>
    <div style='background:#f8fafc;padding:14px 28px;border-top:1px solid #e2e8f0;font-size:12px;color:#94a3b8;text-align:center;'>
      © {DateTime.Now.Year} ReCar Smart Mobility. Email được gửi tự động, vui lòng không trả lời.
    </div>
  </div>
</body>
</html>";
        }
    }
}
