using elFinder.NetCore;

using SlugGenerator;

using System.Security.Cryptography;

using System.Text;

using Microsoft.AspNetCore.Http;

namespace CarRental.Utilities

{

    public class Function

    {

        // Session keys

        private const string SESSION_ACCOUNT_ID = "AccountId";
        private const string SESSION_USER_ACCOUNT_ID = "UserAccountId";

        private const string SESSION_CAR_OWNER_ACCOUNT_ID = "CarOwnerAccountId";

        private const string SESSION_USER_NAME = "UserName";

        private const string SESSION_EMAIL = "Email";

        private const string SESSION_ADDRESS = "Address";

        private const string SESSION_PHONE = "Phone";

        private const string SESSION_MESSAGE = "Message";

        private const string SESSION_MESSAGE_EMAIL = "MessageEmail";

        private const string SESSION_PICKUP_DATE = "PickupDate";

        private const string SESSION_RETURN_DATE = "ReturnDate";



        // Admin Scoped Keys

        private const string SESSION_ADMIN_ACCOUNT_ID = "Admin_AccountId";

        private const string SESSION_ADMIN_USER_NAME = "Admin_UserName";

        private const string SESSION_ADMIN_EMAIL = "Admin_Email";

        private const string SESSION_ADMIN_AVATAR = "Admin_Avatar";

        private const string SESSION_AVATAR = "Avatar";



        // CarOwner Scoped Keys

        private const string SESSION_CAROWNER_ID = "CarOwner_Id";

        private const string SESSION_CAROWNER_USER_NAME = "CarOwner_UserName";

        private const string SESSION_CAROWNER_EMAIL = "CarOwner_Email";

        private const string SESSION_CAROWNER_FULLNAME = "CarOwner_FullName";



        // Static variables for backward compatibility (deprecated - use Session instead)

        public static int _AccountId = 0;

        public static int _CarOwnerAccountId = 0; // AccountId của chủ xe (nếu Customer có Account với RoleId=3)

        public static string _UserName = String.Empty;

        public static string _Email = String.Empty;

        public static string _Message = string.Empty;

        public static string _MessageEmail = string.Empty;

        public static string _address = String.Empty;

        public static string _Phone = String.Empty;

        public static string _pickupDate = String.Empty;

        public static string _returnDate = String.Empty;

        public static string _Avatar = String.Empty;



        // --- CUSTOMER / GENERAL SESSION METHODS ---

        public static void SetAccountId(ISession session, int accountId)
        {
            session.SetInt32(SESSION_ACCOUNT_ID, accountId);
            session.SetInt32(SESSION_USER_ACCOUNT_ID, accountId);
            _AccountId = accountId;
        }



        public static int GetAccountId(ISession session)

        {

            return session.GetInt32(SESSION_ACCOUNT_ID) ?? 0;

        }

        public static void SetUserAccountId(ISession session, int userAccountId)
        {
            session.SetInt32(SESSION_USER_ACCOUNT_ID, userAccountId);
        }

        public static int GetUserAccountId(ISession session)
        {
            return session.GetInt32(SESSION_USER_ACCOUNT_ID) ?? session.GetInt32(SESSION_ACCOUNT_ID) ?? session.GetInt32(SESSION_CAROWNER_ID) ?? 0;
        }



        public static void SetCarOwnerAccountId(ISession session, int accountId)

        {

            session.SetInt32(SESSION_CAR_OWNER_ACCOUNT_ID, accountId);

            session.SetInt32(SESSION_CAROWNER_ID, accountId);

            _CarOwnerAccountId = accountId;

        }



        public static int GetCarOwnerAccountId(ISession session)
        {
            return session.GetInt32(SESSION_CAROWNER_ID) ?? session.GetInt32(SESSION_CAR_OWNER_ACCOUNT_ID) ?? session.GetInt32(SESSION_ACCOUNT_ID) ?? 0;
        }



        public static void SetUserName(ISession session, string userName)

        {

            session.SetString(SESSION_USER_NAME, userName ?? string.Empty);

            _UserName = userName ?? string.Empty;

        }



