using ChatRoomServer.Utils.Interfaces;
using System.Net;

namespace ChatRoomServer.DataAccessLayer.IONetwork
{
    public class DnsProvider :IDnsProvider
    {

        public IPHostEntry GetDnsHostEntry()
        {
           var result = Dns.GetHostEntry(Dns.GetHostName());
            return result;
        }
    }
}
