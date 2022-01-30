using System.Collections;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bincode.Tests
{
    [TestClass]
    public class BitWriterTests
    {
        [TestMethod]
        public void WriteBits()
        {
            var ms = new MemoryStream();
            var writer = new BitWriter(ms);

            writer.WriteBit(true);
            writer.WriteBit(false);
            writer.WriteBit(true);

            writer.Flush();

            byte[] vs = ms.ToArray();

            Assert.AreEqual(1, vs.Length);
            Assert.AreEqual(5, vs[0]);
        }

        [TestMethod]
        public void WriteBytes()
        {
            var ms = new MemoryStream();
            var writer = new BitWriter(ms);

            writer.Write((byte)255);
            writer.Write((byte)10);

            writer.Flush();

            byte[] vs = ms.ToArray();

            Assert.AreEqual(2, vs.Length);
            Assert.AreEqual(255, vs[0]);
            Assert.AreEqual(10, vs[1]);
        }

        [TestMethod]
        public void WriteByteArray()
        {
            var ms = new MemoryStream();
            var writer = new BitWriter(ms);

            var bytes = new byte[] { 255, 10 };
            writer.Write(bytes);

            writer.Flush();

            byte[] vs = ms.ToArray();

            Assert.AreEqual(2, vs.Length);
            Assert.AreEqual(255, vs[0]);
            Assert.AreEqual(10, vs[1]);
        }

        [TestMethod]
        public void WriteBitsAndBytes()
        {
            var ms = new MemoryStream();
            var writer = new BitWriter(ms);

            writer.WriteBit(true);

            var bytes = new byte[] { 5, 10 };
            writer.Write(bytes);

            writer.Flush();

            byte[] vs = ms.ToArray();

            Assert.AreEqual(3, vs.Length);
            Assert.AreEqual(11, vs[0]);
            Assert.AreEqual(20, vs[1]);
            Assert.AreEqual(0, vs[2]);
        }
    }
}
