namespace MultiplayerSignalRHubClasses;
public class MultiplayerConnectionHub : Hub, ISerializable
{
    private static readonly ConcurrentDictionary<string, ConnectionInfo> _playerList = new();
    private static string _hostName = "";
    private static string _hostGame = "";
    private static bool _gameStarted = false;
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        ConnectionInfo thisCon = _playerList!.Where(x => x.Value.ConnectionID == Context.ConnectionId).Select(xx => xx.Value).SingleOrDefault()!;
        if (thisCon != null)
        {
            thisCon.IsConnected = false; //no longer connected
            if (thisCon.UserID == _hostName)
            {
                _hostName = "";
                _hostGame = "";
                _gameStarted = false; //game not started anymore.
            }
        }
        await base.OnDisconnectedAsync(exception);
    }
    private async Task SendErrorAsync(string errorMessage)
    {
        await Clients.Caller.SendAsync("ConnectionError", errorMessage);
    }

    private readonly static BasicList<string> _excludeList = new();
    public static BasicList<string> ExcludedList => _excludeList; //if there is any on the list, then only those can host.  this would stop the problems where 2 people are hosting the same time.
    public void BackToMain(string nickName)
    {
        //don't worry about game anymore.
        ConnectionInfo thisCon = _playerList!.Where(x => x.Value.ConnectionID == Context.ConnectionId).Select(xx => xx.Value).SingleOrDefault()!;
        if (thisCon is not null)
        {
            thisCon.IsConnected = false; //no longer connected because you are going back to main.
        }
        if (nickName == _hostName)
        {
            _hostName = "";
            _hostGame = "";
            _gameStarted = false;
            //try to not automatically disconnect them anymore.  that way i can attempt to make it where the host can rejoin game later.

            //foreach (var item in _playerList)
            //{
            //    item.Value.IsConnected = false; //act like its not connected.  try this way to start with.
            //}
        }
        //i don't think the client part has the problem.
    }
    //hopefully this is okay because somebody has to request to close connection.  if i am wrong, rethink.
    public void CloseConnection()
    {
        //you already know the connectionid.  will literally delete from the playerlist now.  that would be best.
        ConnectionInfo thisCon = _playerList!.Where(x => x.Value.ConnectionID == Context.ConnectionId).Select(xx => xx.Value).SingleOrDefault()!;
        if (thisCon is not null)
        {
            if (thisCon.UserID == _hostName)
            {
                _hostName = "";
                _hostGame = "";
                _gameStarted = false;
                _playerList.Clear(); //try to clear at this point if the host name matches.  gives extra options.
            }
        }
    }
    public async Task HostingAsync(string nickName, string gameName) //send message only to user.
    {
        //we can't clear the playerlist anymore.  because if its cleared, then clients can't connect first.
        //for now, will also show the game.
        // _playerList.Clear();
        if (nickName == "")
        {
            await SendErrorAsync("You must have a nick name in order to host");
            return;
        }
        if (gameName == "")
        {
            await SendErrorAsync("You must now enter the game name in order to host");
            return; //this means i have to break all older versions of game package.
        }
        if (ExcludedList.Count > 0)
        {
            if (ExcludedList.Any(xx => xx.ToLower() == nickName.ToLower()) == false)
            {
                await SendErrorAsync("You do not have permission to host game");
                return;
            }
        }
        ConnectionInfo connect = new()
        {
            ConnectionID = Context.ConnectionId,
            UserID = nickName,
            GameName = gameName,
            IsConnected = true
        };
        _hostName = nickName;
        _hostGame = gameName;
        _gameStarted = false; //hopefully can later restart a game (?)
        _playerList.AddOrUpdate(_hostName, connect, (key, temps) =>
        {
            return connect;
        });


        //_playerList.TryAdd(nickName, connect);
        //when host gets the hosting message, the host can decide what to do.
        await Clients.Caller.SendAsync("Hosting"); //all you need to know is you are hosting now.
        var list = _playerList.Where(xx => xx.Value.IsConnected && xx.Value.UserID != _hostName);
        foreach (var player in list)
        {
            await Clients.Client(player.Value.ConnectionID).SendAsync("HostName", _hostName); //hopefully this is what is needed (?)
            //hopefully does not hurt when host rejoining (?)
        }
    }
    public async Task RestoreGameForReconnectionAsync(string nickNameReconnected) //iffy now (?)
    {
        if (_hostName == "")
        {
            await SendErrorAsync("Nobody is even hosting to restore game for reconnections");
            return;
        }
        string connectionId = Context.ConnectionId;
        var connectionInfo = _playerList.SingleOrDefault(xx => xx.Value.ConnectionID == connectionId).Value;
        if (connectionInfo is null)
        {
            await SendErrorAsync("Could not find the connection info to even restore game for reconnection");
            return;
        }
        if (connectionInfo.UserID != _hostName)
        {
            await SendErrorAsync("Only the host can restore game for reconnection");
            return;
        }
        BasicList<ConnectionInfo> sendTo = _playerList.Where(x => x.Key != _hostName && x.Value.IsConnected == true && x.Key != nickNameReconnected).Select(xx => xx.Value).ToBasicList();
        await sendTo.ForEachAsync(async x =>
        {
            await Clients.Client(x.ConnectionID).SendAsync("RestoreForReconnection");
        });
    }
    public async Task EndGameEarlyAsync(string nickNameReconnected)
    {
        if (_hostName == "")
        {
            await SendErrorAsync("Nobody is even hosting to end game early");
            return;
        }
        string connectionId = Context.ConnectionId;
        var connectionInfo = _playerList.SingleOrDefault(xx => xx.Value.ConnectionID == connectionId).Value;
        if (connectionInfo is null)
        {
            await SendErrorAsync("Could not find the connection info to even end game early");
            return;
        }
        if (connectionInfo.UserID != _hostName)
        {
            await SendErrorAsync("Only the host can end game early");
            return;
        }
        BasicList<ConnectionInfo> sendTo = _playerList.Where(x => x.Key != _hostName && x.Value.IsConnected == true && x.Key != nickNameReconnected).Select(xx => xx.Value).ToBasicList();
        await sendTo.ForEachAsync(async x =>
        {
            await Clients.Client(x.ConnectionID).SendAsync("EndGameEarly");
        });
    }
    //what should ideally happen is as follows:
    //if a host wants to rejoin, since the host has the autoresume information, they can just reload that autoresume information.
    //get a list of players.
    //if the player is not connected, then that player can rejoin like normal.
    //if that happens, maybe no need for changes to hostrejoingame.
    //in that case, i can wait before have new version of the andycristina service.


    //public async Task HostRejoinGameAsync()
    //{
    //    if (_hostName == "")
    //    {
    //        await SendErrorAsync("Nobody is even hosting to get a list of players to rejoin game");
    //        return;
    //    }
    //    BasicList<string> list = _playerList.Where(x => x.Key != _hostName && x.Value.IsConnected == true).Select(x => x.Key).ToBasicList();
    //    if (list.Count == 0)
    //    {
    //        await SendErrorAsync("There was no players for rejoining");
    //        return;
    //    }
    //    string text = await js.SerializeObjectAsync(list);
    //    await Clients.Caller.SendAsync("RejoinedPlayers", text); //will get a list of rejoined players.
    //}
    public async Task StartGameAsync()
    {
        if (_hostName == "")
        {
            await SendErrorAsync("Nobody is even hosting to start game");
            return;
        }
        //don't trust the clients.  if the one sending is not host, then ignore.
        //i propose a message to make sure they know there is a serious bug.
        string connectionId = Context.ConnectionId;
        var connectionInfo = _playerList.SingleOrDefault(xx => xx.Value.ConnectionID == connectionId).Value;
        if (connectionInfo is null)
        {
            await SendErrorAsync("Could not find the connection info to even start game");
            return;
        }
        if (connectionInfo.UserID != _hostName)
        {
            await SendErrorAsync("Only the host can start the game");
            return;
        }
        _gameStarted = true;
    }
    public async Task DisconnectEverybodyAsync()
    {
        if (_hostName == "")
        {
            await SendErrorAsync("Nobody is hosting to even allow disconnecting everybody");
        }
        ConnectionInfo connect = new()
        {
            ConnectionID = Context.ConnectionId,
            UserID = _hostName,
            GameName = _hostGame,
            IsConnected = true
        };
        foreach (var item in _playerList)
        {
            if (item.Value.IsConnected && item.Key != _hostName)
            {
                //await Clients.Client(thisInfo.ConnectionID).SendAsync("ReceiveMessage", thisMessage.Message);
                await Clients.Client(item.Value.ConnectionID).SendAsync("Close");
                //await Clients.Caller.SendAsync("ConnectionError", errorMessage);
            }
        }
        _playerList.Clear();
        _playerList.AddOrUpdate(_hostName, connect, (key, temps) =>
        {
            return connect;
        });
    }
    public async Task ReconnectionAsync(string nickName)
    {
        if (_playerList.ContainsKey(nickName) == true)
        {
            var connect = _playerList[nickName];
            connect.ConnectionID = Context.ConnectionId;
            connect.IsConnected = true;
        }
        else
        {
            await SendErrorAsync($"The name {nickName} was never connected"); //i think no problem showing popup.
        }
    }

    //the purpose of game name is to make sure if a client connects to wrong game, they will get an error.
    public async Task ClientConnectingAsync(string nickName, string gameName)
    {
        //if (_hostName == "")
        //{
        //    await Clients.Caller.SendAsync("NoHost"); //because nobody is hosting.
        //    return;
        //}
        //Console.WriteLine("Client Connecting");
        try
        {
            if (nickName == "")
            {
                await SendErrorAsync("You must have a nick name in order to connect to host");
                return;
            }
            if (gameName == "")
            {
                await SendErrorAsync("You must now enter the game name in order to connect to host");
            }
            if (_hostGame != "" && _hostGame != gameName)
            {
                await SendErrorAsync($"The host chose {_hostGame} but you chose {gameName}");
                //if you get the error, you have to go back to main to choose different game (that is the solution to that problem).
                return;
            }
            ConnectionInfo connect;
            if (_playerList.ContainsKey(nickName) == false)
            {
                //this happened too often.


                //await SendErrorAsync("You are already registered.  Rethink if intended this time");
                //return;
                connect = new()
                {
                    UserID = nickName
                };
                _playerList.TryAdd(nickName, connect);
            }
            else
            {
                connect = _playerList[nickName];


            }
            connect.IsConnected = true;
            connect.ConnectionID = Context.ConnectionId;
            connect.GameName = gameName; //you can still show as connected even if there is no host.  the host may appear later.
            //even if no host, can have host later though.
            if (_hostName != "" && _gameStarted == false)
            {
                await Clients.Caller.SendAsync("HostName", _hostName);
            }
            else if (_gameStarted == false)
            {
                //can be iffy now.
                await Clients.Caller.SendAsync("NoHost");
                //if nobody is hosting, they may be hosting eventually.
            }
            else if (_hostName == "")
            {
                await SendErrorAsync("The host was blank even though a game has started.  This is wrong");
            }
            else
            {
                //this means send message to host.  so host can send information to client.
                //obviously there has to be a host.
                await Clients.Caller.SendAsync("WaitForGame"); //this means its going back into the game.

                connect = _playerList[_hostName];

                await Clients.Client(connect.ConnectionID).SendAsync("GameState", nickName); //even if its not the hosts turn, the host has to send the state so the clients can join in and play.
                //i think its best for the host to know who reconnected so they send game state just to that player.


            }
        }
        catch (Exception ex)
        {
            await SendErrorAsync(ex.Message); //to get hints.
        }

    }
    //this means work needs to be done on the client as well.
    //the server knows if it gets disconnected, then go ahead and close all.
    //however, otherwise, don't do it.
    //otherwise, probably cannot make it where host does not have to start first.
    //player can change their minds (maybe).
    //private async Task CloseAllAsync()
    //{
    //    _hostName = "";
    //    _hostGame = "";
    //    _gameStarted = false;
    //    await Clients.Others.SendAsync("Close"); //i think.
    //    _playerList.Clear(); //just act like nobody is connected anymore.
    //}
    public async Task SendMessageAsync(string tempMessage, string gameName)
    {
        //since there is a ready message, this means i am unable to use this to show its in game.

        string errorMessage = "";
        //needs another parameter now.
        ConnectionInfo thisInfo;
        NetworkMessage thisMessage = await js.DeserializeObjectAsync<NetworkMessage>(tempMessage);
        //still only supports one game (to make it simplier).
        if (gameName != _hostGame && _hostGame != "")
        {
            await SendErrorAsync($"The host game was {_hostGame} but the game sent was {gameName}");
            return;
        }
        if (_playerList.IsEmpty)
        {
            await SendErrorAsync("There are no players to send messages to");
            return;
        }
        if (thisMessage.SpecificPlayer != "")
        {
            //for now, when sending to specific player, this should be okay.  may have to rethink eventually.
            if (_playerList.ContainsKey(thisMessage.SpecificPlayer) == false)
            {
                errorMessage = $"{thisMessage.SpecificPlayer} was not even registered to receive messages";
            }
            thisInfo = _playerList[thisMessage.SpecificPlayer];
            if (thisInfo.IsConnected == false)
            {
                errorMessage = $"{thisMessage.SpecificPlayer} was disconnected even though it showed it was registered";
            }
            if (errorMessage != "")
            {
                await SendErrorAsync(errorMessage);
                return;
            }
            await Clients.Client(thisInfo.ConnectionID).SendAsync("ReceiveMessage", thisMessage.Message);
            return; //i think
        }
        BasicList<ConnectionInfo> sendTo = _playerList.Where(x => x.Key != thisMessage.YourNickName && x.Value.IsConnected == true).Select(xx => xx.Value).ToBasicList();
        //ignore anybody who is not connected.  when they connect, will get the state.


        //if (SendTo.Any(Items => Items.IsConnected == false))
        //{
        //    await SendErrorAsync("Somebody got disconnected when trying to send a message.");
        //    return;
        //}
        await sendTo.ForEachAsync(async x =>
        {
            await Clients.Client(x.ConnectionID).SendAsync("ReceiveMessage", thisMessage.Message);
        });
    }
}