        public static string GetUserName(ISession session)

        {

            return session.GetString(SESSION_USER_NAME) ?? string.Empty;

        }



        public static void SetEmail(ISession session, string email)

        {

            session.SetString(SESSION_EMAIL, email ?? string.Empty);

            _Email = email ?? string.Empty;

        }



        public static string GetEmail(ISession session)

        {

            return session.GetString(SESSION_EMAIL) ?? string.Empty;

        }



        public static void SetAddress(ISession session, string address)

        {

            session.SetString(SESSION_ADDRESS, address ?? string.Empty);

            _address = address ?? string.Empty;

        }



        public static string GetAddress(ISession session)

        {

            return session.GetString(SESSION_ADDRESS) ?? string.Empty;

        }



        public static void SetPhone(ISession session, string phone)

        {

            session.SetString(SESSION_PHONE, phone ?? string.Empty);

            _Phone = phone ?? string.Empty;

        }



        public static string GetPhone(ISession session)

        {

            return session.GetString(SESSION_PHONE) ?? string.Empty;

        }



        public static void SetAvatar(ISession session, string avatar)

        {

            session.SetString(SESSION_AVATAR, avatar ?? string.Empty);

            _Avatar = avatar ?? string.Empty;

        }



        public static string GetAvatar(ISession session)

        {

            return session.GetString(SESSION_AVATAR) ?? _Avatar ?? string.Empty;

        }



        // --- ADMIN SCOPED METHODS ---

        public static void SetAdminSession(ISession session, int accountId, string userName, string email, string avatar = "")

        {

            session.SetInt32(SESSION_ADMIN_ACCOUNT_ID, accountId);

            session.SetString(SESSION_ADMIN_USER_NAME, userName ?? string.Empty);

            session.SetString(SESSION_ADMIN_EMAIL, email ?? string.Empty);

            SetAdminAvatar(session, avatar);



            // Đồng thời set fallback cho code cũ

            SetAccountId(session, accountId);

            SetUserName(session, userName ?? string.Empty);

            SetEmail(session, email ?? string.Empty);

            SetAvatar(session, avatar);

        }



        public static void SetAdminSession(ISession session, int accountId, string userName, string email)

        {

            SetAdminSession(session, accountId, userName, email, "");

        }



        public static void SetAdminAvatar(ISession session, string avatar)

        {

            session.SetString(SESSION_ADMIN_AVATAR, avatar ?? string.Empty);

            SetAvatar(session, avatar);

        }



        public static string GetAdminAvatar(ISession session)

        {

            return session.GetString(SESSION_ADMIN_AVATAR) ?? GetAvatar(session);

        }



        public static int GetAdminAccountId(ISession session)

        {

            return session.GetInt32(SESSION_ADMIN_ACCOUNT_ID) ?? (IsAdmin(session) ? GetAccountId(session) : 0);

        }



        public static string GetAdminUserName(ISession session)

        {

            return session.GetString(SESSION_ADMIN_USER_NAME) ?? (IsAdmin(session) ? GetUserName(session) : string.Empty);

        }



        public static bool IsAdmin(ISession session)

        {

            var adminId = session.GetInt32(SESSION_ADMIN_ACCOUNT_ID) ?? 0;

            if (adminId > 0) return true;



            var userName = GetUserName(session);

            return !string.IsNullOrEmpty(userName) && userName.Equals("Admin", StringComparison.OrdinalIgnoreCase);

        }



        public static void ClearAdminSession(ISession session)

        {

            session.Remove(SESSION_ADMIN_ACCOUNT_ID);

            session.Remove(SESSION_ADMIN_USER_NAME);

            session.Remove(SESSION_ADMIN_EMAIL);

            // Nếu session chung đang là Admin thì dọn luôn

            if (GetUserName(session).Equals("Admin", StringComparison.OrdinalIgnoreCase))

            {

                session.Remove(SESSION_ACCOUNT_ID);

                session.Remove(SESSION_USER_NAME);

                session.Remove(SESSION_EMAIL);

            }

        }



        // --- CAR OWNER SCOPED METHODS ---

