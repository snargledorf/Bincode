using System.Text;

namespace Bincode
{
    public sealed class Bincoder<T> where T : class
    {
        private static Bincoder<T>? instance;

        private readonly IEnumerable<IBincoderFieldHandler<T>> fields;

        internal Bincoder(IEnumerable<IBincoderFieldHandler<T>> fields)
        {
            this.fields = fields;
        }

        public static Bincoder<T> Instance => instance ??= BincoderFactory.CreateEncoder<T>();

        public byte[] Encode(T obj, Encoding? encoding = default)
        {
            using var ms = new MemoryStream();
            Encode(obj, ms, encoding);
            return ms.ToArray();
        }

        private void Encode(T obj, Stream stream, Encoding? encoding = default, bool leaveOpen = true)
        {
            using var writer = new BitWriter(stream, encoding, leaveOpen);

            foreach (var field in fields.OrderBy(f => f.Order))
                field.Encode(obj, writer);

            writer.Flush();
        }

        public T Decode(byte[] bytes, Encoding? encoding = default)
        {
            using var ms = new MemoryStream(bytes);
            return Decode(ms, encoding);
        }

        public T Decode(Stream stream, Encoding? encoding = default, bool leaveOpen = true)
        {
            using var reader = new BitReader(stream, encoding, leaveOpen);

            T obj = Activator.CreateInstance<T>()!;

            foreach (var field in fields.OrderBy(f => f.Order))
                field.Decode(reader, obj);

            return obj;
        }
    }
}