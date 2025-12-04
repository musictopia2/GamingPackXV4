namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.Extensions;
public static class GameContainerExtensions
{
    extension(IBasicGameContainer gameContainer)
    {
        public void RepaintBoard()
        {
            gameContainer.Aggregator!.RepaintBoard();
        }
        public async Task UIEndTurnAsync()
        {
            if (gameContainer.BasicData!.MultiPlayer)
            {
                await gameContainer.Network!.SendEndTurnAsync();
            }
            if (gameContainer.EndTurnAsync == null)
            {
                throw new CustomBasicException("Nobody is handling game end turn.  Rethink");
            }
            await gameContainer.EndTurnAsync.Invoke();
        }
    }
    extension <P, S>(BasicGameContainer<P, S> gameContainer)
        where P : class, IPlayerItem, new()
        where S : BasicSavedGameClass<P>, new()
    {
        public bool CanSendMessage()
        {
            return gameContainer.SingleInfo!.CanSendMessage(gameContainer.BasicData);
        }
    }
}