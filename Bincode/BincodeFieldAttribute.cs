using System.Runtime.CompilerServices;

namespace Bincode
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class BincodeFieldAttribute : Attribute
    {
        public BincodeFieldAttribute(int bitCount = default, [CallerLineNumber] int order = 0)
        {
            Bits = bitCount;
            Order = order;
        }

        public int Bits { get; }

        public int Order { get; }
    }
}