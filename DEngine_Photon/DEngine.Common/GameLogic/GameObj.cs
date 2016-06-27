using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace DEngine.Common.GameLogic
{
    public abstract class GameObj : IDataSerializable
    {
        #region Fields

        protected int _id = 0;

        protected string _name = "";

        protected object _tag = null;

        #endregion

        #region Properties

        public virtual int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public virtual object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        #endregion

        #region Methods

        public virtual void Serialize(BinSerializer serializer)
        {
            serializer.Serialize(ref _id);
            serializer.Serialize(ref _name);
        }

        #endregion
    }

    public class GameObjList : List<GameObj>, IDataSerializable
    {
        public Object Tag { get; set; }

        public new GameObj this[int idx]
        {
            get
            {
                try { return base[idx]; }
                catch { return null; }
            }
        }

        public void Serialize(BinSerializer serializer)
        {
            lock (this)
            {
                int itemCount = Count;
                serializer.Serialize(ref itemCount);

                if (serializer.Mode == SerializerMode.Read)
                {
                    Clear();

                    if (itemCount == 0)
                        return;

                    Type itemType = Type.GetType(serializer.Reader.ReadString());
                    for (int i = 0; i < itemCount; i++)
                    {
                        GameObj gameObj = Activator.CreateInstance(itemType) as GameObj;
                        gameObj.Serialize(serializer);

                        Add(gameObj);
                    }
                }
                else
                {
                    if (itemCount == 0)
                        return;

                    for (int i = 0; i < itemCount; i++)
                    {
                        if (i == 0)
                        {
                            Type itemType = this[i].GetType();
                            serializer.Writer.Write(itemType.FullName);
                        }

                        this[i].Serialize(serializer);
                    }
                }
            }
        }
    }

    public class GameObjCollection : KeyedCollection<int, GameObj>, IDataSerializable
    {
        #region Properties

        public Object Tag { get; set; }

        public new GameObj this[int key]
        {
            get
            {
                try { return base[key]; }
                catch { return null; }
            }
        }

        public Dictionary<int, string> ShortInfo
        {
            get
            {
                lock (this)
                {
                    return this.ToDictionary(obj => obj.Id, obj => obj.ToString());
                }
            }
        }

        #endregion

        #region Methods

        protected override int GetKeyForItem(GameObj item)
        {
            return item.Id;
        }

        public GameObj GetAt(int idx)
        {
            return Items.ElementAt(idx);
        }

        public int GetCount()
        {
            lock (this)
            {
                return this.Count;
            }
        }

        public int LockAdd(GameObj item)
        {
            lock (this)
            {
                try { Add(item); return this.Count; }
                catch { return -1; }
            }
        }

        public int LockRemove(int itemId)
        {
            lock (this)
            {
                if (Remove(itemId))
                    return this.Count;

                return -1;
            }
        }

        public int LockRemove(GameObj item)
        {
            lock (this)
            {
                if (Remove(item))
                    return this.Count;
                
                return -1;
            }
        }

        public void ForEach(Action<GameObj> action)
        {
            lock (this)
            {
                foreach (GameObj item in this)
                {
                    action(item);
                }
            }
        }

        public GameObj ForEachRet(Func<GameObj, GameObj> action)
        {
            lock (this)
            {
                foreach (GameObj item in this)
                {
                    GameObj retObj = action(item);
                    if (retObj != null)
                        return retObj;
                }

                return null;
            }
        }

        public void Serialize(BinSerializer serializer)
        {
            lock (this)
            {
                int itemCount = Count;
                serializer.Serialize(ref itemCount);

                if (serializer.Mode == SerializerMode.Read)
                {
                    Clear();

                    if (itemCount == 0)
                        return;

                    Type itemType = Type.GetType(serializer.Reader.ReadString());
                    for (int i = 0; i < itemCount; i++)
                    {
                        GameObj gameObj = (GameObj)Activator.CreateInstance(itemType);
                        gameObj.Serialize(serializer);

                        Add(gameObj);
                    }
                }
                else
                {
                    if (itemCount == 0)
                        return;

                    bool isFirst = true;

                    foreach (GameObj gameObj in this)
                    {
                        if (isFirst)
                        {
                            Type itemType = gameObj.GetType();
                            serializer.Writer.Write(itemType.FullName);
                            isFirst = false;
                        }

                        gameObj.Serialize(serializer);
                    }
                }
            }
        }

        #endregion
    }
}
