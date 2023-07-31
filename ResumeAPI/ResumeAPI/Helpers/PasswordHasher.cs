using System.Security.Cryptography;

namespace ResumeAPI.Helpers;

public class PasswordHasher
{
    public static byte[] GenerateSalt(int byteLength)
    {
        using (var cryptoServiceProvider = new RNGCryptoServiceProvider())
        {
            var data = new byte[byteLength];
            cryptoServiceProvider.GetBytes(data);
            return data;
        }
    }
}