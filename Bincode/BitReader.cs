using System.Text;

namespace Bincode
{
    public partial class BitReader
    {
        private readonly Stream stream;
        private readonly Encoding? encoding;
        private readonly bool leaveOpen;

        private byte currentByte;
        private int currentBitCount;

        public BitReader(Stream stream, Encoding? encoding = default, bool leaveOpen = true)
        {
            this.stream = stream;
            this.encoding = encoding ?? Encoding.Default;
            this.leaveOpen = leaveOpen;
        }

        public bool TryReadBit(out bool bit)
        {
            if (currentBitCount == 0)
            {
                int read = stream.ReadByte();
                if (read == -1)
                {
                    bit = false;
                    return false;
                }

                currentByte = (byte)read;
                currentBitCount = BitUtils.BitsInByte;
            }

            bit = (currentByte & 0x1) == 1;
            currentByte >>= 1;
            currentBitCount--;

            return true;
        }

        public int ReadByte(bool enforceBitCount = true)
        {
            return ReadByte(BitUtils.BitsInByte, enforceBitCount);
        }

        public int ReadByte(int bitCount, bool enforceBitCount = true)
        {
            ReadBits(bitCount, enforceBitCount, out byte result);
            return result;
        }

        public short ReadInt16(int bitCount, bool enforceBitCount = true)
        {
            var buffer = new byte[sizeof(short)];
            Read(buffer, bitCount, enforceBitCount);
            return BitConverter.ToInt16(buffer);
        }

        public int ReadInt32(int bitCount, bool enforceBitCount = true)
        {
            var buffer = new byte[sizeof(int)];
            Read(buffer, bitCount, enforceBitCount);
            return BitConverter.ToInt32(buffer);
        }

        public long ReadInt64(int bitCount, bool enforceBitCount = true)
        {
            var buffer = new byte[sizeof(long)];
            Read(buffer, bitCount, enforceBitCount);
            return BitConverter.ToInt64(buffer);
        }

        public float ReadSingle(int bitCount, bool enforceBitCount = true)
        {
            var buffer = new byte[sizeof(float)];
            Read(buffer, bitCount, enforceBitCount);
            return BitConverter.ToSingle(buffer);
        }

        public double ReadDouble(int bitCount, bool enforceBitCount = true)
        {
            var buffer = new byte[sizeof(double)];
            Read(buffer, bitCount, enforceBitCount);
            return BitConverter.ToDouble(buffer);
        }

        public int Read(byte[] buffer, int offset, int count, bool enforceBitCount = true)
        {
            return Read(buffer.AsSpan().Slice(offset, count), enforceBitCount);
        }

        public int Read(byte[] buffer, int offset, int count, int bitCount, bool enforceBitCount = true)
        {
            return Read(buffer.AsSpan().Slice(offset, count), bitCount, enforceBitCount);
        }

        public int Read(Span<byte> buffer, bool enforceBitCount = true)
        {
            int bitCount = buffer.Length * BitUtils.BitsInByte;
            return Read(buffer, bitCount, enforceBitCount);
        }

        public int Read(Span<byte> buffer, int bitCount, bool enforceBitCount = true)
        {
            return ReadBits(buffer, bitCount, enforceBitCount);
        }

        public int ReadBits(byte[] buffer, int offset, int length, int bitCount, bool enforceBitCount = true)
        {
            return ReadBits(buffer.AsSpan().Slice(offset, length), bitCount, enforceBitCount);
        }

        public int ReadBits(Span<byte> bytes, int bitCount, bool enforceBitCount = true)
        {
            if (bytes.Length * BitUtils.BitsInByte < bitCount)
                throw new ArgumentOutOfRangeException(nameof(bitCount), "Bit count is greater than number of bits in buffer");

            int totalBitsRead = 0;
            for (int i = 0; totalBitsRead < bitCount; i++)
            {
                int bitsToRead = Math.Min(BitUtils.BitsInByte, bitCount);
                int bitsRead = ReadBits(bitsToRead, enforceBitCount, out byte b);
                totalBitsRead += bitsRead;

                bytes[i] = b;

                if (bitsRead < bitsToRead)
                    break;
            }

            return totalBitsRead;
        }

        public int ReadBits(int bitCount, out byte result)
        {
            return ReadBits(bitCount, true, out result);
        }

        public int ReadBits(int bitCount, bool enforceBitCount, out byte result)
        {
            if (bitCount > BitUtils.BitsInByte)
                throw new ArgumentOutOfRangeException(nameof(bitCount), $"Bit count is larger than a byte: {bitCount}");

            if (currentBitCount >= bitCount)
            {
                result = BitUtils.RightBits(currentByte, bitCount);
                currentByte >>= bitCount;
                currentBitCount -= bitCount;
                return bitCount;
            }

            int read = stream.ReadByte();
            if (read == -1)
            {
                result = currentByte;

                if (enforceBitCount && currentBitCount < bitCount)
                    throw new EndOfStreamException();

                bitCount = currentBitCount;

                currentByte = 0;
                currentBitCount = -1;

                return bitCount;
            }

            byte newByte = (byte)read;
            int newByteBitCount;
            if (currentBitCount == 0)
            {
                currentByte = newByte;
                currentBitCount = BitUtils.BitsInByte;
                newByte = 0;
                newByteBitCount = 0;
            }
            else
            {
                // Append the new bits to the end of the current byte to make a whole byte
                currentByte |= (byte)(newByte << currentBitCount);

                // Shift the newByte right to remove the bits we just appended to the current byte
                newByte >>= BitUtils.BitsInByte - currentBitCount;
                newByteBitCount = BitUtils.BitsInByte - (BitUtils.BitsInByte - currentBitCount);
            }

            // Grab just the bits we need
            result = BitUtils.RightBits(currentByte, bitCount);

            // Remove the bits we just grabbed
            currentByte >>= bitCount;
            currentBitCount = BitUtils.BitsInByte - bitCount;

            // Append the remaining bits to the current byte
            currentByte |= (byte)(BitUtils.RightBits(newByte, newByteBitCount) << currentBitCount);
            currentBitCount += newByteBitCount;

            return bitCount;
        }
    }

    partial class BitReader : IDisposable
    {
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (!leaveOpen)
                        stream.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~BitReader()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
