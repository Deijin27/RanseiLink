using System.Text;

namespace RanseiLink.Core.Text;

/// <summary>
/// Always get japanese encodings from here to make sure that the encoding provider is registered before doing anything else
/// </summary>
internal static class EncodingProvider
{
    static EncodingProvider()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        ShiftJIS = Encoding.GetEncoding("shift_jis");
        ShiftJISExtended = Encoding.GetEncoding(932);
    }

    public static Encoding ShiftJIS { get; }
    public static Encoding ShiftJISExtended { get; }
}
