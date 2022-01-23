using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using QRCoder;
using System.Collections.Concurrent;

namespace SolidMatrix.Toolkits.QrSheets;

public class QrSheets
{
    private string _name;
    private ConcurrentDictionary<string, Stream> _codeDict = new();

    private readonly int PageNumber = 100;
    private readonly int CodePerPage = 10;

    public string Name => _name;

    public QrSheets()
    {
        GlobalFontSettings.FontResolver = new FontResolver();

        using (var nameGenerator = new UniqueGenerator<string>())
        {
            var name = nameGenerator.Next(checker =>
            {
                for (int i = 0; i < 100; i++)
                {
                    var name = $"001-{i:00}-{DateTime.Now.ToString("yyMMdd")}";
                    if (checker(name)) return name;
                }
                return null;
            });

            _name = name;
        }
    }

    public void DrawPage(XGraphics gfx, int pageNumber)
    {
        double _factor = 2.8346456692913389;
        var font = new XFont("SourceHanSansCN", 12);
        for (int codeNumber = 0; codeNumber < CodePerPage; codeNumber++)
        {
            var code = $"{_name}-{pageNumber * 100 + codeNumber:00000}";
            var image = XImage.FromStream(() => _codeDict[code]);

            var xs = codeNumber / 5 * 52;
            var ys = codeNumber % 5 * 32;

            gfx.DrawImage(image, new XRect(xs * _factor, ys * _factor, 20 * _factor, 20 * _factor));
            gfx.DrawString(code, font, XBrushes.Black, new XRect(xs * _factor, (ys + 20) * _factor, 50 * _factor, 10 * _factor), XStringFormats.TopCenter);
            gfx.DrawString("米数", font, XBrushes.Black, new XRect((xs + 20) * _factor, ys * _factor, 30 * _factor, 10 * _factor), XStringFormats.CenterLeft);
            gfx.DrawString("花号", font, XBrushes.Black, new XRect((xs + 20) * _factor, (ys + 10) * _factor, 30 * _factor, 10 * _factor), XStringFormats.CenterLeft);
        }
    }

    public void PrepareQrCode()
    {
        var qrCodeGenerator = new QRCodeGenerator();

        Parallel.For(0, CodePerPage, j =>
        {
            for (int i = 0; i < PageNumber; i++)
            {
                var code = $"{_name}-{i * 100 + j:00000}";
                var qrCodeData = qrCodeGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.H);
                var qrCode = new BitmapByteQRCode(qrCodeData);
                _codeDict[code] = new MemoryStream(qrCode.GetGraphic(4)); ;
            }
        });
    }

    public void Gen()
    {
        var doc = new PdfDocument();

        for (int i = 0; i < PageNumber; i++)
        {
            var page = doc.AddPage();
            page.Width = new XUnit(102, XGraphicsUnit.Millimeter);
            page.Height = new XUnit(158, XGraphicsUnit.Millimeter);
            var gfx = XGraphics.FromPdfPage(page);
            DrawPage(gfx, i);
        }

        doc.Save($"{_name}.pdf");
    }
}
