using ChatRoomServer.Utils.Interfaces;
using Newtonsoft.Json;

namespace ChatRoomServer.DataAccessLayer.IONetwork
{
    public class SerializationProvider : ISerializationProvider
    {
        public string SerializeObject<T>(T obj) where T : class
        {
            try
            {
                string serializedObject = JsonConvert.SerializeObject(obj);
                return serializedObject;
            }
            catch (Exception ex)
            {
                return "ERROR: " + ex.ToString();
            }
        }

        public T DeserializeObject<T>(string obj) where T : class
        {
            var deserializedObject = JsonConvert.DeserializeObject<T>(obj);
            return deserializedObject;
        }
    }
}
