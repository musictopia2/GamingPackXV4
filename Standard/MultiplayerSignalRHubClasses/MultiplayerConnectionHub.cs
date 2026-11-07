namespace MultiplayerSignalRHubClasses;
public class MultiplayerConnectionHub : Hub, ISerializable
{
    private static readonly ConcurrentDictionary<string, ConnectionInfo> _playerList = new();
    private static string _hostName = "";
    private static string _hostGame = "";
    private static bool _gameStarted = false;
    private static bool _hostRecovering;
    private static int _maxPlayers;
    private static long _nextWaitingOrder;
    private static string _sessionHostName = "";
    private static string _sessionGame = "";
    private static string _nextGameHost = "";
    private static readonly HashSet<string> _nextGamePlayers = new(StringComparer.Ordinal);
    private static readonly HashSet<string> _reservedPlayers = new(StringComparer.Ordinal);
    // Frozen when play starts; pending/rejected clients are never participants.
    private static readonly HashSet<string> _startedPlayers = new(StringComparer.Ordinal);


    private static long _nextMessageSequence;
    private static readonly List<SentMessage> _hostMessageJournal = [];
    private const string _waitingForHostedGameGroup = "WaitingForHostedGame";

    private static readonly SemaphoreSlim _joinLock = new(1, 1);


    private static string _networkSessionId =
        Guid.NewGuid().ToString("N");

    // Caller must hold _joinLock.
    private static async Task PrepareMessageAsync(
        NetworkMessage message,
        string senderName)
    {
        SentMessage data =
            await js1.DeserializeObjectAsync<SentMessage>(message.Message);

        // Always assign this on the server.
        data.NetworkSessionId = _networkSessionId;
        data.SequenceNumber = ++_nextMessageSequence;

        message.Message = await js1.SerializeObjectAsync(data);
        message.Message = await js1.SerializeObjectAsync(data);

        bool intendedForHost =
            senderName != _sessionHostName &&
            (message.SpecificPlayer == "" ||
             message.SpecificPlayer == _sessionHostName);

        if (intendedForHost)
        {
            _hostMessageJournal.Add(data);
        }
    }

    public async Task WaitForHostedGameAsync()
    {
        await _joinLock.WaitAsync();
        try
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                _waitingForHostedGameGroup);

