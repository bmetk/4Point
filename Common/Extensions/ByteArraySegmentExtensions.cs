using System.Text;

namespace Common.Extensions;

public static class ByteArraySegmentExtensions
{
    public static string AsUTF8String(this ArraySegment<byte> segment) => Encoding.UTF8.GetString(segment);
}