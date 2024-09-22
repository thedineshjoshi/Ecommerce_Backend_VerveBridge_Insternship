using System.Text;

namespace Ecommerce_Final.Common
{
    public static class CommonMethods
    {
        public static string Key = "aefsadnhf@fsjdnfgiwe@ijiasn@";

        public static string ConvertToEncrypt(string password)
        {
            if (string.IsNullOrEmpty(password)) return "";
            password += Key;
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(passwordBytes);
        }

        public static string ConvertToDecrypt(string base64EncodedData)
        {
            if (string.IsNullOrEmpty(base64EncodedData)) return "";
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            var result = Encoding.UTF8.GetString(base64EncodedBytes);
            result = result.Substring(0, result.Length - Key.Length);
            return result;
        }
    }
}
