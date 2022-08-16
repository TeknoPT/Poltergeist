﻿using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using Poltergeist.PhantasmaLegacy.Core;
using Poltergeist.PhantasmaLegacy.Numerics;

namespace Poltergeist.PhantasmaLegacy.Storage.Context
{
    public struct StorageMap : IStorageCollection
    {
        public StorageMap(string baseKey, StorageContext context) : this(Encoding.UTF8.GetBytes(baseKey), context)
        {

        }

        public StorageMap(byte[] baseKey, StorageContext context) : this()
        {
            BaseKey = baseKey;
            Context = context;
        }

        public byte[] BaseKey { get; }
        public StorageContext Context { get; }
    }

    public static class MapUtils
    { 
/*        public readonly StorageContext Context;


        internal Map(StorageContext context, byte[] name, byte[] prefix)
        {
            this.Context = context;
            this.BaseKey = ByteArrayUtils.ConcatBytes(prefix, name);
        }
        */

        private static byte[] count_prefix = "{count}".AsByteArray();

        private static byte[] CountKey(byte[] baseKey)
        {
            return ByteArrayUtils.ConcatBytes(baseKey, count_prefix);
        }

        private static byte[] ElementKey<K>(byte[] baseKey, K index)
        {
            byte[] bytes = Serialization.Serialize(index);
            return ByteArrayUtils.ConcatBytes(baseKey, bytes);
        }

        private static byte[] MergeKey(byte[] parentKey, object childKey)
        {
            var bytes = Encoding.UTF8.GetBytes($".{childKey}");
            return ByteArrayUtils.ConcatBytes(parentKey, bytes);
        }

        public static BigInteger Count(this StorageMap map)
        {
            return map.Context.Get(CountKey(map.BaseKey)).AsBigInteger();
        }

        public static bool ContainsKey<K>(this StorageMap map, K key)
        {
            return map.Context.Has(ElementKey(map.BaseKey, key));
        }

        public static void Set<K, V>(this StorageMap map, K key, V value)
        {
            bool exists = map.ContainsKey(key);
            byte[] bytes;
            if (typeof(IStorageCollection).IsAssignableFrom(typeof(V)))
            {
                var collection = (IStorageCollection)value;
                //bytes = MergeKey(map.BaseKey, key);
                bytes = collection.BaseKey;
            }
            else
            {
                bytes = Serialization.Serialize(value);
            }
            map.Context.Put(ElementKey(map.BaseKey, key), bytes);

            if (!exists)
            {
                var size = map.Count() + 1;
                map.Context.Put(CountKey(map.BaseKey), size);
            }
        }

        public static void SetRaw(this StorageMap map, byte[] key, byte[] bytes)
        {
            bool exists = map.ContainsKey(key);
            map.Context.Put(ElementKey(map.BaseKey, key), bytes);

            if (!exists)
            {
                var size = map.Count() + 1;
                map.Context.Put(CountKey(map.BaseKey), size);
            }
        }

        public static V Get<K, V>(this StorageMap map, K key)
        {
            if (map.ContainsKey(key))
            {
                var bytes = map.Context.Get(ElementKey(map.BaseKey, key));

                if (typeof(IStorageCollection).IsAssignableFrom(typeof(V)))
                {
                    var args = new object[] { bytes, map.Context };
                    var obj = (V)Activator.CreateInstance(typeof(V), args);
                    return obj;
                }
                else
                {
                    return Serialization.Unserialize<V>(bytes);
                }
            }

            if (typeof(IStorageCollection).IsAssignableFrom(typeof(V)))
            {
                var baseKey = MergeKey(map.BaseKey, key);
                var args = new object[] { baseKey, map.Context };
                var obj = (V)Activator.CreateInstance(typeof(V), args);
                return obj;
            }

            return default(V);
        }

        public static byte[] GetRaw(this StorageMap map, byte[] key)
        {
            if (map.ContainsKey(key))
            {
                var bytes = map.Context.Get(ElementKey(map.BaseKey, key));
                return bytes;
            }

            return null;
        }

        public static void Remove<K>(this StorageMap map, K key)
        {
            if (map.ContainsKey(key))
            {
                map.Context.Delete(ElementKey(map.BaseKey, key));
                var size = map.Count() - 1;
                map.Context.Put(CountKey(map.BaseKey), size);
            }
        }

        public static void Visit<K,V>(this StorageMap map, Action<K,V> visitor)
        {
            var countKey = CountKey(map.BaseKey);
            var found = false;
            var countKeyRun = false;

            map.Context.Visit((key, value) =>
            {
                if (!found && key.SequenceEqual(countKey))
                {
                    countKeyRun = true;
                    found = true;
                }

                if (!countKeyRun)
                {
                    var k = Serialization.Unserialize<K>(key.Skip(map.BaseKey.Length).ToArray());
                    var v = Serialization.Unserialize<V>(value);
                    visitor(k, v);
                }
                else
                {
                    countKeyRun = false;
                }
            }, (uint)map.Count(), map.BaseKey);
        }

        public static V[] All<K,V>(this StorageMap map, K[] keys)
        {
            var size = keys.Length;
            var items = new V[size];
            for (int i = 0; i < size; i++)
            {
                items[i] = map.Get<K,V>(keys[i]);
            }
            return items;
        }

        public static V[] AllValues<V>(this StorageMap map)
        {
            var values = new List<V>();
            var countKey = CountKey(map.BaseKey);
            var found = false;
            var countKeyRun = false;

            map.Context.Visit((key, value) =>
            {
                if (!found && key.SequenceEqual(countKey))
                {
                    countKeyRun = true;
                    found = true;
                }

                if (!countKeyRun)
                {
                    V Val;
                    if (typeof(IStorageCollection).IsAssignableFrom(typeof(V)))
                    {
                        var args = new object[] { value, map.Context };
                        var obj = (V)Activator.CreateInstance(typeof(V), args);
                        Val = obj;
                        values.Add(Val);
                    }
                    else
                    {
                        Val = Serialization.Unserialize<V>(value);
                        values.Add(Val);
                    }
                }
                else
                {
                    countKeyRun = false;
                }
            }, (uint)map.Count(), map.BaseKey);

            return values.ToArray();
        }

        // TODO optimize this
        public static void Clear(this StorageMap map)
        {
            var keys = new List<byte[]>();
            var count = (uint)map.Count();

            map.Context.Visit((key, value) =>
            {
                keys.Add(key);
            }, count, map.BaseKey);

            Throw.If(keys.Count != count, "map.clear failed to fetch all existing keys");

            foreach (var key in keys)
            {
                map.Context.Delete(key);
            }

            map.Context.Put(CountKey(map.BaseKey), 0);
        }

    }
}
