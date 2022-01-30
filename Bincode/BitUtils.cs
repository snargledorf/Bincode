namespace Bincode
{
    internal static class BitUtils
    {
        public const int BitsInByte = 8;

        public static byte RightBits(byte b, int count)
        {
            return (byte)((0xFF >> (BitsInByte - count)) & b);
        }

        public static byte LeftBits(byte b, int count)
        {
            return (byte)((0xFF << (BitsInByte - count)) & b);
        }
    }
}
