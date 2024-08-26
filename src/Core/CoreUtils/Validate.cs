namespace Core.CoreUtils
{
    using System.Text.RegularExpressions;

    public abstract class Validate
    {
        public static bool IsValidPassword(string password)
        {
            if (password.Length < 8)
                return false;

            bool hasLowerCase = Regex.IsMatch(password, @"[a-z]");
            bool hasUpperCase = Regex.IsMatch(password, @"[A-Z]");
            bool hasDigit = Regex.IsMatch(password, @"\d");
            bool hasSpecialChar = Regex.IsMatch(password, @"[@$!%*?&]");

            return hasLowerCase && hasUpperCase && hasDigit && hasSpecialChar;
        }
        //public static bool ValidatePassword(string password)
        //{
        //    if(string.IsNullOrWhiteSpace(password)) return false;
        //    // Define the regex pattern for the password
        //    var passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";

        //    // Create a regex object with the pattern
        //    var regex = new Regex(passwordPattern);

        //    // Validate the password against the pattern
        //    return regex.IsMatch(password);
        //}
        public static bool ValidateChuHoa(string strIn)
        {
            return Regex.IsMatch(strIn, @".*[A-Z].*");
        }
        public static bool ValidateSo(string strIn)
        {
            return Regex.IsMatch(strIn, @".*[0-9].*");
        }
        public static bool ValidateChuThuong(string strIn)
        {
            return Regex.IsMatch(strIn, @".*[a-z].*");
        }
        public static bool ValidateKyTuDacBiet(string strIn)
        {
            return Regex.IsMatch(strIn, @".*[`!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?~].*");
        }
        public static bool ValidateCMND(string str)
        {
            return Regex.IsMatch(str, @"^[0-9]{9,12}$");
        }

        public static string ValidPhoneNumber(ref string phoneNumber)
        {
            phoneNumber = Utils.ReplaceWhitespace(phoneNumber?.Trim(), "");
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return "Vui lòng nhập số điện thoại!";
            }
            if (!ValidateSo(phoneNumber))
            {
                return "Số điện thoại chỉ được phép nhập số!";
            }
            if (phoneNumber.StartsWith("+84"))
            {
                phoneNumber = phoneNumber.Replace("+84", "0");
            }
            if (phoneNumber.StartsWith("84"))
            {
                phoneNumber = "0" + phoneNumber.Substring(2);
            }
            if (phoneNumber.Length < 10)
                return "Số điện thoại đang <10 số!";
            if (phoneNumber.Length > 12)
                return "Số điện thoại đang >11 số!";
            if (!IsValidPhone(phoneNumber))
                return "Số điện thoại không đúng định dạng!";
            return null;
        }

        public static bool ValidEmail(string strEmail)
        {
            Regex rgxEmail = new Regex(@"^([\w]*)([@]?)([\w]*)([.]?)([\w]{1,3})$");

            return rgxEmail.IsMatch(strEmail);
        }



        /// <summary>
        /// Determines whether [is unicode string] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if [is unicode string] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUnicodeString(string value)
        {
            string characterUnicode = "áảàạãăắẳằặẵâấẩầậẫóỏòọõôốổồộỗơớởờợỡéèẻẹẽêếềểệễúùủụũưứừửựữíìỉịĩýỳỷỵỹđ";

            if (characterUnicode.IndexOf(value.ToString()) > -1)
            {
                return true;
            }

            return false;
        }
        public static bool IsValidPhone(string phone)
        {
            Regex phoneRegex = new Regex(@"^(\+84|84|0)+([0-9]{9,10})*$");
            Match m = phoneRegex.Match(phone);
            return m.Success;
        }

        public static string IsValidateName(string input)
        {
            // Regular Expression pattern to check if the string contains only letters and spaces
            string pattern = @"^[a-zA-Z\s]+$";
            if (string.IsNullOrEmpty(input)) return "Họ tên không thể trống";
            else if (input.Length < 3 || input.Length > 100) return "Họ tên phải có độ dài từ 3-100 kí tự.";
            else if (!Regex.IsMatch(input, pattern))
            {
                return "Họ tên không được phép nhập số hoặc ký tự đặc biệt";
            }
            return null;
        }
    }
}

