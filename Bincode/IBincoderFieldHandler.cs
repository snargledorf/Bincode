namespace Bincode
{
    internal interface IBincoderFieldHandler<T> where T : class
    {
        int Order { get; }
        void Encode(T obj, BitWriter buffer);
        void Decode(BitReader reader, T obj);
    }
}