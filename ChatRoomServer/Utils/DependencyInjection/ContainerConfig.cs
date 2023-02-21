using Autofac;
using ChatRoomServer.DataAccessLayer.IONetwork;
using ChatRoomServer.DomainLayer;
using ChatRoomServer.Services;
using ChatRoomServer.Utils.Interfaces;

namespace ChatRoomServer.Utils.DependencyInjection
{
    public static class ContainerConfig
    {
        public static Autofac.IContainer Configure()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<ChatRoomManager>().As<IChatRoomManager>();
            builder.RegisterType<ClientAction>().As<IClientAction>();
            builder.RegisterType<InputValidator>().As<IInputValidator>();
            builder.RegisterType<MessageDispatcher>().As<IMessageDispatcher>();
            builder.RegisterType<ObjectCreator>().As<IObjectCreator>();
            builder.RegisterType<SerializationProvider>().As<ISerializationProvider>();
            builder.RegisterType<ServerManager>().As<IServerManager>();
            builder.RegisterType<StreamProvider>().As<IStreamProvider>();
            builder.RegisterType<Transmitter>().As<ITransmitter>();



            return builder.Build();
        }
    }
}
