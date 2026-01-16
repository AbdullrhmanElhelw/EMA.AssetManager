using QRCoder;

public class QrCodeService
{
    public string GenerateQrCode(string text)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);

        // تحويل الباركود لمصفوفة بايت
        var qrCodeImage = qrCode.GetGraphic(20);

        // تحويله لنص Base64 عشان يظهر في المتصفح
        return $"data:image/png;base64,{Convert.ToBase64String(qrCodeImage)}";
    }
}