        public static void SetCarOwnerSession(ISession session, int accountId, string userName, string email, string fullName = "")
        {
            session.SetInt32(SESSION_CAROWNER_ID, accountId);
            session.SetString(SESSION_CAROWNER_USER_NAME, userName ?? string.Empty);
            session.SetString(SESSION_CAROWNER_EMAIL, email ?? string.Empty);
            session.SetString(SESSION_CAROWNER_FULLNAME, fullName ?? string.Empty);
            session.SetInt32(SESSION_CAR_OWNER_ACCOUNT_ID, accountId);
            session.SetInt32(SESSION_ACCOUNT_ID, accountId);
            session.SetInt32(SESSION_USER_ACCOUNT_ID, accountId);
            session.SetString(SESSION_USER_NAME, !string.IsNullOrEmpty(fullName) ? fullName : (userName ?? string.Empty));
            session.SetString(SESSION_EMAIL, email ?? string.Empty);
            _AccountId = accountId;
        }



        public static string GetCarOwnerUserName(ISession session)

        {

            return session.GetString(SESSION_CAROWNER_USER_NAME) ?? string.Empty;

        }



        public static string GetCarOwnerFullName(ISession session)

        {

            var fullName = session.GetString(SESSION_CAROWNER_FULLNAME);

            return !string.IsNullOrEmpty(fullName) ? fullName : GetCarOwnerUserName(session);

        }



        public static string GetCarOwnerEmail(ISession session)

        {

            return session.GetString(SESSION_CAROWNER_EMAIL) ?? string.Empty;

        }



        public static bool IsCarOwnerLogin(ISession session)

        {

            return GetCarOwnerAccountId(session) > 0;

        }



        public static void ClearCarOwnerSession(ISession session)

        {

            session.Remove(SESSION_CAROWNER_ID);

            session.Remove(SESSION_CAROWNER_USER_NAME);

            session.Remove(SESSION_CAROWNER_EMAIL);

            session.Remove(SESSION_CAROWNER_FULLNAME);

            session.Remove(SESSION_CAR_OWNER_ACCOUNT_ID);

            _CarOwnerAccountId = 0;

        }



        public static void ClearCustomerSession(ISession session)

        {

            session.Remove(SESSION_ACCOUNT_ID);

            session.Remove(SESSION_USER_NAME);

            session.Remove(SESSION_EMAIL);

            session.Remove(SESSION_ADDRESS);

            session.Remove(SESSION_PHONE);

            session.Remove(SESSION_MESSAGE);

            session.Remove(SESSION_MESSAGE_EMAIL);

            session.Remove(SESSION_PICKUP_DATE);

            session.Remove(SESSION_RETURN_DATE);

            _AccountId = 0;

            _UserName = string.Empty;

            _Email = string.Empty;

        }



        public static void SetMessage(ISession session, string message)

        {

            session.SetString(SESSION_MESSAGE, message ?? string.Empty);

            _Message = message ?? string.Empty;

        }



        public static string GetMessage(ISession session)

        {

            return session.GetString(SESSION_MESSAGE) ?? string.Empty;

        }



        public static void ClearSession(ISession session)

        {

            session.Clear();

            _AccountId = 0;

            _CarOwnerAccountId = 0;

            _UserName = string.Empty;

            _Email = string.Empty;

            _Message = string.Empty;

            _MessageEmail = string.Empty;

            _address = string.Empty;

            _Phone = string.Empty;

            _pickupDate = string.Empty;

            _returnDate = string.Empty;

        }



        //public static string CART_KEY = "MYCART";

        public static string TitleSlugGenerationAlias(string title)

        {

            return SlugGenerator.SlugGenerator.GenerateSlug(title);

        }

        public static string MD5Hash(string text)

        {

            MD5 md5 = new MD5CryptoServiceProvider();

            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            byte[] result = md5.Hash;

            StringBuilder strbui = new StringBuilder();

            for (int i = 0; i < result.Length; i++)

            {

                strbui.Append(result[i].ToString("x2"));

            }

            return strbui.ToString();

        }

        public static string MD5Password(string? text)

