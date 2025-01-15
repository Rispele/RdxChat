using System.Security.Cryptography.Pkcs;

namespace Domain.Crypto;

public static class SignedCmsHelper
{
    public static SignedCms FromSignature(byte[] signature, byte[]? content = null)
    {
        ArgumentNullException.ThrowIfNull(signature);
        
        var signedCms = content is null 
            ? new SignedCms()
            : new SignedCms(new ContentInfo(content), detached: true);

        signedCms.Decode(signature);

        return signedCms;
    }
}