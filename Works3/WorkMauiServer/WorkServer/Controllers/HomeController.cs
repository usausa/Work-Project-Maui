namespace WorkServer.Controllers;

using QRCoder;

public class HomeController : Controller
{
    [Route("/")]
    [HttpGet]
    public IActionResult Home()
    {
        var req = HttpContext.Request;
        var content = $"ApiEndPoint={req.Scheme}://{req.Host}";

        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        using var png = new PngByteQRCode(data);
        var bytes = png.GetGraphic(20);

        var src = "data:image/png;base64," + Convert.ToBase64String(bytes);

        // ReSharper disable StringLiteralTypo
        // lang=html
        var html = """
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="utf-8">
                <title>Server</title>
            </head>
            <body>
                <img src="{0}" width="320" height="320">
            </body>
            </html>
            """;
        // ReSharper restore StringLiteralTypo
        return Content(String.Format(html, src), "text/html");
    }
}
