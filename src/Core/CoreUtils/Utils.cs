using Core.CommonModels.BaseModels;
using Core.Enums;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;


namespace Core.CoreUtils
{
    /// <summary>
    /// Class provide common APIs that are relate to orders. 
    /// </summary>
    public static class Utils
    {
        #region Private Declare Model
        public static string secretKey = "auth_api_3sdf43rc11239hsdcnsc0esdcsd!asd0023";//line 1
        // Set your salt here, change it to meet your flavor:
        // The salt bytes must be at least 8 bytes.
        private static byte[] saltBytes = new byte[] { 5, 6, 1, 7, 4, 2, 8, 9 };
        private static readonly Regex sWhitespace = new Regex(@"\s+");
        private readonly static string[] unicodeToHop = new string[] {
            "a","â","ă","e","ê","i","o","ô","ơ","u","ư","y","A","Â","Ă","E","Ê","I","O","Ô","Ơ","U","Ư","Y","á","ấ","ắ","é","ế","í","ó","ố","ớ","ú","ứ","ý","Á","Ấ","Ắ","É","Ế","Í","Ó","Ố","Ớ","Ú","Ứ","Ý","à","ầ","ằ","è","ề","ì","ò","ồ","ờ","ù","ừ","ỳ","À","Ầ","Ằ","È","Ề","Ì","Ò","Ồ","Ờ","Ù","Ừ","Ỳ","ạ","ậ","ặ","ẹ","ệ","ị","ọ","ộ","ợ","ụ","ự","ỵ","Ạ","Ậ","Ặ","Ẹ","Ệ","Ị","Ọ","Ộ","Ợ","Ụ","Ự","Ỵ","ả","ẩ","ẳ","ẻ","ể","ỉ","ỏ","ổ","ở","ủ","ử","ỷ","Ả","Ẩ","Ẳ","Ẻ","Ể","Ỉ","Ỏ","Ổ","Ở","Ủ","Ử","Ỷ","ã","ẫ","ẵ","ẽ","ễ","ĩ","õ","ỗ","ỡ","ũ","ữ","ỹ","Ã","Ẫ","Ẵ","Ẽ","Ễ","Ĩ","Õ","Ỗ","Ỡ","Ũ","Ữ","Ỹ","d","đ","D","Đ"
        };

        private readonly static string[] unicode = new string[] {
            "a","â","ă","e","ê","i","o","ô","ơ","u","ư","y","A","Â","Ă","E","Ê","I","O","Ô","Ơ","U","Ư","Y","á","ấ","ắ","é","ế","í","ó","ố","ớ","ú","ứ","ý","Á","Ấ","Ắ","É","Ế","Í","Ó","Ố","Ớ","Ú","Ứ","Ý","à","ầ","ằ","è","ề","ì","ò","ồ","ờ","ù","ừ","ỳ","À","Ầ","Ằ","È","Ề","Ì","Ò","Ồ","Ờ","Ù","Ừ","Ỳ","ạ","ậ","ặ","ẹ","ệ","ị","ọ","ộ","ợ","ụ","ự","ỵ","Ạ","Ậ","Ặ","Ẹ","Ệ","Ị","Ọ","Ộ","Ợ","Ụ","Ự","Ỵ","ả","ẩ","ẳ","ẻ","ể","ỉ","ỏ","ổ","ở","ủ","ử","ỷ","Ả","Ẩ","Ẳ","Ẻ","Ể","Ỉ","Ỏ","Ổ","Ở","Ủ","Ử","Ỷ","ã","ẫ","ẵ","ẽ","ễ","ĩ","õ","ỗ","ỡ","ũ","ữ","ỹ","Ã","Ẫ","Ẵ","Ẽ","Ễ","Ĩ","Õ","Ỗ","Ỡ","Ũ","Ữ","Ỹ","d","đ","D","Đ"
        };
        #endregion

        #region Utils

        public static LunarDate ConvertSolarToLunar(DateTime solarDate)
        {
            ChineseLunisolarCalendar chineseCalendar = new ChineseLunisolarCalendar();

            int lunarYear = chineseCalendar.GetYear(solarDate);
            int lunarMonth = chineseCalendar.GetMonth(solarDate);
            int lunarDay = chineseCalendar.GetDayOfMonth(solarDate);
            int leapMonth = chineseCalendar.GetLeapMonth(lunarYear);

            bool isLeapMonth = false;
            if (leapMonth > 0 && lunarMonth == leapMonth)
            {
                isLeapMonth = true;
                lunarMonth--; // Adjust to 1-based month for non-leap months
            }
            else if (leapMonth > 0 && lunarMonth > leapMonth)
            {
                lunarMonth--; // Adjust to 1-based month for non-leap months
            }

            return new LunarDate(lunarYear, lunarMonth, lunarDay, isLeapMonth);
        }

