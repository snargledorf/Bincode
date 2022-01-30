using System.Reflection;

namespace Bincode
{
    internal static class BincoderFactory
    {
        internal static Bincoder<T> CreateEncoder<T>() where T : class
        {
            IEnumerable<IBincoderFieldHandler<T>> bincodeFields = typeof(T).GetFields()
                .Where(f => f.GetCustomAttribute<BincodeFieldAttribute>() != null)
                .Select(f => new FieldBincoderFieldHandler<T>(f));
            IEnumerable<IBincoderFieldHandler<T>> bincodeProperties = typeof(T).GetProperties()
                .Where(p => p.GetCustomAttribute<BincodeFieldAttribute>() != null)
                .Select(p => new PropertyBincoderFieldHandler<T>(p));

            return new Bincoder<T>(bincodeFields.Concat(bincodeProperties));
        }
    }
}