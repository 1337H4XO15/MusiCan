using System.Text;
using System.Security.Cryptography;

namespace MusiCan.Server.Helper
{
    /// <summary>
    /// DeviceHash:
    /// string password = "...";
    /// string hashed = SecretHasher.DeviceHash(password);
    /// 
    /// Verify:
    /// string enteredPassword = "...";
    /// bool isPasswordCorrect = SecretHasher.Verify(enteredPassword, hashed);
    /// 
    /// https://stackoverflow.com/questions/2138429/hash-and-salt-passwords-in-c-sharp
    /// </summary>
    public class SecretHasher
    {
        private const int _keySize = 64; // 512 bits
        private const int _iterations = 10000;
        private static readonly HashAlgorithmName _algorithm = HashAlgorithmName.SHA512;
        private static byte[] _salt = Encoding.UTF8.GetBytes("TnFWNhg3Abxtkp1u");
        // 1234 > "jLhHfUwAZ3laEdzj1tCBpIk3w0vJe5Lk/7+yc3jdoMNE6qh8qAhZZNNmIAK1TQOLQCwhIXAnFXq3r5y2YSHoAQ=="

        /// <summary>
        /// compares input password with db stored pw and salt
        /// </summary>
        /// <param Name="input">input password gets Salted and compared</param>
        /// <param Name="hashString">salted password from the db</param>
        /// <returns>True if derived Hash equals stored Hash</returns>
        public static bool VerifyHash(string input, string hashString)
        {
            byte[] inputPwd = Encoding.UTF8.GetBytes(input);
            byte[] hash = Convert.FromBase64String(hashString);
            byte[] hashedPW = Rfc2898DeriveBytes.Pbkdf2(
                inputPwd,
                _salt,
                _iterations,
                _algorithm,
                _keySize
            );
            return CryptographicOperations.FixedTimeEquals(hashedPW, hash);
        }

        public static string Hash(string input_s)
        {
            byte[] input = Encoding.UTF8.GetBytes(input_s);
            byte[] hashedPW = Rfc2898DeriveBytes.Pbkdf2(
                input,
                _salt,
                _iterations,
                _algorithm,
                _keySize
            );
            return Convert.ToBase64String(hashedPW);
        }
    }
}