        {

            string str = MD5Hash(text);

            //lap 5 lan ma hoa xau dam bao tinh bao mat

            //moi lan laao nhan doi xau, o giua them "_"

            for (int i = 0; i <= 5; i++)

            {

                str = MD5Hash(str + "_" + str);

            }

            return str;

        }

        public static bool IsLogin(ISession session = null)

        {

            // If session is provided, use Session; otherwise use static variables for backward compatibility

            if (session != null)

            {

                var accountId = GetAccountId(session);

                var userName = GetUserName(session);

                var email = GetEmail(session);

                return accountId > 0 && !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(email);

            }

            // Backward compatibility: use static variables

            if (string.IsNullOrEmpty(Function._UserName) || string.IsNullOrEmpty(Function._Email) || (Function._AccountId <= 0))

            {

                return false;

            }

            return true;     

        }

        /* public static string UploadImage(IFormFile image, string folder)

         {

             try

             {

                 var FullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", folder, image.FileName);

                 using (var myfile = new FileStream(FullPath, FileMode.CreateNew))

                 {

                     image.CopyTo(myfile);

                 }

                 return image.FileName;

             } catch(Exception ex)

             {

                 return string.Empty;

             }





         }*/



        public static string UploadImage(IFormFile image, string folder)

        {

            try

            {

                // Tạo tên file duy nhất bằng cách thêm timestamp

                var fileName = Path.GetFileNameWithoutExtension(image.FileName);

                var extension = Path.GetExtension(image.FileName);

                var uniqueFileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmssfff}{extension}";



                // Xây dựng đường dẫn đầy đủ để lưu file

                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", folder, uniqueFileName);



                // Kiểm tra và tạo thư mục nếu chưa tồn tại

                var directory = Path.GetDirectoryName(fullPath);

                if (!Directory.Exists(directory))

                {

                    Directory.CreateDirectory(directory);

                }



                // Lưu file vào thư mục

                using (var myFile = new FileStream(fullPath, FileMode.Create))

                {

                    image.CopyTo(myFile);

                }



                // Trả về tên file duy nhất

                return uniqueFileName;

            }

            catch (Exception ex)

            {

                // Log lỗi nếu cần

                Console.WriteLine($"Error uploading image: {ex.Message}");

                return string.Empty;

            }

        }

