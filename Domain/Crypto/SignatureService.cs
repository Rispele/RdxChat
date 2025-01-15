using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;

namespace Domain.Crypto;

public class SignatureService
{
    public byte[] Sign(X509Certificate2 certificate, byte[] content, bool attached = false)
    {
        var cms = new SignedCms(new ContentInfo(content), detached: !attached);
        cms.ComputeSignature(new CmsSigner(certificate), silent: false);
        return cms.Encode();
    }

    public string? VerifySignature(byte[] signature, byte[]? content = null)
    {
        try
        {
            var signedData = SignedCmsHelper.FromSignature(signature, content);
            signedData.CheckSignature(verifySignatureOnly: true);
        }
        catch (Exception e)
        {
            return e.Message;
        }

        return null;
    }
}