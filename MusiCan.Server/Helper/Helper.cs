using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Components.Forms;

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
        // 1234 > "jLhHfUwAZ3laEdzj1tCBpIk3w0vJe5Lk/7+yc3jdoMNE6qh8qAhZZNNmIAK1TQOLQCwhIXAnFXq3r5y2YSHoAQ=="

        /// <summary>
        /// compares input password with db stored pw and salt
        /// </summary>
        /// <param name="input">input Password gets Salted and compared</param>
        /// <param name="hashString">salted password from the db</param>
        /// <param name="salt">salt from the db, used to salt the input and comapre against the hash</param>
        /// <returns>True if derived Hash equals stored Hash</returns>
        public static bool VerifyHash(string input, string hashString, byte[] salt)
        {
            byte[] inputHash = Convert.FromBase64String(input);
            byte[] hash = Convert.FromBase64String(hashString);
            byte[] hashedPW = Rfc2898DeriveBytes.Pbkdf2(
                inputHash,
                salt,
                _iterations,
                _algorithm,
                _keySize
            );
            return CryptographicOperations.FixedTimeEquals(hashedPW, hash);
        }

        public static string GenerateSaltedPassword(string input, string input_salt)
        {
            byte[] salt = Encoding.UTF8.GetBytes(input_salt);
            byte[] hashedPW = Rfc2898DeriveBytes.Pbkdf2(
                input,
                salt,
                _iterations,
                _algorithm,
                _keySize
            );
            return Convert.ToBase64String(hashedPW);
        }

        /// <summary>
        /// überprüft ob ein String ein DeviceHash ist
        /// </summary>
        /// <param name="input">String zum testen</param>
        /// <returns>True wenn input ein DeviceHash ist</returns>
        public static bool IsHash(string input)
        {
            byte[] input_hash = Convert.FromBase64String(input);

            if (input_hash.Length != _keySize)
            {
                return false;
            }

            return true;
        }
    }

    //Helper Klasse die Helper Funktionen beinhaltet
    public static class Helper
    {
        public static long ToWindowsSystemTime(this DateTime? utcTime)
        {
            if(utcTime == null)
            {
                return -1;
            }

            DateTime startTime = new(1601,1,1,0,0,0,DateTimeKind.Utc);

            TimeSpan difference = utcTime.Value - startTime;

            //windows Zeit in 100ns ticks
            const long TicksPerSecond = 10000000;

            return (long)difference.TotalSeconds * TicksPerSecond;
        }

        public static long ToWindowsSystemTime(this DateTime utcTime)
        {
            DateTime startTime = new(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            TimeSpan difference = utcTime - startTime;

            //windows Zeit in 100ns ticks
            const long TicksPerSecond = 10000000;

            return (long)difference.TotalSeconds * TicksPerSecond;
        }
    }
}
