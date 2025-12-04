namespace BasicGameFrameworkLibrary.Core.NetworkingClasses.Extensions;
public static class CommonMessages
{
    extension (IGameNetwork network)
    {
        public async Task SendReadyMessageAsync(string yourName, string hostName)
        {
            await network.SendToParticularPlayerAsync("ready", yourName, hostName);
        }
        public async Task SendLoadGameMessageAsync<T>(T payLoad)
        {
            await network.SendAllAsync("loadgame", payLoad);
        }
        public async Task SendNewGameAsync<T>(T payLoad)
        {
            await network.SendAllAsync("newgame", payLoad);
        }
        public async Task SendRestoreGameAsync<T>(T payLoad)
        {
            await network.SendAllAsync("restoregame", payLoad);
        }
        public async Task SendMoveAsync<T>(T payLoad)
        {
            await network.SendAllAsync("move", payLoad);
        }
        public async Task SendMoveAsync(int move)
        {
            await network.SendAllAsync("move", move);
        }
        public async Task SendDiscardMessageAsync(int deck)
        {
            await network.SendAllAsync("discard", deck);
        }
        public async Task SendEndTurnAsync()
        {
            await network.SendAllAsync("endturn");
        }
        public async Task SendDrawAsync()
        {
            await network.SendAllAsync("drawcard");
        }
        public async Task SendPlayDominoAsync(int deck)
        {
            await network.SendAllAsync("playdomino", deck);
        }
        public async Task SendCustomDeckListAsync<D>(string status, DeckRegularDict<D> list)
            where D : class, IDeckObject
        {
            var output = list.GetDeckListFromObjectList();
            await network.SendAllAsync(status, output);
        }
    }
    
}