        /// <summary>
        /// Converts the day of week.
        /// </summary>
        /// <param name="dayOfWeek">The day of week.</param>
        /// <returns></returns>
        public static int ConvertDayOfWeek(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    return 2;
                case DayOfWeek.Tuesday:
                    return 3;
                case DayOfWeek.Wednesday:
                    return 4;
                case DayOfWeek.Thursday:
                    return 5;
                case DayOfWeek.Friday:
                    return 6;
                case DayOfWeek.Saturday:
                    return 7;
                case DayOfWeek.Sunday:
                    return 8;
            }

            return 0;
        }

        /// <summary>
        /// Removes the redundant character.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="redundantCharacter">The redundant character.</param>
        /// <returns></returns>
        public static string RemoveRedundantCharacter(string value, string redundantCharacter)
        {
            if (!string.IsNullOrEmpty(value))
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (i + 1 < value.Length && value[i].Equals(value[i + 1]))
                    {
                        value = value.Remove(i, 1);
                        i = i - 1;
                    }
                }
                return value;
            }
            return string.Empty;
        }

        /// <summary>
        /// Convert Unicode to NonUnicode
        /// </summary>
        /// <param name="text">the text</param>
        /// <returns></returns>
        public static string NonUnicode(this string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            string[] arr1 = new string[] {
                "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
                "đ", "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ","í","ì","ỉ","ĩ","ị","ó","ò","ỏ","õ",
                "ọ", "ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ","ú","ù","ủ","ũ","ụ","ư","ứ","ừ",
                "ử", "ữ","ự","ý","ỳ","ỷ","ỹ","ỵ",};
            string[] arr2 = new string[] {
                "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
                "d", "e","e","e","e","e","e","e","e","e","e","e","i","i","i","i","i","o","o","o","o",
                "o","o","o","o","o","o","o","o","o","o","o","o","o","u","u","u","u","u","u","u","u",
                "u","u","u","y","y","y","y","y",};
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return text;
        }

        public static string ToUnsignedLowerText(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            text = text.ToLower();
            string stFormD = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            sb = sb.Replace('đ', 'd');
            return sb.ToString().Normalize(NormalizationForm.FormD);
        }

        public static string RemoveSpecialCharacters(string str)
        {
            try
            {
                return Regex.Replace(str, "[^a-zA-Z0-9_. ]+", "", RegexOptions.Compiled);
            }
            catch (Exception)
            {
                return str;
                throw;
            }
        }
        public static string ReplaceWhitespace(string input, string replacement)
        {
            return sWhitespace.Replace(input, replacement);
        }

        public static int GetAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;

            var a = (today.Year * 100 + today.Month) * 100 + today.Day;
            var b = (dateOfBirth.Year * 100 + dateOfBirth.Month) * 100 + dateOfBirth.Day;

            return (a - b) / 10000;
        }

        public static string GetMacAddress()
        {
            string macAddresses = string.Empty;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }

            return macAddresses;
        }

        public static double GetDistance(decimal longitude, decimal latitude, decimal otherLongitude, decimal otherLatitude)
        {
            var d1 = latitude * (decimal)(Math.PI / 180.0);
            var num1 = longitude * (decimal)(Math.PI / 180.0);
            var d2 = otherLatitude * (decimal)(Math.PI / 180.0);
            var num2 = otherLongitude * (decimal)(Math.PI / 180.0) - num1;
            var d3 = Math.Pow(Math.Sin((double)(d2 - d1) / 2.0), 2.0) + Math.Cos((double)d1) * Math.Cos((double)d2) * Math.Pow(Math.Sin((double)num2 / 2.0), 2.0);

            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));//met
        }

        public static byte[] DownloadFile(string fileNamePath)
        {
            try
            {
                byte[] fileData = null;

                if (File.Exists(fileNamePath))
                {
                    byte[] imageBytes = File.ReadAllBytes(fileNamePath);
                    return imageBytes;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        /// <summary>
        /// Compare 2 Vietnamese name
        /// </summary>
        /// <param name="a"> First name.</param>
        /// <param name="b">The Second name</param>
        /// <returns> </returns>
        public static bool CompareVietNameseName(string a, string b)
        {
            //remove all any char is not letter or digit
            a = new string(a.Where(c => char.IsLetterOrDigit(c)).ToArray()).ToLower();
            b = new string(b.Where(c => char.IsLetterOrDigit(c)).ToArray()).ToLower();
            if (a == b)
                return true;
            return false;
        }


        public static string convertVietNameseText(string input)
        {
            // dấu unicode tổ hợp regex 
            if (!Regex.Match(input, @"[̣́̀̉̃]").Success)
                return input;
            for (int i = 0; i < unicodeToHop.Length; i++)
                input = Regex.Replace(input, unicodeToHop[i], unicode[i]);
            return input;
        }

        public static string RemoveSignAndLowerCaseVietnameseString(string str)
        {
            try
            {
                if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
                    return "";

                string[] VietnameseSigns = new string[]
                {

                    "aAeEoOuUiIdDyY",
                    "áàạảãâấầậẩẫăắằặẳẵ",
                    "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
                    "éèẹẻẽêếềệểễ",
                    "ÉÈẸẺẼÊẾỀỆỂỄ",
                    "óòọỏõôốồộổỗơớờợởỡ",
                    "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
                    "úùụủũưứừựửữ",
                    "ÚÙỤỦŨƯỨỪỰỬỮ",
                    "íìịỉĩ",
                    "ÍÌỊỈĨ",
                    "đ",
                    "Đ",
                    "ýỳỵỷỹ",
                    "ÝỲỴỶỸ"
                };
                str = str.Trim().ToLower();
                for (int i = 1; i < VietnameseSigns.Length; i++)
                    for (int j = 0; j < VietnameseSigns[i].Length; j++)
                        str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
                return str;
            }
            catch (Exception)
            {
                return str;
                throw;
            }
        }

        public static bool ContainsSpecialCharacter(string input)
        {
            // Biểu thức chính quy để tìm ký tự đặc biệt
            string specialCharPattern = @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>/?]";

            // Kiểm tra xem chuỗi có khớp với biểu thức chính quy không
            return Regex.IsMatch(input, specialCharPattern);
        }

        #endregion


        #region Encrypt
        public static string MD5Encrypt(string strValue)
        {
            byte[] data, output;
            UTF8Encoding encoder = new UTF8Encoding();
            MD5CryptoServiceProvider hasher = new MD5CryptoServiceProvider();

            data = encoder.GetBytes(strValue);
            output = hasher.ComputeHash(data);

            return BitConverter.ToString(output).Replace("-", "").ToLower();
        }

        public static string EncodePassword(string pass, string encodeType)
        {
            if (string.IsNullOrEmpty(encodeType) || encodeType.ToLower() == "sha256")
                return EncodePassword(pass, EEncodeType.SHA_256);

            return "encode type " + encodeType + " not support";
        }

        public static string DecodePassword(string pass, string encodeType)
        {
            if (string.IsNullOrEmpty(encodeType) || encodeType.ToLower() == "sha256")
                return DecodePassword(pass, EEncodeType.SHA_256);

            return "encode type " + encodeType + " not support";
        }

        public static string EncodePassword(string pass, EEncodeType encodeType)
        {
            switch (encodeType)
            {
                case EEncodeType.SHA_256:
                    return EncryptPassSHA256(pass);
                default:
                    return "encode type " + encodeType.ToString() + " not support";
            }
        }

        public static string DecodePassword(string encodePass, EEncodeType encodeType)
        {
            switch (encodeType)
            {
                case EEncodeType.SHA_256:
                    return DecryptPassSHA256(encodePass);
                default:
                    return "encode type " + encodeType.ToString() + " not support";
            }
        }

        private static string EncryptPassSHA256(string pass)
        {
            try
            {
                //string secretKey = Util.secretKey;

                var bytesToBeEncrypted = Encoding.UTF8.GetBytes(pass);
                var passwordBytes = Encoding.UTF8.GetBytes(secretKey);

                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                var bytesEncrypted = EncryptSHA256(bytesToBeEncrypted, passwordBytes);
                return Convert.ToBase64String(bytesEncrypted);
            }
            catch (Exception ex)
            {
                return ex.Message + ex.StackTrace;
            }
        }

        private static string DecryptPassSHA256(string encryptedText)
        {
            try
            {
                //string secretKey = Util.secretKey;
                // Get the bytes of the string
                var bytesToBeDecrypted = Convert.FromBase64String(encryptedText);
                var passwordBytes = Encoding.UTF8.GetBytes(secretKey);

                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                var bytesDecrypted = DecryptSHA256(bytesToBeDecrypted, passwordBytes);

                return Encoding.UTF8.GetString(bytesDecrypted);
            }
            catch (Exception ex)
            {
                return ex.Message + ex.StackTrace;
            }
        }

        public static byte[] EncryptSHA256(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;
                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }

                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        public static byte[] DecryptSHA256(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }

                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }
        public static ELevelRole MapSourceToTarget(this ERoleType source)
        {
            switch (source)
            {
                case ERoleType.SystemAdmin:
                    return ELevelRole.SystemAdmin;
                case ERoleType.Admin:
                    return ELevelRole.Admin;
                case ERoleType.Reporter:
                    return ELevelRole.Reporter;
                default:
                    return ELevelRole.User; // Handle unmapped cases
            }
        }
        public static int GetMaxLevelRole(List<ERoleType> currentUserRoleId)
        {
            return (int)currentUserRoleId.Select(i => i.MapSourceToTarget()).Max();
        }
        #endregion
    }
}
