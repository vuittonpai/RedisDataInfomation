using Newtonsoft.Json;
using System.IO;

namespace RedisDataInfomation.Serializer
{
    public class NewtonJsonTester<TTestObject>
    {
        private readonly JsonSerializer jsonSerializer;
        private StreamReader streamReader;
        protected MemoryStream MemoryStream { get; set; }
        protected TTestObject TestObject { get; private set; }


        public NewtonJsonTester(TTestObject testObject)
  
        {
            this.TestObject = testObject;
            jsonSerializer = new JsonSerializer();
        }

        protected void Init()
        {
            //base.Init();
            streamReader = new StreamReader(this.MemoryStream);
        }

        public TTestObject Deserialize()
        {
            this.MemoryStream.Position = 0;
            var jsonTextReader = new JsonTextReader(streamReader) { CloseInput = false };

            return jsonSerializer.Deserialize<TTestObject>(jsonTextReader);
        }

        public  MemoryStream Serialize()
        {
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            jsonSerializer.Serialize(streamWriter, this.TestObject);
            streamWriter.Flush();

            return stream;
        }

        public void Dispose()
        {
            streamReader.Dispose();
        }
    }
}
