using PdfSharpCore.Fonts;
namespace SolidMatrix.Toolkits.QrSheets;

public class FontResolver : IFontResolver
{
    public string DefaultFontName => "SourceHanSansCN";
    public byte[] GetFont(string faceName) => Fonts.SourceHanSansCN;
    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic) => new("SourceHanSansCN");
}