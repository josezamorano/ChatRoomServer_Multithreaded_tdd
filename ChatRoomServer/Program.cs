using Autofac;
using ChatRoomServer.Utils.DependencyInjection;
using ChatRoomServer.Utils.Interfaces;

namespace ChatRoomServer
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Autofac.IContainer container = ContainerConfig.Configure();

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            IServerManager _serverManager = container.Resolve<IServerManager>();
            IInputValidator _inputValidator = container.Resolve<IInputValidator>();
            IChatRoomManager _chatRoomManager = container.Resolve<IChatRoomManager>();

            Application.Run(new PresentationLayer(_serverManager , _inputValidator , _chatRoomManager));
        }
    }
}