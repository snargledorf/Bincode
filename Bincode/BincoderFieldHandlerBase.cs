using System.Reflection;
using System.Runtime.InteropServices;

namespace Bincode
{
    internal abstract class BincoderFieldHandlerBase<T> : IBincoderFieldHandler<T> where T : class
    {
        private readonly BincodeFieldAttribute attribute;

        internal int BitCount
        {
            get
            {
                var bitCount = attribute.Bits;
                if (bitCount == 0)
                    bitCount = Marshal.SizeOf(DataType)*BitUtils.BitsInByte;

                return bitCount;
            }
        }

        protected BincoderFieldHandlerBase(MemberInfo member)
        {
            attribute = member.GetCustomAttribute<BincodeFieldAttribute>()!;
        }

        internal abstract Type DataType { get; }

        public int Order => attribute.Order;

        public void Decode(BitReader reader, T obj)
        {
            object? value = ReadValue(reader, DataType);
            SetValue(obj, value);
        }

        public void Encode(T obj, BitWriter writer)
        {
            object? value = GetValue(obj);
            WriteValue(writer, value);
        }

        protected abstract void SetValue(T obj, object? value);

        protected abstract object? GetValue(T obj);

        private object? ReadValue(BitReader reader, Type propertyType)
        {
            if (propertyType == typeof(byte))
                return (byte)reader.ReadByte(BitCount);

            if (propertyType == typeof(short))
                return reader.ReadInt16(BitCount);

            if (propertyType == typeof(int))
                return reader.ReadInt32(BitCount);

            if (propertyType == typeof(long))
                return reader.ReadInt64(BitCount);

            if (propertyType == typeof(float))
                return reader.ReadSingle(BitCount);

            if (propertyType == typeof(double))
                return reader.ReadDouble(BitCount);

            if (propertyType.IsEnum)
            {
                Type enumType = Enum.GetUnderlyingType(propertyType);
                return ReadValue(reader, enumType);
            }

            throw new NotSupportedException($"{propertyType} is not a supported data type");
        }

        private void WriteValue(BitWriter writer, object? value)
        {
            switch (value)
            {
                case byte b:
                    writer.Write(b, BitCount);
                    break;
                case short s:
                    writer.Write(s, BitCount);
                    break;
                case int i:
                    writer.Write(i, BitCount);
                    break;
                case long l:
                    writer.Write(l, BitCount);
                    break;
                case float f:
                    writer.Write(f, BitCount);
                    break;
                case double d:
                    writer.Write(d, BitCount);
                    break;
                case string str:
                    writer.Write(str, BitCount);
                    break;
                case object possibleEnum when possibleEnum.GetType().IsEnum:
                    Type enumDataType = Enum.GetUnderlyingType(possibleEnum.GetType());
                    object enumData = Convert.ChangeType(possibleEnum, enumDataType);
                    WriteValue(writer, enumData);
                    break;
                case null:
                    throw new NullReferenceException();
                default:
                    throw new InvalidOperationException($"Unsupported data type: '{value.GetType()}'");
            }
        }
    }
}