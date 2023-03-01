using ChatRoomServer.Services;
using ChatRoomServer.Utils.Interfaces;
using Newtonsoft.Json;

namespace ChatRoomServer.DataAccessLayer.IONetwork
{
    public class SerializationProvider : ISerializationProvider
    {
        //Tested
        public string SerializeObject<T>(T obj) where T : class
        {
            try
            {
                string serializedObject = JsonConvert.SerializeObject(obj);
                return serializedObject;
            }
            catch (Exception ex)
            {
                return Notification.Exception + ex.ToString();
            }
        }


        //Tested
        public T DeserializeObject<T>(string obj) where T : class
        {
            try
            {
                var deserializedObject = JsonConvert.DeserializeObject<T>(obj);
                return deserializedObject;
            }
            catch (Exception ex) 
            {
                return null;
            }
            
        }
    }
}
