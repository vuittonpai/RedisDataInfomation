
using ETMall.WS.DataClass.Response.Category.Entity;
using RedisCacheDemo;
using RedisDataInfomation.Repository;
using RedisDataInfomation.Serializer;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RedisDataInfomation
{
    class Program
    {
        
        //public static string redisHost = "127.0.0.1"; 
        //public static string redisPort = "6379"; 
        public static string FilePathToStoreResult = "E:\\RedisDataSize.txt";

        


        static void Main(string[] args)
        {
            //findlen();
            //findHashValue();
            TestPerformance();
        }
        static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
        static double ConvertBytesToKilobytes(long bytes)
        {
            return (bytes / 1024f);
        }

        static void findlen()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@FilePathToStoreResult, true))
            {
                file.WriteLine("開始時間 : " + DateTime.Now);
            }

            var redisClient = new RedisDataCaching<string>("Categories_Dictionary");
      

            //using (var redisClient = new RedisClient(redisHost, Convert.ToInt16(redisPort)))
            //{
            double totalsize = 0;
            var original = redisClient._allKeys;
            //var keys = original.ToList();
            //var arrayKeys = original.Where(e=>e.ToString().Contains("Category")).ToArray();

            Stopwatch watch = new Stopwatch();
            //watch.Start();
            //var resultData = redisClient.StringGets(arrayKeys);
            //watch.Stop();
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@FilePathToStoreResult, true))
            {
                //file.WriteLine("Key Count : " + arrayKeys.Count() + " Key MGet : " + watch.ElapsedMilliseconds);
            }

            string key = "Categories_Dictionary";
            //foreach (string key in arrayKeys)
            //{
                    try
                    {
                        //Stopwatch watch = new Stopwatch();
                        watch.Start();

                var resultData = redisClient.StringGet((RedisKey) key);
     
                        watch.Stop();

                        byte[] bytarr = redisClient.StringByte((RedisKey)key);
                        //byte[] bytarr1 = redisClient.GetByte((RedisKey)key); //使用dump跟原本的stringGet取得的byte會不同，不確定哪個對。
                        double kblen = ConvertBytesToKilobytes(bytarr.Length);
                        double mblen = ConvertBytesToMegabytes(bytarr.Length);
                        totalsize = totalsize + mblen;
                        Console.WriteLine("Key Name : " + key + " Key length in MB : " + mblen + " Key Length in Kb : " + kblen);
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@FilePathToStoreResult, true))
                        {
                            file.WriteLine("Key名稱 : " + key + " 資料的大小MB : " + mblen + " 存取時間MS : " + watch.ElapsedMilliseconds);
                        }

                    }
                    catch (Exception ex)
                    {
                        //try
                        //{
                        //    byte[][] bythsharr = redisClient.HGetAll(key);
                        //    double kblen = ConvertBytesToKilobytes(bythsharr.Length);
                        //    double mblen = ConvertBytesToMegabytes(bythsharr.Length);
                        //    Console.WriteLine("Hash Key Name : " + key + " Key length in MB : " + mblen + " Key Length in Kb : " + kblen);
                        //    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@FilePathToStoreResult, true))
                        //    {
                        //        file.WriteLine("Hash Key Name : " + key + " Key length in MB : " + mblen + " Key Length in Kb : " + kblen);
                        //    }
                        //    totalsize = totalsize + mblen;
                        //}
                        //catch (Exception ex1)
                        //{

                        //}
                    }
                //}
            //}
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@FilePathToStoreResult, true))
            {
                file.WriteLine("結束時間 : " + DateTime.Now);
            }
        }

        static void findHashValue()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@FilePathToStoreResult, true))
            {
                file.WriteLine("開始時間 : " + DateTime.Now);
            }

            var redisClient = new RedisDataCaching<FrontCategoryInfo>("Categories_Dictionary");


            //using (var redisClient = new RedisClient(redisHost, Convert.ToInt16(redisPort)))
            //{
            double totalsize = 0;
            var original = redisClient._allKeys;
            //var keys = original.ToList();
            //var arrayKeys = original.Where(e=>e.ToString().Contains("Category")).ToArray();

            Stopwatch watch = new Stopwatch();
            //watch.Start();
            //var resultData = redisClient.StringGets(arrayKeys);
            //watch.Stop();
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@FilePathToStoreResult, true))
            {
                //file.WriteLine("Key Count : " + arrayKeys.Count() + " Key MGet : " + watch.ElapsedMilliseconds);
            }

            string key = "HashCategory";
            //foreach (string key in arrayKeys)
            //{
            double totalMB = 0;
            try
            {
                //Stopwatch watch = new Stopwatch();
                watch.Start();
                var resultData = redisClient.GetHashT<FrontCategoryInfo>((RedisKey)key);

                watch.Stop();
                
                HashEntry[] byt = redisClient.GetHByte((RedisKey)key);
                foreach (var bytarr in byt)
                {
                    var terrr = (byte[])bytarr.Value;
                    double kblen = ConvertBytesToKilobytes(terrr.Length);
                    double mblen = ConvertBytesToMegabytes(terrr.Length);
                    totalsize = totalsize + mblen;
                    Console.WriteLine("Key Name : " + key + " Key length in MB : " + mblen + " Key Length in Kb : " + kblen);
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@FilePathToStoreResult, true))
                    {
                        file.WriteLine("Key名稱 : " + key + " 資料的大小MB : " + mblen + " 存取時間MS : " + watch.ElapsedMilliseconds);
                    }
                    totalMB = totalMB +  mblen;
                }
            }
            catch (Exception ex)
            {
                
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@FilePathToStoreResult, true))
            {
                file.WriteLine("結束時間 : " + DateTime.Now + "mb: " + totalMB);
            }
        }

        static void TestPerformance()
        {
            var redisClient = new RedisDataCaching<FrontCategoryInfo>("Hash_Categories");
            
            var db = redisClient._redisDatabase;

            //string demo

            string inputStringData = "A little bit of string  data.";

            db.StringSet("StringData", inputStringData);

            string outputStringData = db.StringGet("StringData");

            db.KeyDelete("StringData");

            //integer demo

            int inputIntData = 3855;

            db.StringSet("IntData", inputIntData);

            int outputIntData = (int)db.StringGet("IntData");

            db.KeyDelete("IntData");

            //hash demo

            var redisObjectStore = new RedisObjectStore<Dictionary<string, List<FrontCategoryInfo>>>(db);

            //var objDataIn = new ExampleData { Reference = Guid.NewGuid(), IntegerData = 4967, StringData = "This is an example of how to store object data in a hash" };

            //redisObjectStore.Save(objDataIn.Reference.ToString(), objDataIn);

            //var objDataOut = redisObjectStore.Get2("Hash_Categories");

            //redisObjectStore.Delete(objDataIn.Reference.ToString());


            // 

            var repo = new CategoryRepository();

            var result = repo.SetCategoryDicData();
            //var json = new NewtonJsonTester<Dictionary<string, List<FrontCategoryInfo>>>(result);
            //var Stream = json.Serialize();
            //Newtonsoft.Json.Serialization();
            var cate = result["4"];
            var cate2 = result["0"];
            

            redisObjectStore.Save("PerformanceTest", result);
            var objDataOut = redisObjectStore.Get2("PerformanceTest");
            redisObjectStore.Delete("PerformanceTest");


        }
    }
}
