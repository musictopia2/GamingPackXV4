using CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.Misc;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace BasicGameFrameworkLibrary.Core.NetworkingClasses.SocketClasses;

public class BasicGameServerTCP : ISerializable
{
    private TcpListener? _mainListen;
    private readonly Dictionary<string, ClientInfo> _playerList = new();
    private string _hostName = "";
    private readonly object _syncLock = new();
    public void StartServer()
    {
        _mainListen = new TcpListener(IPAddress.Any, 8010); //decided to use 8010 this time.
        _mainListen.Start();
        AcceptMain();
    }
    private async void AcceptMain()
    {
        await Task.Run(() =>
        {
            do
                try
                {
                    var thisClient = _mainListen!.AcceptTcpClient();
                    ProcessMainClientRequests(thisClient);
                }
                catch (Exception)
                {
                }
            while (true);
        });
    }
    private static void SendError(NetworkStream thisStream, string errorMessage)
    {
        SentMessage thisSend = new();
        thisSend.Status = "Connection Error";
        thisSend.Body = errorMessage;
        var results = js.SerializeObject(thisSend);
        var ends = NetworkStreamHelpers.CreateDataPacket(results);
        thisStream.Write(ends, 0, ends.Length);
        thisStream.Flush(); //i think this too.
    }
    private async void ProcessMainClientRequests(TcpClient thisClient)
    {
        byte[]? ends = default;
        string errorMessage = "";
        ClientInfo? thisInfo = default;
        await Task.Run(() =>
        {
            do
            {
                try
                {
                    var thisStream = thisClient.GetStream();
                    var readInt = thisStream.ReadByte();
                    if (readInt == -1)
                    {
                        break;
                    }
                    if (readInt == 2)
                    {
                        var data = NetworkStreamHelpers.ReadStream(thisStream);
                        var thisStr = Encoding.ASCII.GetString(data);
                        NetworkMessage thisMessage = js.DeserializeObject<NetworkMessage>(thisStr);
                        errorMessage = ""; //has to be proven an error message.
                        switch (thisMessage.NetworkCategory)
                        {
                            case EnumNetworkCategory.None:
                                break;
                            case EnumNetworkCategory.CloseAll:
                                _hostName = "";
                                lock (_syncLock)
                                {
                                    foreach (var item in _playerList.Values)
                                    {
                                        item.ThisStream!.Flush();
                                        item.ThisStream.Close();
                                        item.ThisStream.Dispose();
                                        item.Socket!.Close();
                                        item.Socket.Dispose();
                                    }
                                    _playerList.Clear();
                                }
                                break;
                            case EnumNetworkCategory.Hosting:
                                lock (_syncLock)
                                {
                                    if (string.IsNullOrWhiteSpace(thisMessage.YourNickName))
                                    {
                                        SendError(thisStream, "You must enter a host name");
                                        return;
                                    }
                                    _hostName = thisMessage.YourNickName; //i think the entire message will be nick name in this case.
                                    Console.WriteLine($"{_hostName} Is Hosting"); //to get hints.
                                    foreach (var item in _playerList.Values)
                                        item.ThisStream!.Dispose();
                                    _playerList.Clear();
                                    thisInfo = new();
                                    thisInfo.Socket = thisClient;
                                    thisInfo.ThisStream = thisStream;
                                    _playerList.Add(_hostName, thisInfo);
                                    SentMessage temp1 = new();
                                    temp1.Status = "hosting"; //i think.
                                    string str1 = js.SerializeObject(temp1);
                                    ends = NetworkStreamHelpers.CreateDataPacket(str1);
                                    thisStream.Write(ends, 0, ends.Length); //confirmation.
                                    thisStream.Flush();
                                }
                                break;
                            case EnumNetworkCategory.Client:
                                lock (_syncLock)
                                {
                                    string TempNick = thisMessage.YourNickName;
                                    if (_playerList.ContainsKey(TempNick) == false)
                                    {
                                        thisInfo = new();
                                        thisInfo.Socket = thisClient;
                                        thisInfo.ThisStream = thisStream;
                                        _playerList.Add(TempNick, thisInfo);
                                    }
                                    else
                                    {
                                        thisInfo = _playerList[TempNick];
                                        thisInfo.ThisStream = thisStream;
                                        thisInfo.Socket = thisClient;
                                    }
                                    SentMessage temp2 = new();
                                    temp2.Status = "clienthost";
                                    temp2.Body = _hostName;
                                    string str2 = js.SerializeObject(temp2);
                                    ends = NetworkStreamHelpers.CreateDataPacket(str2);
                                    thisStream.Write(ends, 0, ends.Length);
                                    thisStream.Flush();
                                }
                                break;
                            case EnumNetworkCategory.Message:
                                lock (_syncLock)
                                {
                                    if (thisMessage.SpecificPlayer != "")
                                    {
                                        if (_playerList.ContainsKey(thisMessage.SpecificPlayer) == false)
                                        {
                                            errorMessage = $"{thisMessage.SpecificPlayer} was not even registered to receive messages";
                                        }
                                        thisInfo = _playerList[thisMessage.SpecificPlayer];
                                        if (thisInfo.Socket!.Connected == false)
                                        {
                                            errorMessage = $"{thisMessage.SpecificPlayer} was disconnected even though it showed it was registered";
                                        }
                                        if (errorMessage != "")
                                        {
                                            SendError(thisStream, errorMessage);
                                            break;
                                        }
                                        ends = NetworkStreamHelpers.CreateDataPacket(thisMessage.Message);
                                        thisInfo.ThisStream!.Write(ends, 0, ends.Length);
                                        thisInfo.ThisStream.Flush(); //i think
                                        thisStream.Flush(); //i think here too.
                                        break;
                                    }
                                    //message gets sent to everybody except for self.
                                    BasicList<ClientInfo> SendTo = _playerList.Where(Items => Items.Key != thisMessage.YourNickName).Select(Temps => Temps.Value).ToBasicList();
                                    if (SendTo.Any(items => items.Socket!.Connected == false))
                                    {
                                        SendError(thisStream, "Somebody got disconnected when trying to send a message.");
                                        return;
                                    }
                                    SendTo.ForEach(items =>
                                    {
                                        ends = NetworkStreamHelpers.CreateDataPacket(thisMessage.Message);
                                        items.ThisStream!.Write(ends, 0, ends.Length);
                                        items.ThisStream.Flush(); //i think here too.
                                    });
                                    thisStream.Flush();
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    break; //not sure
                }
            }
            while (true);
        });
    }
}