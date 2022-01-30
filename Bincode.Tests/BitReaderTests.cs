using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bincode.Tests
{
    [TestClass]
    public class BitReaderTests
    {
        [TestMethod]
        public void ReadBits()
        {
            var ms = new MemoryStream(new byte[] { 5 });
            var reader = new BitReader(ms);

            Assert.IsTrue(reader.TryReadBit(out bool bit));
            Assert.IsTrue(bit);

            Assert.IsTrue(reader.TryReadBit(out bit));
            Assert.IsFalse(bit);

            Assert.IsTrue(reader.TryReadBit(out bit));
            Assert.IsTrue(bit);
        }

        [TestMethod]
        public void ReadBytes()
        {
            var ms = new MemoryStream(new byte[] { 160, 3 });
            var reader = new BitReader(ms);

            Assert.AreEqual(160, reader.ReadByte());
            Assert.AreEqual(3, reader.ReadByte());
        }

        [TestMethod]
        public void ReadBitsAndBytes()
        {
            var ms = new MemoryStream(new byte[] { 5, 3 });
            var reader = new BitReader(ms);

            Assert.IsTrue(reader.TryReadBit(out bool bit));
            Assert.IsTrue(bit);

            Assert.AreEqual(130, reader.ReadByte());
            Assert.AreEqual(1, reader.ReadByte(false));
        }

        [TestMethod]
        public void ReadBitEndOfStream()
        {
            var ms = new MemoryStream(System.Array.Empty<byte>());
            var reader = new BitReader(ms);

            Assert.IsFalse(reader.TryReadBit(out bool _));
        }
    }
}
