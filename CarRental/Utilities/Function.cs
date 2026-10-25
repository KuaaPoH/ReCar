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

        // --- CUSTOMER / GENERAL SESSION METHODS ---
        public static void SetAccountId(ISession session, int accountId)
        {
            session.SetInt32(SESSION_ACCOUNT_ID, accountId);
            _AccountId = accountId;
        }

        public static int GetAccountId(ISession session)
        {
            return session.GetInt32(SESSION_ACCOUNT_ID) ?? 0;
        }

        public static void SetCarOwnerAccountId(ISession session, int accountId)
        {
            session.SetInt32(SESSION_CAR_OWNER_ACCOUNT_ID, accountId);
            session.SetInt32(SESSION_CAROWNER_ID, accountId);
            _CarOwnerAccountId = accountId;
        }

        public static int GetCarOwnerAccountId(ISession session)
        {
            return session.GetInt32(SESSION_CAROWNER_ID) ?? session.GetInt32(SESSION_CAR_OWNER_ACCOUNT_ID) ?? 0;
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

        // --- ADMIN SCOPED METHODS ---
        public static void SetAdminSession(ISession session, int accountId, string userName, string email)
        {
            session.SetInt32(SESSION_ADMIN_ACCOUNT_ID, accountId);
            session.SetString(SESSION_ADMIN_USER_NAME, userName ?? string.Empty);
            session.SetString(SESSION_ADMIN_EMAIL, email ?? string.Empty);
            // Đồng thời set fallback cho code cũ
            SetAccountId(session, accountId);
            SetUserName(session, userName ?? string.Empty);
            SetEmail(session, email ?? string.Empty);
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

    }
}
