using System.Collections.Generic;

namespace DEngine.Common
{
    public class StringMap<T> : Dictionary<string, T>
    {
        public new T this[string key]
        {
            get
            {
                if (ContainsKey(key))
                    return base[key];
                else
                    return default(T);
            }
            set
            {
                base[key] = value;
            }
        }
    }
}