            if (_hostGame != "")
            {
                await Clients.Caller.SendAsync("HostedGameChosen", _hostGame);
            }
        }
        finally
        {
            _joinLock.Release();
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await _joinLock.WaitAsync();
        try
        {
            ConnectionInfo? connection = FindCaller();
            if (connection is not null)
            {
                connection.IsConnected = false;
                // Keep admission and the started roster for client reconnections.
                if (connection.UserID == _hostName)
                {
                    ClearHost();
                }
            }
        }
        finally
        {
            _joinLock.Release();
        }
        await base.OnDisconnectedAsync(exception);
    }
    private async Task SendErrorAsync(string errorMessage)
    {
        await Clients.Caller.SendAsync("ConnectionError", errorMessage);
    }
    public static BasicList<string> AllowedHosts { get; } = []; //if there is any on the list, then only those can host.  this would stop the problems where 2 people are hosting the same time.
    public async Task BackToMain(string nickName)
    {
        await _joinLock.WaitAsync();
        try
        {
            ConnectionInfo? connection = FindCaller();
            if (connection is null)
            {
                return;
            }
            connection.IsConnected = false;
            // Use the calling connection, not a supplied nickname, to identify the host.
            if (connection.UserID == _hostName)
            {
                ClearHost();
            }
        }
        finally
        {
            _joinLock.Release();
        }
    }
    //hopefully this is okay because somebody has to request to close connection.  if i am wrong, rethink.
    public async Task CloseConnection()
    {
        await _joinLock.WaitAsync();
        try
        {
            ConnectionInfo? connection = FindCaller();
            if (connection is not null)
            {
                connection.IsConnected = false;
                if (connection.UserID == _hostName)
                {
                    ClearHost();
                    ClearHostMessageJournal();
                    _playerList.Clear();
                    _gameStarted = false;
                    _startedPlayers.Clear();
                    _sessionHostName = "";
                    _sessionGame = "";
                    _reservedPlayers.Clear();
                    _nextGameHost = "";
                    _nextGamePlayers.Clear();
                }
            }
        }
        finally
        {
            _joinLock.Release();
        }
    }


    private static string GetNextGameGroup(string hostName)
    {
        return $"WaitingForNextGame:{hostName.ToUpperInvariant()}";
    }

    public async Task WaitForNextHostedGameAsync(string hostName)
    {
        await _joinLock.WaitAsync();
        try
        {
            if (string.IsNullOrWhiteSpace(hostName))
            {
                await SendErrorAsync("A host name is required.");
                return;
            }

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                GetNextGameGroup(hostName));

            // Covers clients that arrive after the host has chosen.
            if (_hostGame != "" &&
                string.Equals(_hostName, hostName,
                    StringComparison.OrdinalIgnoreCase))
            {
                await Clients.Caller.SendAsync("HostedGameChosen", _hostGame);
            }
        }
        finally
        {
            _joinLock.Release();
        }
    }

    public async Task PrepareForNextGameAsync()
    {
        await _joinLock.WaitAsync();
        try
        {
            if (!IsHostCaller())
            {
                await SendErrorAsync("Only the host can choose the next game.");
                return;
            }
            string hostName = _hostName;
            _nextGameHost = hostName;
            _nextGamePlayers.Clear();
            foreach (var player in _playerList.Values.Where(x =>
                x.IsAdmitted && x.GameName == _hostGame))
            {
                _nextGamePlayers.Add(player.UserID);
            }
            // Carry forward places reserved for a group member still arriving.
            _nextGamePlayers.UnionWith(_reservedPlayers);
            _reservedPlayers.Clear();
            var clients = _playerList.Values
                .Where(x => x.IsConnected && x.IsAdmitted
                    && x.UserID != hostName && x.GameName == _hostGame)
                .Select(x => x.ConnectionID)
                .ToArray();

            // Stop advertising and routing the old game before notifying clients.
            _hostGame = "";
            _maxPlayers = 0;
            _gameStarted = false;
            _startedPlayers.Clear();
            ClearHostMessageJournal();
            _sessionHostName = "";
            _sessionGame = "";
            foreach (var player in _playerList.Values)
            {
                player.IsAdmitted = false;
            }
            foreach (string connectionId in clients)
            {
                await Clients.Client(connectionId)
                    .SendAsync("WaitingForNextGame", hostName);
            }
        }
        finally
        {
            _joinLock.Release();
        }
    }

    public async Task HostingAsync(string nickName, string gameName, int maxPlayers)
    {
        await _joinLock.WaitAsync();
        try
        {
            if (string.IsNullOrWhiteSpace(nickName) || string.IsNullOrWhiteSpace(gameName))
            {
                await SendErrorAsync("A nickname and game name are required to host.");
                return;
            }
            if (maxPlayers < 1)
            {
                await SendErrorAsync("The game must specify a valid maximum player count.");
                return;
            }
            if (AllowedHosts.Count > 0 && !AllowedHosts.Any(x =>
                x.Equals(nickName, StringComparison.CurrentCultureIgnoreCase)))
            {
                await SendErrorAsync("You do not have permission to host game");
                return;
            }
            if (_hostName != "" && _hostName != nickName)
            {
                await SendErrorAsync("Another player is already hosting.");
                return;
            }

            // Existing admitted players take precedence when a host returns.
            // Otherwise, waiting clients are considered in arrival order.
            bool returningHost = _sessionHostName == nickName && _sessionGame == gameName;

            if (returningHost && _gameStarted)
            {
                if (!_startedPlayers.Contains(nickName))
                {
                    await SendErrorAsync(
                        "The returning host is not in the started game's roster.");
                    return;
                }

                if (_startedPlayers.Count > maxPlayers)
                {
                    await SendErrorAsync(
                        "This game cannot accommodate the existing players.");
                    return;
                }

                _hostName = nickName;
                _hostGame = gameName;
                _maxPlayers = maxPlayers;
                _hostRecovering = true;

                _playerList[nickName] = new ConnectionInfo
                {
                    ConnectionID = Context.ConnectionId,
                    UserID = nickName,
                    GameName = gameName,
                    IsConnected = true,
                    IsAdmitted = true
                };

                await Clients.Caller.SendAsync("ReturningHost");
                return;
            }

            var reserved = new HashSet<string>(StringComparer.Ordinal);
            if (_nextGameHost == nickName)
            {
                reserved.UnionWith(_nextGamePlayers);
            }
            else if (returningHost)
            {
                reserved.UnionWith(_reservedPlayers);
                reserved.UnionWith(_playerList.Values
                    .Where(x => x.IsAdmitted && x.GameName == gameName)
                    .Select(x => x.UserID));
            }
            reserved.Remove(nickName);
            if (reserved.Count > maxPlayers - 1)
            {
                await SendErrorAsync("This game cannot accommodate the group moving with the host.");
                return;
            }
            var waiting = _playerList.Values
                .Where(x => x.UserID != nickName && x.GameName == gameName
                    && (x.IsConnected || (returningHost && x.IsAdmitted)))
                .OrderByDescending(x => reserved.Contains(x.UserID))
                .ThenByDescending(x => x.IsAdmitted)
                .ThenBy(x => x.WaitingOrder)
                .ToArray();
            int missingReservedPlayers = reserved.Count(name =>
                !waiting.Any(x => x.UserID == name));
            int availablePlaces = maxPlayers - 1 - missingReservedPlayers;
            var accepted = waiting.Take(availablePlaces).ToArray();
            var rejected = waiting.Skip(availablePlaces).ToArray();
            if (returningHost == false)
            {
                ClearHostMessageJournal();
            }
            foreach (var player in _playerList.Values)
            {
                player.IsAdmitted = false;
            }
            _reservedPlayers.Clear();
            _reservedPlayers.UnionWith(reserved);
            if (_nextGameHost == nickName)
            {
                _nextGameHost = "";
                _nextGamePlayers.Clear();
            }
            _maxPlayers = maxPlayers;
            _hostName = nickName;
            _hostGame = gameName;
            _sessionHostName = nickName;
            _sessionGame = gameName;
            _gameStarted = false;
            _startedPlayers.Clear();
            _playerList[nickName] = new ConnectionInfo
            {
                ConnectionID = Context.ConnectionId,
                UserID = nickName,
                GameName = gameName,
                IsConnected = true,
                IsAdmitted = true
            };
            // Establish the complete admission decision before sending notifications.
            foreach (var client in accepted)
            {
                client.IsAdmitted = true;
            }
            foreach (var client in rejected)
            {
                client.IsConnected = false;
            }

            await Clients.Caller.SendAsync("Hosting");
            foreach (var client in rejected)
            {
                await Clients.Client(client.ConnectionID)
                    .SendAsync("JoinRejected", "This game is full.");
            }
            foreach (var client in accepted)
            {
                if (client.IsConnected)
                {
                    await Clients.Client(client.ConnectionID).SendAsync("HostName", _hostName);
                }
            }
            await Clients.Group(_waitingForHostedGameGroup)
                .SendAsync("HostedGameChosen", _hostGame);
            await Clients.Group(GetNextGameGroup(_hostName))
                .SendAsync("HostedGameChosen", _hostGame);
        }
        finally
        {
            _joinLock.Release();
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
        BasicList<ConnectionInfo> sendTo = _playerList.Where(x => x.Key != _hostName && x.Value.IsConnected == true && x.Value.IsAdmitted && x.Value.GameName == _hostGame && x.Key != nickNameReconnected).Select(xx => xx.Value).ToBasicList();
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
        BasicList<ConnectionInfo> sendTo = _playerList.Where(x => x.Key != _hostName && x.Value.IsConnected == true && x.Value.IsAdmitted && x.Value.GameName == _hostGame && x.Key != nickNameReconnected).Select(xx => xx.Value).ToBasicList();
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
        await _joinLock.WaitAsync();
        try
        {
            if (!IsHostCaller() || _hostGame == "")
            {
                await SendErrorAsync("Only the connected host can start the game.");
                return;
            }
            if (_gameStarted)
            {
                return;
            }
            var participants = _playerList.Values
                .Where(x => x.IsAdmitted && x.GameName == _hostGame)
                .Select(x => x.UserID)
                .ToArray();
            if (participants.Length > _maxPlayers)
            {
                await SendErrorAsync("There are too many admitted players to start this game.");
                return;
            }
            _startedPlayers.Clear();
            foreach (string player in participants)
            {
                _startedPlayers.Add(player);
            }
            // Starting is manual: freeze actual admissions, not unfilled reservations.
            _reservedPlayers.Clear();
            _gameStarted = true;
        }
        finally
        {
            _joinLock.Release();
        }
    }
    public async Task NewGameAsync()
    {
        await DisconnectEverybodyAsync("NewGame");
    }
    public async Task DisconnectEverybodyAsync()
    {
        await DisconnectEverybodyAsync("Close");
        
    }
    private async Task DisconnectEverybodyAsync(string message)
    {
        await _joinLock.WaitAsync();
        try
        {
            if (!IsHostCaller())
            {
                await SendErrorAsync("Only the host can disconnect everybody.");
                return;
            }
            var clients = _playerList.Values
                .Where(x => x.IsAdmitted && x.GameName == _hostGame && x.UserID != _hostName)
                .ToArray();
            var recipients = clients.Where(x => x.IsConnected)
                .Select(x => x.ConnectionID).ToArray();
            foreach (var client in clients)
            {
                client.IsConnected = false;
                client.IsAdmitted = false;
            }
            ClearHostMessageJournal();
            _gameStarted = false;
            _startedPlayers.Clear();
            _reservedPlayers.Clear();
            foreach (string connectionId in recipients)
            {
                await Clients.Client(connectionId).SendAsync(message);
            }
        }
        finally
        {
            _joinLock.Release();
        }
    }

    public async Task ReconnectionAsync(string nickName)
    {
        await _joinLock.WaitAsync();
        try
        {
            if (!_playerList.TryGetValue(nickName, out var connection)
                || !connection.IsAdmitted
                || connection.GameName != _hostGame
                || (_gameStarted && !_startedPlayers.Contains(nickName)))
            {
                await Clients.Caller.SendAsync("JoinRejected",
                    "You are not a participant in this game.");
                return;
            }
            connection.ConnectionID = Context.ConnectionId;
            connection.IsConnected = true;
        }
        finally
        {
            _joinLock.Release();
        }
    }

    //the purpose of game name is to make sure if a client connects to wrong game, they will get an error.
    public async Task ClientConnectingAsync(string nickName, string gameName)
    {
        await _joinLock.WaitAsync();
        try
        {
            if (string.IsNullOrWhiteSpace(nickName) || string.IsNullOrWhiteSpace(gameName))
            {
                await SendErrorAsync("A nickname and game name are required to join.");
                return;
            }
            if (nickName == _hostName)
            {
                await Clients.Caller.SendAsync("JoinRejected",
                    "That nickname is already being used by the host.");
                return;
            }

            bool currentGame = _hostName != "" && _hostGame == gameName;
            if (currentGame && _gameStarted)
            {
                if (!_startedPlayers.Contains(nickName))
                {
                    await Clients.Caller.SendAsync("JoinRejected",
                        "This game has already started. Only its existing players can reconnect.");
                    return;
                }
            }
            else if (currentGame)
            {
                // Reserve admitted players' places even while temporarily disconnected.
                // Excluding this nickname makes repeated joins/reconnections idempotent.
                int otherPlayers = _playerList.Values
                    .Where(x => x.IsAdmitted && x.GameName == gameName)
                    .Select(x => x.UserID)
                    .Concat(_reservedPlayers)
                    .Where(name => name != nickName)
                    .Distinct(StringComparer.Ordinal)
                    .Count();
                if (otherPlayers >= _maxPlayers)
                {
                    await Clients.Caller.SendAsync("JoinRejected", "This game is full.");
                    return;
                }
            }

            if (!_playerList.TryGetValue(nickName, out var connection))
            {
                connection = new ConnectionInfo { UserID = nickName };
                _playerList[nickName] = connection;
            }
            if (!connection.IsConnected || connection.ConnectionID != Context.ConnectionId
                || connection.GameName != gameName)
            {
                connection.WaitingOrder = ++_nextWaitingOrder;
            }
            connection.ConnectionID = Context.ConnectionId;
            connection.GameName = gameName;
            connection.IsConnected = true;
            connection.IsAdmitted = currentGame;

            if (!currentGame)
            {
                await Clients.Caller.SendAsync("NoHost");
            }
            else if (!_gameStarted)
            {
                await Clients.Caller.SendAsync("HostName", _hostName);
            }
            else
            {
                await Clients.Caller.SendAsync("WaitForGame");
                await Clients.Client(_playerList[_hostName].ConnectionID)
                    .SendAsync("GameState", nickName);
            }
        }
        catch (Exception ex)
        {
            await SendErrorAsync(ex.Message);
        }
        finally
        {
            _joinLock.Release();
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
        await _joinLock.WaitAsync();
        try
        {
            string routingGame =
                _hostGame != "" ? _hostGame : _sessionGame;

            ConnectionInfo? sender = FindCaller();

            if (routingGame == "" ||
                gameName != routingGame ||
                sender is null ||
                !sender.IsConnected ||
                !sender.IsAdmitted ||
                sender.GameName != routingGame)
            {
                return;
            }

            NetworkMessage message =
                await js1.DeserializeObjectAsync<NetworkMessage>(tempMessage);

            // Use the verified connection identity.
            message.YourNickName = sender.UserID;

            if (message.SpecificPlayer != "")
            {
                if (!_playerList.TryGetValue(
                        message.SpecificPlayer, out var recipient) ||
                    !recipient.IsAdmitted ||
                    recipient.GameName != routingGame)
                {
                    await SendErrorAsync(
                        $"{message.SpecificPlayer} is not a participant in this game.");
                    return;
                }

                // Messages for an absent host are retained for catch-up.
                // Other private recipients must still be connected.
                if (!recipient.IsConnected &&
                    recipient.UserID != _sessionHostName)
                {
                    await SendErrorAsync(
                        $"{message.SpecificPlayer} is not connected to this game.");
                    return;
                }

                await PrepareMessageAsync(message, sender.UserID);

                if (CanReceiveLiveMessages(recipient))
                {
                    await Clients.Client(recipient.ConnectionID)
                        .SendAsync("ReceiveMessage", message.Message);
                }

                return;
            }

            await PrepareMessageAsync(message, sender.UserID);

            var recipients = _playerList.Values
                .Where(x =>
                    CanReceiveLiveMessages(x) &&
                    x.IsAdmitted &&
                    x.GameName == routingGame &&
                    x.UserID != sender.UserID)
                .Select(x => x.ConnectionID)
                .ToArray();

            foreach (string connectionId in recipients)
            {
                await Clients.Client(connectionId)
                    .SendAsync("ReceiveMessage", message.Message);
            }
        }
        finally
        {
            _joinLock.Release();
        }
    }
    private ConnectionInfo? FindCaller()
    {
        return _playerList.Values.SingleOrDefault(x => x.ConnectionID == Context.ConnectionId);
    }

    private bool IsHostCaller()
    {
        ConnectionInfo? caller = FindCaller();
        return caller is not null && caller.IsConnected && caller.UserID == _hostName;
    }

    private static void ClearHost()
    {
        _hostName = "";
        _hostGame = "";
        //_gameStarted = false;
        _maxPlayers = 0;
        // Preserve admission while the host is temporarily away. HostingAsync
        // gives those players priority if the host returns to the same game.
        // Starting another game explicitly clears admission in PrepareForNextGameAsync.
    }

    // Caller must hold _joinLock.
    private static void ClearHostMessageJournal()
    {
        _hostMessageJournal.Clear();
        _networkSessionId = Guid.NewGuid().ToString("N");
        _hostRecovering = false;
    }

    public async Task<string> GetHostCatchUpAsync(
        string networkSessionId,
        long lastProcessedMessageSequence)
    {
        await _joinLock.WaitAsync();
        try
        {
            if (!IsHostCaller())
            {
                throw new HubException(
                    "Only the connected host can request catch-up messages.");
            }

            if (lastProcessedMessageSequence < 0)
            {
                throw new HubException(
                    "The saved message sequence cannot be negative.");
            }

            HostCatchUpResult result = new()
            {
                NetworkSessionId = _networkSessionId,
                LatestSequenceNumber = _nextMessageSequence,
                SessionMatches =
                    networkSessionId != "" &&
                    networkSessionId == _networkSessionId
            };

            if (result.SessionMatches)
            {
                if (lastProcessedMessageSequence > _nextMessageSequence)
                {
                    throw new HubException(
                        "The saved message sequence is ahead of the server.");
                }

                result.Messages = _hostMessageJournal
                    .Where(x =>
                        x.SequenceNumber > lastProcessedMessageSequence)
                    .OrderBy(x => x.SequenceNumber)
                    .ToList();
            }

            return await js1.SerializeObjectAsync(result);
        }
        finally
        {
            _joinLock.Release();
        }
    }
    // Caller must hold _joinLock.
    private static bool CanReceiveLiveMessages(ConnectionInfo recipient)
    {
        return recipient.IsConnected &&
            (!_hostRecovering ||
             recipient.UserID != _sessionHostName);
    }


    public async Task<bool> TryCompleteHostCatchUpAsync(
        string networkSessionId,
        long caughtUpThroughSequence,
        string gameState)
    {
        await _joinLock.WaitAsync();
        try
        {
            if (!IsHostCaller())
            {
                throw new HubException(
                    "Only the connected host can complete catch-up.");
            }

            if (!_hostRecovering)
            {
                throw new HubException(
                    "The host is not currently recovering.");
            }

            if (networkSessionId != _networkSessionId)
            {
                throw new HubException(
                    "The network session changed during recovery.");
            }

            if (caughtUpThroughSequence < 0 ||
                caughtUpThroughSequence > _nextMessageSequence)
            {
                throw new HubException(
                    "The catch-up message sequence is invalid.");
            }

            // More traffic arrived after the host retrieved its snapshot.
            // The host must retrieve and process another batch.
            if (caughtUpThroughSequence != _nextMessageSequence)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(gameState))
            {
                throw new HubException(
                    "The recovered game state is required.");
            }

            SentMessage restoreMessage = new()
            {
                Status = "restoregame",
                Body = gameState,
                NetworkSessionId = _networkSessionId,
                SequenceNumber = ++_nextMessageSequence
            };

            string serializedMessage =
                await js1.SerializeObjectAsync(restoreMessage);

            var recipients = _playerList.Values
                .Where(x =>
                    x.IsConnected &&
                    x.IsAdmitted &&
                    x.GameName == _sessionGame &&
                    x.UserID != _sessionHostName)
                .Select(x => x.ConnectionID)
                .ToArray();

            foreach (string connectionId in recipients)
            {
                await Clients.Client(connectionId)
                    .SendAsync("RestoreForReconnection");

                await Clients.Client(connectionId)
                    .SendAsync("ReceiveMessage", serializedMessage);
            }

            _hostRecovering = false;
            return true;
        }
        finally
        {
            _joinLock.Release();
        }
    }

}