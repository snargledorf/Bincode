using System.Reflection;

namespace Bincode
{
    internal class FieldBincoderFieldHandler<T> : BincoderFieldHandlerBase<T> where T : class
    {
        private readonly FieldInfo fieldInfo;

        internal override Type DataType => fieldInfo.FieldType;

        public FieldBincoderFieldHandler(FieldInfo fieldInfo)
            : base(fieldInfo)
        {
            this.fieldInfo = fieldInfo;
        }

        protected override void SetValue(T obj, object? value)
        {
            fieldInfo.SetValue(obj, value);
        }

        protected override object? GetValue(T obj)
        {
            return fieldInfo.GetValue(obj);
        }
    }
}