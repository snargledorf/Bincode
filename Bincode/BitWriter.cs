using System.Text;

namespace Bincode
{
    public partial class BitWriter
    {
        private readonly Stream stream;
        private readonly Encoding encoding;
        private readonly bool leaveOpen;

        private byte currentByte;
        private int currentBitCount;

        public BitWriter(Stream stream, Encoding? encoding = default, bool leaveOpen = true)
        {
            this.stream = stream;
            this.encoding = encoding ?? Encoding.Default;
            this.leaveOpen = leaveOpen;
            currentBitCount = 0;
        }

        public void WriteBit(bool bit)
        {
            if (bit)
                currentByte |= (byte)(1 << currentBitCount);
          
            if (++currentBitCount == BitUtils.BitsInByte)
            {
                stream.WriteByte(currentByte);
                currentByte = 0;
                currentBitCount = 0;
            }
        }

        public void Write(byte value)
        {
            Write(value, BitUtils.BitsInByte);
        }

        public void Write(byte value, int bitCount)
        {
            if (bitCount > BitUtils.BitsInByte)
                throw new ArgumentOutOfRangeException(nameof(bitCount), $"Bit count larger than byte: {bitCount}");

            int bitsLeftInCurrentByte = BitUtils.BitsInByte - currentBitCount;

            int bitsToTakeFromValue = Math.Min(bitsLeftInCurrentByte, bitCount);

            // Append the bits taken from the value to the currentByte
            currentByte |= (byte)(BitUtils.RightBits(value, bitsToTakeFromValue) << currentBitCount);

            // Update the current bit count to reflect the bits we just appended
            currentBitCount += bitsToTakeFromValue;

            // If we are at a full byte, write it to the stream
            if (currentBitCount == BitUtils.BitsInByte)
            {
                stream.WriteByte(currentByte);

                // Update the bitCount to reflect the number of bits left in the value
                bitCount -= bitsToTakeFromValue;

                // If we hit a full byte, then there may still be some bits left if the value
                if (bitCount != 0)
                {
                    // Shift the value bits to the right to remove the bits we previously appended to the currentByte
                    value >>= bitsToTakeFromValue;

                    // Grab just the remaining bits from the value and set the new currentByte
                    currentByte = BitUtils.RightBits(value, bitCount);
                    currentBitCount = bitCount;
                }
                else
                {
                    // No more bits in value, so the current byte is just reset
                    currentBitCount = 0;
                    currentByte = 0;
                }
            }

            // If we weren't at a full byte, then all the bits were added to the currentByte
        }

        public void Write(bool value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(bool value, int bitCount)
        {
            WriteBits(BitConverter.GetBytes(value), bitCount);
        }

        public void Write(short value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(short value, int bitCount)
        {
            WriteBits(BitConverter.GetBytes(value), bitCount);
        }

        public void Write(int value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(int value, int bitCount)
        {
            WriteBits(BitConverter.GetBytes(value), bitCount);
        }

        public void Write(float value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(float value, int bitCount)
        {
            WriteBits(BitConverter.GetBytes(value), bitCount);
        }

        public void Write(double value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(double value, int bitCount)
        {
            WriteBits(BitConverter.GetBytes(value), bitCount);
        }

        public void Write(string str)
        {
            Write(encoding.GetBytes(str));
        }

        public void Write(string str, int bitCount)
        {
            WriteBits(encoding.GetBytes(str), bitCount);
        }

        public void Write(byte[] bytes, int offset, int count)
        {
            Write(bytes.AsSpan().Slice(offset, count));
        }

        public void Write(ReadOnlySpan<byte> bytes)
        {
            foreach (byte b in bytes)
                Write(b);
        }

        public void WriteBits(byte[] bytes, int bitCount)
        {
            WriteBits(bytes, 0, bitCount);
        }

        public void WriteBits(byte[] bytes, int offset, int bitCount)
        {
            WriteBits(bytes.AsSpan().Slice(offset), bitCount);
        }

        public void WriteBits(ReadOnlySpan<byte> bytes, int bitCount)
        {
            if (bytes.Length * BitUtils.BitsInByte < bitCount)
                throw new ArgumentOutOfRangeException(nameof(bitCount), $"Bit count is greater than number of bits in buffer");

            foreach (byte b in bytes)
            {
                int bitsToWrite = Math.Min(BitUtils.BitsInByte, bitCount);
                Write(b, bitsToWrite);
                bitCount -= bitsToWrite;
            }
        }

        public void Flush()
        {
            if (currentBitCount != 0)
            {
                stream.WriteByte(currentByte);
                currentByte = 0;
                currentBitCount = 0;
            }

            stream.Flush();
        }
    }

    partial class BitWriter : IDisposable
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
        // ~BitWriter()
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
