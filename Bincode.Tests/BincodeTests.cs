using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bincode.Tests
{
    [TestClass]
    public class BincodeTests
    {
        [TestMethod]
        public void Encode()
        {
            byte[] expectedBytes = { 127, 0, 0, 0, 0 };

            var s = new Test() { A = 7, B = TestEnum.World, C = 7 };
            byte[] result = Bincoder<Test>.Instance.Encode(s);

            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(expectedBytes, result);
        }

        [TestMethod]
        public void Decode()
        {
            byte[] bytes = { 127, 0, 0, 0, 0 };

            Test result = Bincoder<Test>.Instance.Decode(bytes);

            Assert.AreEqual(7, result.A);
            Assert.AreEqual(TestEnum.World, result.B);
            Assert.AreEqual(7, result.C);
        }

        class Test
        {
            [BincodeField(3)]
            public byte A;
            [BincodeField(1)]
            public TestEnum B;
            [BincodeField]
            public int C;
        }

        enum TestEnum
        {
            Hello,
            World
        }
    }
}