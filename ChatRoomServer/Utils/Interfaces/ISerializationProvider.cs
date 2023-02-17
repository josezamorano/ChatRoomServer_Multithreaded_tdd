namespace ChatRoomServer.Utils.Interfaces
{
    public interface ISerializationProvider
    {
        string SerializeObject<T>(T obj) where T : class;

        T DeserializeObject<T>(string obj) where T : class;
    }
}