        public static string UploadAvatar(IFormFile? file)
        {
            if (file == null || file.Length == 0) return string.Empty;
            try
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                if (!allowedExts.Contains(ext)) return string.Empty;

                var fileName = $"avatar_{DateTime.Now:yyyyMMddHHmmssfff}_{Guid.NewGuid().ToString("N").Substring(0, 6)}{ext}";
                var root = Directory.GetCurrentDirectory();
                var avatarsDir = Path.Combine(root, "CarRental", "wwwroot", "files", "avatars");

                if (!Directory.Exists(avatarsDir))
                {
                    avatarsDir = Path.Combine(root, "wwwroot", "files", "avatars");
                    if (!Directory.Exists(avatarsDir))
                    {
                        Directory.CreateDirectory(avatarsDir);
                    }
                }

                var fullPath = Path.Combine(avatarsDir, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                return $"/files/avatars/{fileName}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading avatar: {ex.Message}");
                return string.Empty;
            }
        }

        public static string ResolveAvatarUrl(string? rawAvatar)
        {
            if (string.IsNullOrWhiteSpace(rawAvatar)) return string.Empty;
            var trimmed = rawAvatar.Trim();
            if (trimmed.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
                trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase) || 
                trimmed.StartsWith("/"))
            {
                return trimmed;
            }
            return $"/files/avatars/{trimmed}";
        }

        public static bool VerifyPassword(string inputPassword, string? storedPassword)
        {
            if (string.IsNullOrEmpty(storedPassword)) return true;
            if (string.IsNullOrEmpty(inputPassword)) return false;

            var hashed = MD5Password(inputPassword);
            return string.Equals(hashed, storedPassword, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(inputPassword, storedPassword);
        }



    

        /// <summary>

        /// Lấy thời gian hiện tại chuẩn múi giờ Việt Nam (UTC+7 / ICT)

        /// </summary>

        public static DateTime NowVn

        {

            get

            {

                try

                {

                    var vnTz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

                    return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTz);

                }

                catch

                {

                    try

                    {

                        var vnTz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");

                        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTz);

                    }

                    catch

                    {

                        return DateTime.UtcNow.AddHours(7);

                    }

                }

            }

        }



        /// <summary>

        /// Định dạng thời gian sang chuỗi 'HH:mm - dd/MM/yyyy'.

        /// Nếu DateTime chỉ lưu ngày (00:00), tự động dùng defaultTimeIfZeroHour (ví dụ '09:00' hoặc '18:00') để không bị lộ '00:00' nửa đêm.

        /// </summary>

        public static string FormatSafeDateTime(DateTime? dt, string defaultTimeIfZeroHour = "09:00")

        {

            if (!dt.HasValue) return "";

            var val = dt.Value;

            if (val.Hour == 0 && val.Minute == 0)

            {

                return $"{defaultTimeIfZeroHour} - {val:dd/MM/yyyy}";

            }

            return val.ToString("HH:mm - dd/MM/yyyy");

        }



        /// <summary>

        /// Trích xuất mốc giờ phút (HH:mm - dd/MM/yyyy) từ log trong Notes của đơn hàng.

        /// Hỗ trợ cả tag đơn lẻ lẫn danh sách các tag thay thế (không dấu, có dấu, viết tắt, ký tự lỗi encoding).

        /// </summary>

        public static string ExtractTimeFromNotes(string notes, string tag, string fallbackDateStr)

        {

            return ExtractTimeFromNotes(notes, new[] { tag }, fallbackDateStr);

        }



        public static string ExtractTimeFromNotes(string notes, string[] candidateTags, string fallbackDateStr)

        {

            if (string.IsNullOrEmpty(notes) || candidateTags == null || candidateTags.Length == 0) 

                return fallbackDateStr;



            try

            {

                var tagPatterns = candidateTags

                    .Where(t => !string.IsNullOrWhiteSpace(t))

                    .Select(System.Text.RegularExpressions.Regex.Escape)

                    .ToList();



                if (!tagPatterns.Any()) return fallbackDateStr;



                string patternGroup = string.Join("|", tagPatterns);



                // Khớp dạng: [... Tag ... dd/MM/yyyy HH:mm ...]

                var rx1 = new System.Text.RegularExpressions.Regex(

                    $@"\[[^\]]*(?:{patternGroup})[^\]]*?(\d{{2}}/\d{{2}}/\d{{4}})\s+(\d{{2}}:\d{{2}})[^\]]*\]",

                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                var match1 = rx1.Match(notes);

                if (match1.Success)

                {

                    return $"{match1.Groups[2].Value} - {match1.Groups[1].Value}";

                }



                // Khớp dạng: [... Tag ... HH:mm dd/MM/yyyy ...]

                var rx2 = new System.Text.RegularExpressions.Regex(

                    $@"\[[^\]]*(?:{patternGroup})[^\]]*?(\d{{2}}:\d{{2}})\s*[-:]?\s*(\d{{2}}/\d{{2}}/\d{{4}})[^\]]*\]",

                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                var match2 = rx2.Match(notes);

                if (match2.Success)

                {

                    return $"{match2.Groups[1].Value} - {match2.Groups[2].Value}";

                }



                // Khớp dạng chung trong cả đoạn note gần từ khóa

                var rx3 = new System.Text.RegularExpressions.Regex(

                    $@"(?:{patternGroup})[^\d


]*?(\d{{2}}/\d{{2}}/\d{{4}})\s+(\d{{2}}:\d{{2}})",

                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                var match3 = rx3.Match(notes);

                if (match3.Success)

                {

                    return $"{match3.Groups[2].Value} - {match3.Groups[1].Value}";

                }

            }

            catch { }



            return fallbackDateStr;

        }

        /// <summary>
        /// Trích xuất hình thức lái, hình thức giao nhận xe và phí giao xe từ ghi chú đơn hàng.
        /// </summary>
        public static (string DriveMode, string DeliveryMode, decimal DeliveryFee) ParseOrderDeliveryInfo(string? notes)
        {
            if (string.IsNullOrWhiteSpace(notes))
                return ("Tự lái", "Nhận tại địa chỉ xe", 0m);

            string driveMode = (notes.Contains("Có tài xế", StringComparison.OrdinalIgnoreCase) ||
                                notes.Contains("Kèm tài xế", StringComparison.OrdinalIgnoreCase))
                ? "Có tài xế"
                : "Tự lái";

            string deliveryMode = notes.Contains("Giao xe tận nơi", StringComparison.OrdinalIgnoreCase)
                ? "Giao xe tận nơi"
                : "Nhận tại địa chỉ xe";

            decimal deliveryFee = 0m;
            if (deliveryMode == "Giao xe tận nơi")
            {
                var m = System.Text.RegularExpressions.Regex.Match(
                    notes,
                    @"Phí giao xe:\s*([\d\.,]+)",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    var digits = m.Groups[1].Value.Replace(".", "").Replace(",", "");
                    if (decimal.TryParse(digits, out var parsed))
                    {
                        deliveryFee = parsed;
                    }
                }
            }

            return (driveMode, deliveryMode, deliveryFee);
        }

        /// <summary>
        /// Trích xuất thông tin đơn đang chờ thanh toán ZaloPay (hạn thanh toán & link thanh toán).
        /// </summary>
        public static (bool IsPendingPayment, DateTime? ExpireAt, string? PayUrl) ParseZaloPayPending(string? notes)
        {
            if (string.IsNullOrWhiteSpace(notes))
                return (false, null, null);

            var expMatch = System.Text.RegularExpressions.Regex.Match(
                notes,
                @"\[ZaloPayPending:(\d{14})\]",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (!expMatch.Success)
                return (false, null, null);

            DateTime? expireAt = null;
            if (DateTime.TryParseExact(expMatch.Groups[1].Value, "yyyyMMddHHmmss",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var parsedDt))
            {
                expireAt = parsedDt;
            }

            string? payUrl = null;
            var urlMatch = System.Text.RegularExpressions.Regex.Match(
                notes,
                @"\[ZaloPayUrl:([^\]]+)\]",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (urlMatch.Success)
            {
                payUrl = urlMatch.Groups[1].Value.Trim();
            }

            return (true, expireAt, payUrl);
        }

        /// <summary>
        /// Hủy các đơn hàng đang chờ thanh toán ZaloPay (StatusId == 1) đã quá hạn thanh toán và mở lại xe.
        /// </summary>
        public static int CancelExpiredPendingOrders(Models.DbRenalCarContext db)
        {
            var nowVn = NowVn;
            var pendingOrders = db.CarRentalOrders
                .Where(o => o.StatusId == 1 && o.Notes != null && o.Notes.Contains("[ZaloPayPending:"))
                .ToList();

            if (pendingOrders.Count == 0) return 0;

            int cancelledCount = 0;
            var carIdsToReactivate = new List<int>();

            foreach (var ord in pendingOrders)
            {
                var info = ParseZaloPayPending(ord.Notes);
                if (info.IsPendingPayment && info.ExpireAt.HasValue && nowVn >= info.ExpireAt.Value)
                {
                    ord.StatusId = 2;
                    ord.Notes = (ord.Notes ?? "") + $" | [Hệ thống {nowVn:dd/MM/yyyy HH:mm}] Tự động hủy do quá hạn thanh toán 1 phút";
                    var details = db.OrderDetails.Where(d => d.OrderId == ord.OrderId).Select(d => d.CarId).ToList();
                    carIdsToReactivate.AddRange(details);
                    cancelledCount++;
                }
            }

            if (cancelledCount > 0)
            {
                if (carIdsToReactivate.Count > 0)
                {
                    var cars = db.Cars.Where(c => carIdsToReactivate.Contains(c.CarId)).ToList();
                    foreach (var c in cars)
                    {
                        c.IsActive = true;
                    }
                }
                db.SaveChanges();
            }

            return cancelledCount;
        }

    }

}

