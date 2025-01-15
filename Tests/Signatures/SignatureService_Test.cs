using System.Security.Cryptography.X509Certificates;
using Domain.Crypto;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Signatures;

[TestFixture]
public class SignatureService_Test
{
    private static readonly string Thumbprint = "995a365904eb18cd7eebfac6c6b78eb2fa58efb7".ToUpper();
    private static readonly byte[] Content = "Hello, World!"u8.ToArray();
    private X509Certificate2 certificate = null!;

    private readonly SignatureService signatureService = new();
    private readonly UserCertificateService userCertificateService = new();

    [SetUp]
    public void Setup()
    {
        certificate = userCertificateService.GenerateUserCertificate(Guid.NewGuid());
    }

    [Test]
    public void GenerateUserCertificate_ShouldReturnCertificate_WithUserId()
    {
        var userId = Guid.NewGuid();
        var cert = userCertificateService.GenerateUserCertificate(userId);

        cert.Extensions[UserCertificateService.UserIdOid].Should().NotBeNull();
        
        var actualUserId = new Guid(cert.Extensions[UserCertificateService.UserIdOid]!.RawData);
        
        actualUserId.Should().Be(userId);
    }
    
    [Test]
    public void GetCertificate_Existing_ShouldReturnCertificate()
    {
        var action = () => userCertificateService.GetCertificate(Thumbprint);
        action.Should().NotThrow();
        
        var x509Certificate2 = action();
        x509Certificate2.Thumbprint.Should().Be(Thumbprint);
    }

    [Test]
    public void Sign_ShouldReturnSignature()
    {
        var signature = signatureService.Sign(certificate, Content, attached: true);
        var signedData = SignedCmsHelper.FromSignature(signature);
        
        signedData.Certificates.First().Should().BeEquivalentTo(certificate);
    }
    
    [Test]
    public void VerifySignature_ShouldBeOk()
    {
        var signature = signatureService.Sign(certificate, Content, attached: true);
        
        signatureService.VerifySignature(signature).Should().BeNull();
    }
    
    [Test]
    public void VerifySignature_Broken_ShouldBeOk()
    {
        var signature = signatureService.Sign(certificate, Content, attached: true);

        signature[0]++;
        
        signatureService.VerifySignature(signature).Should().Contain("ASN1 corrupted data");
    }
    
    [Test]
    public void VerifySignature_BrokenIntegrity_ShouldBeOk()
    {
        var signature = signatureService.Sign(certificate, Content, attached: false);

        signatureService.VerifySignature(signature, "Broken integrity"u8.ToArray()).Should().NotBeNull();
    }
}