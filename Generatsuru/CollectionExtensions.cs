using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generatsuru
{
    public static class CollectionExtensions
    {
        public static void AddRange<K,V>(this Dictionary<K,V> dictionary, Dictionary<K,V> keyValuePairs)
        {
            foreach (var kvp in keyValuePairs)
            {
                dictionary.Add(kvp.Key, kvp.Value);
            }
        }
    }
}
