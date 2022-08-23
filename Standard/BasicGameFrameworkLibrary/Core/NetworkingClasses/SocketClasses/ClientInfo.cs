using System.Net.Sockets; //not common enough to put under global usings for sure.
namespace BasicGameFrameworkLibrary.Core.NetworkingClasses.SocketClasses;

internal class ClientInfo
{
    public TcpClient? Socket { get; set; }
    public NetworkStream? ThisStream { get; set; }
}