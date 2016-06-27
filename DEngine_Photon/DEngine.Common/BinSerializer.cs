using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace DEngine.Common
{
    public enum SerializerMode
    {
        Read,
        Write,
    }

    public class BinSerializer
    {
        #region Fields

        private delegate void ActionRef<T>(ref T value);

        private Stream _stream;

        private SerializerMode _mode;

        #endregion

        #region Properties

        public Stream Stream
        {
            get { return _stream; }
            private set { _stream = value; }
        }

        public SerializerMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;

                if (_mode == SerializerMode.Read)
                    Reader = Reader ?? new BinaryReader(_stream);
                else
                    Writer = Writer ?? new BinaryWriter(_stream);
            }
        }

        public BinaryReader Reader { get; private set; }

        public BinaryWriter Writer { get; private set; }

        #endregion

        #region Constructors

        public BinSerializer(Stream stream, SerializerMode mode)
        {
            Stream = stream;
            Mode = mode;
        }

        #endregion

        #region Base Methods

        public void Serialize(ref bool value)
        {
            if (Mode == SerializerMode.Write)
                Writer.Write(value);
            else
                value = Reader.ReadBoolean();
        }

        public void Serialize(ref string value)
        {
            if (Mode == SerializerMode.Write)
                Writer.Write(value);
            else
                value = Reader.ReadString();
        }

        public void Serialize(ref char value)
        {
            if (Mode == SerializerMode.Write)
                Writer.Write(value);
            else
                value = Reader.ReadChar();
        }

        public void Serialize(ref byte value)
        {
            if (Mode == SerializerMode.Write)
                Writer.Write(value);
            else
                value = Reader.ReadByte();
        }

        public void Serialize(ref sbyte value)
        {
            if (Mode == SerializerMode.Write)
                Writer.Write(value);
            else
                value = Reader.ReadSByte();
        }

        public void Serialize(ref short value)
        {
            if (Mode == SerializerMode.Write)
                Writer.Write(value);
            else
                value = Reader.ReadInt16();
        }

        public void Serialize(ref ushort value)
        {
            if (Mode == SerializerMode.Write)
                Writer.Write(value);
            else
                value = Reader.ReadUInt16();
        }

        public void Serialize(ref int value)
        {
            if (Mode == SerializerMode.Write)
                Writer.Write(value);
            else
                value = Reader.ReadInt32();
        }

        public void Serialize(ref uint value)
        {
            if (Mode == SerializerMode.Write)
                Writer.Write(value);
            else
                value = Reader.ReadUInt32();
        }

        public void Serialize(ref long value)
        {
            if (Mode == SerializerMode.Write)
                Writer.Write(value);
            else
                value = Reader.ReadInt64();
        }

        public void Serialize(ref ulong value)
        {
            if (Mode == SerializerMode.Write)
                Writer.Write(value);
            else
                value = Reader.ReadUInt64();
        }

        public void Serialize(ref float value)
        {
            if (Mode == SerializerMode.Write)
                Writer.Write(value);
            else
                value = Reader.ReadSingle();
        }

        public void Serialize(ref double value)
        {
            if (Mode == SerializerMode.Write)
                Writer.Write(value);
            else
                value = Reader.ReadDouble();
        }

        public void Serialize(ref DateTime value)
        {
            if (Mode == SerializerMode.Write)
                Writer.Write(value.ToBinary());
            else
                value = DateTime.FromBinary(Reader.ReadInt64());
        }

        public void Serialize(ref Guid value)
        {
            if (Mode == SerializerMode.Write)
                Writer.Write(value.ToByteArray());
            else
                value = new Guid(Reader.ReadBytes(16));
        }

        public void Serialize(ref Type type)
        {
            if (Mode == SerializerMode.Write)
                Writer.Write(type.FullName);
            else
                type = Type.GetType(Reader.ReadString());
        }

        public void Serialize(ref Vector2 value)
        {
            if (Mode == SerializerMode.Write)
            {
                Writer.Write(value.x);
                Writer.Write(value.y);
            }
            else
            {
                value.x = Reader.ReadSingle();
                value.y = Reader.ReadSingle();
            }
        }

        public void Serialize(ref Vector3 value)
        {
            if (Mode == SerializerMode.Write)
            {
                Writer.Write(value.x);
                Writer.Write(value.y);
                Writer.Write(value.z);
            }
            else
            {
                value.x = Reader.ReadSingle();
                value.y = Reader.ReadSingle();
                value.z = Reader.ReadSingle();
            }
        }

        public void Serialize(ref Quaternion value)
        {
            if (Mode == SerializerMode.Write)
            {
                Writer.Write(value.x);
                Writer.Write(value.y);
                Writer.Write(value.z);
                Writer.Write(value.w);
            }
            else
            {
                value.x = Reader.ReadSingle();
                value.y = Reader.ReadSingle();
                value.z = Reader.ReadSingle();
                value.w = Reader.ReadSingle();
            }
        }

        public void Flush()
        {
            Writer.Flush();
        }

        #endregion

        #region Template Methods

        public void Save<T>(T tObject) where T : IDataSerializable, new()
        {
            Mode = SerializerMode.Write;
            tObject.Serialize(this);
            Flush();
        }

        public T Load<T>() where T : IDataSerializable, new()
        {
            T tObject = new T();
            Mode = SerializerMode.Read;
            tObject.Serialize(this);
            return tObject;
        }

        public void Serialize<T>(ref T tObject) where T : IDataSerializable, new()
        {
            if (Mode == SerializerMode.Read)
                tObject = new T();

            tObject.Serialize(this);
        }

        public void Serialize<T>(List<T> listObject) where T : IDataSerializable, new()
        {
            int count = listObject.Count;
            Serialize(ref count);

            if (Mode == SerializerMode.Write)
            {
                for (int i = 0; i < count; i++)
                    listObject[i].Serialize(this);

                Flush();
            }
            else
            {
                listObject.Clear();
                for (int i = 0; i < count; i++)
                {
                    T itemValue = new T();
                    itemValue.Serialize(this);
                    listObject.Add(itemValue);
                }
            }
        }

        public void SerializeEx<T>(ref T tObject) where T : new()
        {
            Type objType = tObject.GetType();
            TypeCode typeCode = Type.GetTypeCode(objType);

            if (typeCode != TypeCode.Object)
            {
                if (Mode == SerializerMode.Write)
                    WriteValue(typeCode, tObject);
                else
                    tObject = (T)ReadValue(typeCode);
            }
            else if (tObject is IDataSerializable)
                ((IDataSerializable)tObject).Serialize(this);
            else if (tObject is Vector2)
            {
                Vector2 value = (Vector2)((object)tObject);
                Serialize(ref value);
                if (Mode == SerializerMode.Read)
                    tObject = (T)((object)value);
            }
            else if (tObject is Vector3)
            {
                Vector3 value = (Vector3)((object)tObject);
                Serialize(ref value);
                if (Mode == SerializerMode.Read)
                    tObject = (T)((object)value);
            }
            else if (tObject is Quaternion)
            {
                Quaternion value = (Quaternion)((object)tObject);
                Serialize(ref value);
                if (Mode == SerializerMode.Read)
                    tObject = (T)((object)value);
            }
            else
            {
                FieldInfo[] allFields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
                if (allFields.Length == 0)
                    throw new Exception(string.Format("Serialize with unsupported Type {0}", objType));

                if (Mode == SerializerMode.Write)
                {
                    foreach (FieldInfo field in allFields)
                    {
                        Type fieldType = field.FieldType;
                        TypeCode fieldCode = Type.GetTypeCode(fieldType);
                        object fieldValue = field.GetValue(tObject);

                        if (fieldCode != TypeCode.Object)
                            WriteValue(fieldCode, fieldValue);
                        else if (fieldType.IsArray)
                        {
                            Array arrayValue = (Array)fieldValue;
                            if (arrayValue == null)
                                Writer.Write((int)0);
                            else
                            {
                                Writer.Write(arrayValue.Length);
                                foreach (var item in arrayValue)
                                {
                                    object itemValue = item;
                                    SerializeEx(ref itemValue);
                                }
                            }
                        }
                        else if (fieldType.Name == typeof(List<>).Name)
                        {
                            IList listValue = (IList)fieldValue;
                            if (listValue == null)
                                Writer.Write((int)0);
                            else
                            {
                                Writer.Write(listValue.Count);
                                foreach (object item in listValue)
                                {
                                    object itemValue = item;
                                    SerializeEx(ref itemValue);
                                }
                            }
                        }
                        else if (fieldType.Name == typeof(Dictionary<,>).Name)
                        {
                            IDictionary listValue = (IDictionary)fieldValue;
                            if (listValue == null)
                                Writer.Write((int)0);
                            else
                            {
                                Writer.Write(listValue.Count);
                                foreach (DictionaryEntry item in listValue)
                                {
                                    object itemKey = item.Key;
                                    SerializeEx(ref itemKey);

                                    object itemValue = item.Value;
                                    SerializeEx(ref itemValue);
                                }
                            }
                        }
                        else
                            SerializeEx(ref fieldValue);
                    }

                    Flush();
                }
                else
                {
                    object boxedObj = new T();

                    foreach (FieldInfo field in allFields)
                    {
                        Type fieldType = field.FieldType;
                        TypeCode fieldCode = Type.GetTypeCode(fieldType);

                        if (fieldCode != TypeCode.Object)
                            field.SetValue(boxedObj, ReadValue(fieldCode));
                        else if (fieldType.IsArray)
                        {
                            Type itemType = fieldType.GetElementType();
                            int itemCount = Reader.ReadInt32();
                            if (itemCount > 0)
                            {
                                Array fieldValue = Array.CreateInstance(itemType, itemCount);
                                for (int i = 0; i < itemCount; i++)
                                {
                                    object itemValue = CreateObject(itemType);
                                    SerializeEx(ref itemValue);
                                    fieldValue.SetValue(itemValue, i);
                                }
                                field.SetValue(boxedObj, fieldValue);
                            }
                        }
                        else if (fieldType.Name == typeof(List<>).Name)
                        {
                            Type[] argTypes = fieldType.GetGenericArguments();
                            int itemCount = Reader.ReadInt32();
                            if (itemCount > 0)
                            {
                                IList fieldValue = (IList)CreateObject(fieldType);
                                for (int i = 0; i < itemCount; i++)
                                {
                                    object itemValue = CreateObject(argTypes[0]);
                                    SerializeEx(ref itemValue);
                                    fieldValue.Add(itemValue);
                                }
                                field.SetValue(boxedObj, fieldValue);
                            }
                        }
                        else if (fieldType.Name == typeof(Dictionary<,>).Name)
                        {
                            Type[] argTypes = fieldType.GetGenericArguments();
                            int itemCount = Reader.ReadInt32();
                            if (itemCount > 0)
                            {
                                IDictionary fieldValue = (IDictionary)CreateObject(fieldType);
                                for (int i = 0; i < itemCount; i++)
                                {
                                    object itemKey = CreateObject(argTypes[0]);
                                    object itemValue = CreateObject(argTypes[1]);
                                    SerializeEx(ref itemKey);
                                    SerializeEx(ref itemValue);
                                    fieldValue.Add(itemKey, itemValue);
                                }
                                field.SetValue(boxedObj, fieldValue);
                            }
                        }
                        else
                        {
                            object fieldValue = CreateObject(fieldType);
                            SerializeEx(ref fieldValue);
                            field.SetValue(boxedObj, fieldValue);
                        }
                    }

                    tObject = (T)boxedObj;
                }
            }
        }

        public void SerializeEx<T>(List<T> listObject) where T : new()
        {
            int count = listObject.Count;
            Serialize(ref count);

            if (Mode == SerializerMode.Write)
            {
                foreach (object item in listObject)
                {
                    object itemValue = item;
                    SerializeEx(ref itemValue);
                }
            }
            else
            {
                listObject.Clear();
                for (int i = 0; i < count; i++)
                {
                    T itemValue = default(T);
                    SerializeEx(ref itemValue);
                    listObject.Add(itemValue);
                }
            }
        }

        #endregion

        #region Private Methods

        private object CreateObject(Type type)
        {
            if (type == typeof(string))
                return "";

            return Activator.CreateInstance(type);
        }

        private object ReadValue(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return Reader.ReadBoolean();
                case TypeCode.Char:
                    return Reader.ReadChar();
                case TypeCode.SByte:
                    return Reader.ReadSByte();
                case TypeCode.Byte:
                    return Reader.ReadByte();
                case TypeCode.Int16:
                    return Reader.ReadInt16();
                case TypeCode.UInt16:
                    return Reader.ReadUInt16();
                case TypeCode.Int32:
                    return Reader.ReadInt32();
                case TypeCode.UInt32:
                    return Reader.ReadUInt32();
                case TypeCode.Int64:
                    return Reader.ReadInt64();
                case TypeCode.UInt64:
                    return Reader.ReadUInt64();
                case TypeCode.Single:
                    return Reader.ReadSingle();
                case TypeCode.Double:
                    return Reader.ReadDouble();
                case TypeCode.Decimal:
                    return Reader.ReadDecimal();
                case TypeCode.DateTime:
                    DateTime retValue = DateTime.FromFileTime(Reader.ReadInt64());
                    return retValue;
                case TypeCode.String:
                    return Reader.ReadString();
                default:
                    throw new Exception(string.Format("ReadValue with unsupported TypeCode {0}", typeCode));
            }
        }

        private void WriteValue(TypeCode typeCode, object objValue)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    Writer.Write((bool)objValue);
                    break;
                case TypeCode.Char:
                    Writer.Write((char)objValue);
                    break;
                case TypeCode.SByte:
                    Writer.Write((sbyte)objValue);
                    break;
                case TypeCode.Byte:
                    Writer.Write((byte)objValue);
                    break;
                case TypeCode.Int16:
                    Writer.Write((short)objValue);
                    break;
                case TypeCode.UInt16:
                    Writer.Write((ushort)objValue);
                    break;
                case TypeCode.Int32:
                    Writer.Write((int)objValue);
                    break;
                case TypeCode.UInt32:
                    Writer.Write((uint)objValue);
                    break;
                case TypeCode.Int64:
                    Writer.Write((long)objValue);
                    break;
                case TypeCode.UInt64:
                    Writer.Write((ulong)objValue);
                    break;
                case TypeCode.Single:
                    Writer.Write((float)objValue);
                    break;
                case TypeCode.Double:
                    Writer.Write((double)objValue);
                    break;
                case TypeCode.Decimal:
                    Writer.Write((decimal)objValue);
                    break;
                case TypeCode.DateTime:
                    Writer.Write(((DateTime)objValue).ToFileTime());
                    break;
                case TypeCode.String:
                    Writer.Write((string)objValue);
                    break;
                default:
                    throw new Exception(string.Format("WriteValue with unsupported TypeCode {0}", typeCode));
            }
        }

        #endregion
    }
}
