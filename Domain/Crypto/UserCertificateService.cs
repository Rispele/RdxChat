using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Domain.Crypto;

public class UserCertificateService
{
    public const string UserIdOid = "1.2.840.113549.1.9.2";

    /// <summary>
    /// Получает сертификат из личных сертификатов локального компьютера по его отпечатку
    /// </summary>
    /// <param name="thumbprint">Регистронезависимый</param>
    /// <returns></returns>
    /// <exception cref="Exception">Если сертификат не найден</exception>
    public X509Certificate2 GetCertificate(string thumbprint)
    {
        var thumbprintUppercased = thumbprint.ToUpper();

        using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
        store.Open(OpenFlags.ReadOnly);
        return store.Certificates.FirstOrDefault(c => c.Thumbprint == thumbprintUppercased)
         ?? throw new Exception($"Certificate with thumbprint {thumbprint} not found");
    }

    /// <summary>
    /// Создать сертификат и добавить в его расширения userId
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public X509Certificate2 GenerateUserCertificate(Guid userId)
    {
        using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);

        var request = CreateCertificateRequest(userId, ecdsa);

        var notBefore = DateTimeOffset.UtcNow;
        var notAfter = notBefore.AddYears(10);

        return request.CreateSelfSigned(notBefore, notAfter);
    }

    private CertificateRequest CreateCertificateRequest(Guid userId, ECDsa ecdsa)
    {
        var request = new CertificateRequest($"CN=certificate-{userId}", ecdsa, HashAlgorithmName.SHA256);

        request.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(
                certificateAuthority: false,
                hasPathLengthConstraint: false,
                pathLengthConstraint: 0,
                critical: false));
        request.CertificateExtensions.Add(
            new X509KeyUsageExtension(
                X509KeyUsageFlags.DigitalSignature, 
                critical: true));
        request.CertificateExtensions.Add(
            new X509SubjectKeyIdentifierExtension(
                request.PublicKey, 
                critical: false));
        request.CertificateExtensions.Add(
            new X509Extension(
                Oid.FromOidValue(UserIdOid, OidGroup.All),
                userId.ToByteArray(),
                critical: true));
        return request;
    }
}