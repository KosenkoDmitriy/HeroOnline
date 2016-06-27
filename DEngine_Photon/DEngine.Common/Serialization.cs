using Ionic.Zlib;
using System;
using System.IO;

namespace DEngine.Common
{
    public interface IDataSerializable
    {
        void Serialize(BinSerializer serializer);
    }

    public static class Serialization
    {
        public static byte[] Save<T>(T value, bool packed = false) where T : IDataSerializable, new()
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinSerializer serializer = new BinSerializer(memStream, SerializerMode.Write);

                serializer.Save<T>(value);

                byte[] buffer = memStream.GetBuffer();
                Array.Resize(ref buffer, (int)memStream.Length);

                if (packed)
                    buffer = ZlibStream.CompressBuffer(buffer);

                return buffer;
            }
        }

        public static T Load<T>(byte[] buffer, bool packed = false) where T : IDataSerializable, new()
        {
            if (packed)
                buffer = ZlibStream.UncompressBuffer(buffer);

            using (MemoryStream memStream = new MemoryStream(buffer))
            {
                BinSerializer serializer = new BinSerializer(memStream, SerializerMode.Read);
                return serializer.Load<T>();
            }
        }

        public static byte[] SaveArray<T>(T[] arrayValue, bool packed = false) where T : IDataSerializable, new()
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinSerializer serializer = new BinSerializer(memStream, SerializerMode.Write);

                int count = arrayValue.Length;
                serializer.Serialize(ref count);

                for (int i = 0; i < count; i++)
                    arrayValue[i].Serialize(serializer);

                byte[] buffer = memStream.GetBuffer();
                Array.Resize(ref buffer, (int)memStream.Length);

                if (packed)
                    buffer = ZlibStream.CompressBuffer(buffer);

                return buffer;
            }
        }

        public static T[] LoadArray<T>(byte[] buffer, bool packed = false) where T : IDataSerializable, new()
        {
            if (packed)
                buffer = ZlibStream.UncompressBuffer(buffer);

            using (MemoryStream memStream = new MemoryStream(buffer))
            {
                BinSerializer serializer = new BinSerializer(memStream, SerializerMode.Read);

                int count = 0;
                serializer.Serialize(ref count);

                if (count == 0)
                    return null;

                T[] arrayValue = new T[count];

                for (int i = 0; i < count; i++)
                {
                    arrayValue[i] = new T();
                    arrayValue[i].Serialize(serializer);
                }

                return arrayValue;
            }
        }

        public static byte[] SaveStruct<T>(T value, bool packed = false) where T : new()
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinSerializer serializer = new BinSerializer(memStream, SerializerMode.Write);
                serializer.SerializeEx<T>(ref value);

                byte[] buffer = memStream.GetBuffer();
                Array.Resize(ref buffer, (int)memStream.Length);

                if (packed)
                    buffer = ZlibStream.CompressBuffer(buffer);

                return buffer;
            }
        }

        public static T LoadStruct<T>(byte[] buffer, bool packed = false) where T : new()
        {
            if (packed)
                buffer = ZlibStream.UncompressBuffer(buffer);

            using (MemoryStream memStream = new MemoryStream(buffer))
            {
                BinSerializer serializer = new BinSerializer(memStream, SerializerMode.Read);
                T value = new T();
                serializer.SerializeEx<T>(ref value);
                return value;
            }
        }

        public static byte[] SaveStructArray<T>(T[] arrayValue, bool packed = false) where T : new()
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinSerializer serializer = new BinSerializer(memStream, SerializerMode.Write);

                int count = arrayValue.Length;
                serializer.Serialize(ref count);

                for (int i = 0; i < count; i++)
                    serializer.SerializeEx(ref arrayValue[i]);

                byte[] buffer = memStream.GetBuffer();
                Array.Resize(ref buffer, (int)memStream.Length);

                if (packed)
                    buffer = ZlibStream.CompressBuffer(buffer);

                return buffer;
            }
        }

        public static T[] LoadStructArray<T>(byte[] buffer, bool packed = false) where T : new()
        {
            if (packed)
                buffer = ZlibStream.UncompressBuffer(buffer);

            using (MemoryStream memStream = new MemoryStream(buffer))
            {
                BinSerializer serializer = new BinSerializer(memStream, SerializerMode.Read);

                int count = 0;
                serializer.Serialize(ref count);

                if (count == 0)
                    return null;

                T[] arrayValue = new T[count];

                for (int i = 0; i < count; i++)
                {
                    arrayValue[i] = new T();
                    serializer.SerializeEx(ref arrayValue[i]);
                }

                return arrayValue;
            }
        }
    }
}
