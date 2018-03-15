using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace RedisDataInfomation
{
    internal static class StackExchangeRedisExtenstion
    {
        /// <summary>
        /// 取得多筆Redis資料
        /// </summary>
        /// <typeparam name="T">資料型別</typeparam>
        /// <param name="cache">Redis</param>
        /// <param name="keys">Key List</param>
        /// <returns>T List</returns>
        internal static List<T> MGet<T>(this IDatabase cache, RedisKey[] keys)
        {
            List<T> returnValue = new List<T>();

            #region Redis Cluster
            /* Redis架構為Cluster無法使用多Key取值的暫時解法 */
            //foreach (RedisKey key in keys)
            //{
            //    var result = cache.StringGet(key);
            //    if (result != RedisValue.Null)
            //    {
            //        returnValue.Add(Deserialize<T>(result));
            //    }
            //}
            #endregion

            #region Redis Not Cluster
            RedisValue[] values = cache.StringGet(keys);
            if (values != null)
            {
                foreach (var i in values)
                {
                    if (i != RedisValue.Null)
                    {
                        returnValue.Add(Deserialize<T>(i));
                    }
                }
            }
            #endregion

            return returnValue;
        }

        /// <summary>
        /// 取得Redis資料
        /// </summary>
        /// <typeparam name="T">資料型別</typeparam>
        /// <param name="cache">Redis</param>
        /// <param name="key">Key</param>
        /// <returns>T</returns>
        internal static T Get<T>(this IDatabase cache, RedisKey key)
        {
            return Deserialize<T>(cache.StringGet(key));
        }

        /// <summary>
        /// 設定Redis資料
        /// </summary>
        /// <param name="cache">Redis</param>
        /// <param name="key">Key</param>
        /// <param name="value">要設定的資料</param>
        /// <param name="expiration">資料到期時間</param>
        internal static void Set(this IDatabase cache, RedisKey key, object value, TimeSpan expiration)
        {
            cache.StringSet(key, Serialize(value), expiration);
        }

        /// <summary>
        /// 設定Redis資料
        /// </summary>
        /// <param name="cache">Redis</param>
        /// <param name="key">Key</param>
        /// <param name="value">要設定的資料</param>
        internal static void Set(this IDatabase cache, RedisKey key, object value)
        {
            cache.StringSet(key, Serialize(value));
        }

        /// <summary>
        /// 設定多筆Redis資料
        /// </summary>
        /// <param name="cache">Redis</param>
        /// <param name="values">要設定的Key及資料</param>
        internal static void MSet(this IDatabase cache, IDictionary<RedisKey, object> values)
        {
            Dictionary<RedisKey, RedisValue> cacheData = new Dictionary<RedisKey, RedisValue>();
            foreach (var key in values.Keys)
            {
                cacheData.Add(key, Serialize(values[key]));
            }

            cache.StringSet(cacheData.ToArray());
        }

        /// <summary>
        /// 更新Redis資料
        /// </summary>
        /// <param name="cache">Redis</param>
        /// <param name="key">Key</param>
        /// <param name="oldValue">更新前的資料</param>
        /// <param name="newValue">更新後的資料</param>
        /// <param name="expiration">資料到期時間</param>
        /// <returns>是否更新成功(如更新期間資料已被異動則回傳失敗)</returns>
        internal static bool TryUpdate(this IDatabase cache, RedisKey key, object oldValue, object newValue, TimeSpan expiration)
        {
            var tran = cache.CreateTransaction();
            tran.AddCondition(Condition.StringEqual(key, Serialize(oldValue)));
            tran.StringSetAsync(key, Serialize(newValue), expiration);
            return tran.Execute();
        }

        /// <summary>
        /// List Left Push Value
        /// </summary>
        /// <param name="redis">Redis</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        internal static void ListLeftPush(this IDatabase redis, RedisKey key, object value)
        {
            redis.ListLeftPush(key, Serialize(value));
        }

        /// <summary>
        /// List Right Pop Value
        /// </summary>
        /// <typeparam name="T">Typeof Value</typeparam>
        /// <param name="redis">Redis</param>
        /// <param name="key">Key</param>
        /// <returns>Value</returns>
        internal static T ListRightPop<T>(this IDatabase redis, RedisKey key)
        {
            return Deserialize<T>(redis.ListRightPop(key));
        }


        private static byte[] Serialize(object o)
        {
            if (o == null)
            {
                return null;
            }

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, o);
                byte[] objectDataAsStream = memoryStream.ToArray();
                return objectDataAsStream;
            }
        }

        private static T Deserialize<T>(byte[] stream)
        {
            if (stream == null)
            {
                return default(T);
            }

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream(stream))
            {
                T result = (T)binaryFormatter.Deserialize(memoryStream);
                return result;
            }
        }


        /// <summary>
        /// 取得HashTable
        /// </summary>
        /// <param name="theCache"></param>
        /// <param name="theKey"></param>
        /// <returns></returns>
        public static Dictionary<string, T> GetHashTable<T>(this IDatabase theCache, string theKey)
        {
            
            Stopwatch watch = new Stopwatch();
            watch.Start();
            HashEntry[] tResult = theCache.HashGetAll(theKey);
            watch.Stop();
            //foreach (HashEntry tHE in tResult)
            //{
            //    Console.WriteLine(tHE.Name + ":" + tHE.Value);
            //}

            //var result = theCache.HashGetAll(theKey);

            Dictionary<string, T> returnValue = new Dictionary<string, T>();

            if (tResult != null)
            {
                foreach (var i in tResult)
                {
                    if (i.Value != RedisValue.Null)
                    {
                        returnValue.Add(i.Name, Deserialize<T>(i.Value));
                    }
                }
            }
            
            return returnValue;
        }
        public static HashEntry[] GetHashTable(this IDatabase theCache, string theKey)
        {
            HashEntry[] tResult = theCache.HashGetAll(theKey);
            byte[] size = theCache.KeyDump(theKey);
            return tResult;
        }

        /// <summary>
        /// 儲存HashTable
        /// </summary>
        /// <param name="theCache"></param>
        /// <param name="theKey"></param>
        /// <param name="theValue"></param>
        public static void SetHashTable(this IDatabase theCache, string theKey, HashEntry[] theValue)
        {
            theCache.HashSet(theKey, theValue);
        }
        /// <summary>
        /// 物件序列
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static HashEntry[] ToHashEntries(this object obj)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            return properties
                .Where(x => x.GetValue(obj) != null)
                .Select(property => new HashEntry(property.Name, property.GetValue(obj)
                .ToString())).ToArray();
        }

        /// <summary>
        /// 物件反序列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashEntries"></param>
        /// <returns></returns>
        public static T ConvertFromRedis<T>(this HashEntry[] hashEntries)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            var obj = Activator.CreateInstance(typeof(T));
            foreach (var property in properties)
            {
                HashEntry entry = hashEntries.FirstOrDefault(g => g.Name.ToString().Equals(property.Name));
                if (entry.Equals(new HashEntry())) continue;
                property.SetValue(obj, Convert.ChangeType(entry.Value.ToString(), property.PropertyType));
            }
            return (T)obj;
        }

        public static HashEntry[] ToHash<T>(this Dictionary<string, T> theCacheObject)
        {

            return theCacheObject.Select(i => new HashEntry(i.Key, Serialize(i.Value))).ToArray();
        }
    }
}

