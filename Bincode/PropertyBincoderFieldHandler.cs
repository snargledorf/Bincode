using System.Reflection;

namespace Bincode
{
    internal class PropertyBincoderFieldHandler<T> : BincoderFieldHandlerBase<T> where T : class
    {
        private readonly PropertyInfo property;

        private readonly Func<T, object?>?  getter;
        private readonly Action<T, object?>? setter;

        public PropertyBincoderFieldHandler(PropertyInfo property)
            : base(property)
        {
            this.property = property;
            
            getter = BuildGetAccessor(property);
            setter = BuildSetAccessor(property);
        }

        internal override Type DataType => property.PropertyType;

        public static Func<T, object?>? BuildGetAccessor(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetGetMethod()?.CreateDelegate<Func<T, object?>>();
        }

        public static Action<T, object?>? BuildSetAccessor(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetSetMethod()?.CreateDelegate<Action<T, object?>>();
        }

        protected override object? GetValue(T obj)
        {
            if (getter is null)
                throw new NotSupportedException($"Property {property.Name} does not have a getter");

            return getter(obj);
        }

        protected override void SetValue(T obj, object? value)
        {
            if (setter is null)
                throw new NotSupportedException($"Property {property.Name} does not have a setter");

            setter(obj, value);
        }
    }
}