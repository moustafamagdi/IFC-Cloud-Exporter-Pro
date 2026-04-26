using System.Security.Cryptography;
using System.Text;

namespace ACCIFCConverter.Infrastructure.Security;

public sealed class DpapiSecretProtector
{
    public string Protect(string plainText)
    {
        var data = Encoding.UTF8.GetBytes(plainText);
        var encrypted = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(encrypted);
    }

    public string Unprotect(string cipher)
    {
        var data = Convert.FromBase64String(cipher);
        var decrypted = ProtectedData.Unprotect(data, null, DataProtectionScope.CurrentUser);
        return Encoding.UTF8.GetString(decrypted);
    }
}
