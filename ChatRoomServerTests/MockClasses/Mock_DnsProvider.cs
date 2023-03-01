using ChatRoomServer.Utils.Interfaces;
using System.Net;
using System.Net.Sockets;

namespace ChatRoomServerTests.MockClasses
{
    public class Mock_DnsProvider : IDnsProvider
    {
        public IPHostEntry GetDnsHostEntry()
        {

            var ipHostEntry = new IPHostEntry();
            long value = 567890;
            IPAddress address1 = new IPAddress(value);
            //address1.AddressFamily = AddressFamily.InterNetwork;
            ipHostEntry.AddressList = new IPAddress[] {address1 };
            return ipHostEntry;
        }
    }
}
