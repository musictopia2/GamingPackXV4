using System.Net;
using System.Net.NetworkInformation;
using System.Text;
namespace BasicGameFrameworkLibrary.Core.NetworkingClasses.Misc;
public static class GlobalStaticClasses
{
    public static string LocalIPAddress
    {
        get
        {
            var ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            return ipEntry.AddressList.Last().ToString();
        }
    }
    public static bool CanConnectToHomeGameServer
    {
        get
        {
            using Ping thisPing = new();
            string thisStr = "aa";
            var thisData = Encoding.ASCII.GetBytes(thisStr); //i like the idea of iconfiguration if i do redo so no longer hard coded.
            //if iconfiguration is used, then if nothing found, then would have to return false because information is not given.
            //i think that if anybody wants to do locally must use this one.
            string address = "192.168.0.150"; //i think.
            var results = thisPing.Send(address, 50, thisData);
            return results.Status == IPStatus.Success;
        }
    }
    public static string MainAzureHostAddress => "https://onlinegameserver.azurewebsites.net"; //this is default address for azure.
}