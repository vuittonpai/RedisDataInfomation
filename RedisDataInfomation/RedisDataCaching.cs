using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedisDataInfomation
{
    public class RedisDataCaching<T>
    {
        public static string redisHost = "127.0.0.1";
        public IDatabase _redisDatabase;
        private RedisKey _redisKey = string.Empty;
        public IEnumerable<RedisKey> _allKeys;
        private bool _redisError = false;
        private bool _isDisconnect
        {
            get
            {
                if (_redisError) return true;

                bool isDisconnect = !_redisDatabase.IsConnected(_redisKey);
                if (isDisconnect && OnRedisDisconnect != null)
                {
                    OnRedisDisconnect(new Exception(string.Format("Redis connect fail, RedisKey:{0}", _redisKey.ToString())));
                }

                return isDisconnect;
            }
        }

        private static Lazy<ConnectionMultiplexer> _connection = new Lazy<ConnectionMultiplexer>(() =>
        {
            var configuration = new ConfigurationOptions
            {
                EndPoints = { redisHost , "6379"},
                ConnectTimeout = 10000000,
                ResponseTimeout = 10000000,
                SyncTimeout = 10000000

            };
            return ConnectionMultiplexer.Connect(configuration);
        }, LazyThreadSafetyMode.PublicationOnly);


        /// <summary>
        /// Initializes a new instance of the <see cref="RedisDataCaching<T>" /> class.
        /// </summary>
        /// <param name="cacheKey">Cache Key</param>
        /// <param name="intExpirationMinutes">Cache Expiration Minutes</param>
        public RedisDataCaching(string cacheKey = "", int intExpirationMinutes = 10000000)
        {
            this._redisKey = cacheKey;
            this._redisDatabase = GetDatabase();
            this._redisError = this._redisDatabase == null;
            this.Expiry = TimeSpan.FromMinutes(intExpirationMinutes);
            //this.OnRedisDisconnect += new RedisDataCaching<T>.RedisDisconnectHandler();
            
        }

        // <summary>
        /// Redis斷線事件的委派型別宣告
        /// </summary>
        /// <param name="ex">發生錯誤時所擲出的例外</param>
        public delegate void RedisDisconnectHandler(Exception ex);

        /// <summary>
        /// Redis Disconnect Event
        /// </summary>
        protected event RedisDisconnectHandler OnRedisDisconnect;

        /// <summary>
        /// Redis Cache Expiry
        /// </summary>
        protected TimeSpan Expiry { get; set; }

        /// <summary>
        /// 是否有資料
        /// </summary>
        /// <returns>Boolean</returns>
        public bool HasData
        {
            get
            {
                if (_redisError || _isDisconnect) return false;

                return _redisDatabase.KeyExists(_redisKey);
            }
        }

        public byte[] KeyDump(RedisKey k)
        {

            return _redisDatabase.KeyDump(k);
        }

        /// <summary>
        /// Get Redis Cache Object
        /// </summary>
        /// <param name="lstKeys">Cache Key List</param>
        /// <returns>Cache Object List</returns>
        public List<T> MGet(IEnumerable<string> lstKeys)
        {
            if (_redisError || _isDisconnect) return new List<T>();

            var keys = lstKeys.Select(key => (RedisKey)key).ToArray();

            try
            {
                return _redisDatabase.MGet<T>(keys);
            }
            catch
            {
                this.MDelete(lstKeys);
                throw;
            }
        }
        public string StringGets(RedisKey[] k)
        {
            return _redisDatabase.StringGet(k).ToString();
        }

        public string StringGet(RedisKey k)
        {
            return  _redisDatabase.StringGet(k).ToString();
        }

        public byte[] StringByte(RedisKey k)
        {
            return _redisDatabase.StringGet(k);
        }

        /// <summary>
        /// Get Redis Cache Object
        /// </summary>
        /// <returns>Cache Object</returns>
        public T Get(RedisKey k)
        {
            //if (_redisError || _isDisconnect) return GetCacheData();

            T result = default(T);
            try
            {
                result = _redisDatabase.Get<T>(k);
            }
            catch (TimeoutException tex)
            {
                //if (OnRedisDisconnect != null) OnRedisDisconnect(tex);
                //return GetCacheData();
            }
            catch (Exception ex)
            {
                if (OnRedisDisconnect != null) OnRedisDisconnect(ex);
            }

            if (result == null)
            {
                //result = GetCacheData();
                //if (result != null) Set(result);
            }

            return result;
        }

        /// <summary>
        /// 設定Object to Redis Cache
        /// </summary>
        /// <param name="cacheObject">Cache Object</param>
        public void Set(T cacheObject)
        {
            if (_redisError || _isDisconnect || string.IsNullOrEmpty(_redisKey)) return;

            _redisDatabase.Set(_redisKey, cacheObject, Expiry);
        }

        /// <summary>
        /// 設定Object to Redis Cache By Key
        /// </summary>
        /// <param name="cacheKey">Cache Key</param>
        /// <param name="cacheObject">Cache Object</param>
        public void Set(string cacheKey, T cacheObject)
        {
            if (_redisError || _isDisconnect || string.IsNullOrEmpty(cacheKey)) return;

            _redisDatabase.Set(cacheKey, cacheObject, Expiry);
        }

        /// <summary>
        /// 設定Object to Redis Cache No Expiry
        /// </summary>
        /// <param name="cacheObject">Cache Object</param>
        public void SetNoExpiry(T cacheObject)
        {
            if (_redisError || _isDisconnect || string.IsNullOrEmpty(_redisKey)) return;

            _redisDatabase.Set(_redisKey, cacheObject);
        }

        /// <summary>
        /// 設定多筆Object to Redis Cache
        /// </summary>
        /// <param name="values">KeyValue</param>
        public void MSet(IDictionary<string, T> values)
        {
            if (_redisError || _isDisconnect || values == null) return;

            Dictionary<RedisKey, object> cacheData = new Dictionary<RedisKey, object>();
            foreach (string key in values.Keys)
            {
                if (string.IsNullOrEmpty(key)) continue;

                cacheData.Add(key, values[key]);
            }

            if (!cacheData.Any()) return;

            _redisDatabase.MSet(cacheData);
        }

        /// <summary>
        /// 更新Cache資料
        /// </summary>
        /// <param name="oldCacheObject">更新前的Cache資料</param>
        /// <param name="newCacheObject">更新後的Cache資料</param>
        /// <returns>是否更新成功(如更新期間Cache資料已被異動則會更新失敗)</returns>
        public bool TryUpdate(T oldCacheObject, T newCacheObject)
        {
            if (_redisError || _isDisconnect || string.IsNullOrEmpty(_redisKey)) return false;

            return _redisDatabase.TryUpdate(_redisKey, oldCacheObject, newCacheObject, Expiry);
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            if (_redisError || _isDisconnect) return;

            Clear();
            DataBind();
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            if (_redisError || _isDisconnect) return;

            _redisDatabase.KeyDelete(_redisKey);
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="cacheKey">Cache Key</param>
        public void Delete(string cacheKey)
        {
            if (_redisError || _isDisconnect || string.IsNullOrEmpty(cacheKey)) return;

            _redisDatabase.KeyDelete(cacheKey);
        }

        /// <summary>
        /// 多筆刪除
        /// </summary>
        /// <param name="cacheKeys"></param>
        public void MDelete(IEnumerable<string> cacheKeys)
        {
            if (_redisError || _isDisconnect || !cacheKeys.Any()) return;

            _redisDatabase.KeyDelete(cacheKeys.Select(key => (RedisKey)key).ToArray());
        }

        /// <summary>
        /// 擊結Redis
        /// </summary>
        private void DataBind()
        {
            T cacheData = GetCacheData();
            if (cacheData != null) Set(cacheData);
        }


        /// <summary>
        /// 取得快取資料
        /// </summary>
        /// <returns>Cache Object</returns>
        //protected abstract T GetCacheData();
        public T GetCacheData()
        {

            return default(T);
        }
        /// <summary>
        /// Get Redis Database
        /// </summary>
        /// <returns>StackExchange.Redis.IDatabase</returns>
        private IDatabase GetDatabase()
        {
            IDatabase result = null;

            try
            {
                result = _connection.Value.GetDatabase();
                _allKeys =  _connection.Value.GetServer(redisHost, 6379).Keys();
                
            }
            catch (Exception ex)
            {

            }

            return result;
        }


        /// <summary>
        /// 清除指定的hashTable其中一個欄位
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool ClearCertainIdHash(string key, string field)
        {
            try
            {
                RedisValue rv = new RedisValue();
                rv = field;
                return _redisDatabase.HashDelete(key, rv);
            }
            catch (Exception e)
            {
                string paramStr = "key:" + key + " field:" + field;
                return false;
            }
        }

        /// <summary>
        /// 儲存HashTable
        /// </summary>
        /// <param name="theCacheKey"></param>
        /// <param name="theCacheObject"></param>
        public void SetHash(string theCacheKey, Dictionary<string, T> theCacheObject)
        {
            try
            {
                if (theCacheKey == string.Empty) return;
                Dictionary<RedisKey, T> cacheData = new Dictionary<RedisKey, T>();
                foreach (string key in theCacheObject.Keys)
                {
                    if (string.IsNullOrEmpty(key)) continue;

                    cacheData.Add(key, theCacheObject[key]);
                }

                _redisDatabase.SetHashTable(theCacheKey, theCacheObject.ToHash<T>());
                _redisDatabase.KeyExpire(theCacheKey, Expiry);
            }
            catch (Exception e)
            {
                string paramStr = "theCacheKey:" + theCacheKey + " HashEntry[]:" + theCacheObject;

            }
        }

        public void SetHash2(string theCacheKey, HashEntry[] theCacheObject)
        {
            try
            {
                //var yourset = new HashSet<List<FrontCategoryInfo>>(theCacheObject);

                if (theCacheKey == string.Empty) return;
                _redisDatabase.HashSet(theCacheKey, theCacheObject);
                _redisDatabase.KeyExpire(theCacheKey, Expiry);
            }
            catch (Exception e)
            {
                string paramStr = "theCacheKey:" + theCacheKey + " HashEntry[]:" + theCacheObject;

            }
        }


        public void SetHashTable(string theCacheKey, HashEntry[] theCacheObject)
        {
            try
            {
                if (theCacheKey == string.Empty) return;
                _redisDatabase.SetHashTable(theCacheKey, theCacheObject);
                _redisDatabase.KeyExpire(theCacheKey, Expiry);
            }
            catch (Exception e)
            {
                string paramStr = "theCacheKey:" + theCacheKey + " HashEntry[]:" + theCacheObject;

            }
        }

        /// <summary>
        /// 取得HashTable
        /// </summary>
        /// <param name="theCacheKey"></param>
        /// <returns></returns>
        public HashEntry[] GetHash(string theCacheKey)
        {
            try
            {
                if (theCacheKey == string.Empty) return null;
                return _redisDatabase.GetHashTable(theCacheKey);
            }
            catch (Exception e)
            {
                string paramStr = "theCacheKey:" + theCacheKey;

                return null;
            }
        }
        public Dictionary<string, T> GetHashT<T>(string theCacheKey)
        {
            if (theCacheKey == string.Empty) return default(Dictionary<string, T>);
            return _redisDatabase.GetHashTable<T>(theCacheKey);

        }

        public byte[] GetByte(string theCacheKey)
        {           
            return _redisDatabase.KeyDump(theCacheKey);
        }
        public HashEntry[] GetHByte(string theCacheKey)
        {
            var list =  _redisDatabase.HashGetAll(theCacheKey);
            return list;
        }

        /// <summary>
        /// init
        /// </summary>
        /// <param name="custKey1"></param>
        public void init(string custKey1)
        {

            HashEntry[] hashEntries =
            {
                new HashEntry("abc", "def"),
                new HashEntry("ghi", "jkl"),
                new HashEntry("mno", "pqr"),
            };
            _redisDatabase.HashSet(custKey1, hashEntries);
            // 依照custKey1取得對應hash值
            HashEntry[] tResult = _redisDatabase.GetHashTable(custKey1);

        }

        public static List<HashEntry> ConvenrtToRedis(Dictionary<RedisKey, T> instance)
        {
            var propertiesList = new List<HashEntry>();
            foreach (var prop in instance.GetType().GetProperties())
            {
                if (prop.Name.Equals(""))
                {
                    propertiesList.Add(new HashEntry(prop.Name, instance.GetType().GetProperty(prop.Name).GetValue(instance).ToString()));

                }
                else
                {
                    var subList = ConvenrtToRedis((Dictionary<RedisKey, T>)instance.GetType().GetProperty(prop.Name).GetValue(instance));
                    propertiesList.AddRange(subList);
                }
            }
            return propertiesList;
        }


    }
}
