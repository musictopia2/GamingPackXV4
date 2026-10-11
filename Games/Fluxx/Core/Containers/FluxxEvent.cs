namespace Fluxx.Core.Containers;
[SingletonGame]
[AutoReset]
public class FluxxEvent(FluxxGameContainer gameContainer,
    FluxxDelegates delegates,
    ActionContainer actionContainer,
    IDrawUseProcesses drawUseProcesses,
    IPlayProcesses playProcesses,
    IEverybodyOneProcesses everybodyOneProcesses,
    IRotateTradeHandProcesses rotateTradeHandProcesses,
    IFinalKeeperProcesses finalKeeperProcesses,
    IFinalRuleProcesses finalRuleProcesses
        ) : IFluxxEvent
{
    private readonly FluxxGameContainer _gameContainer = gameContainer;
    private readonly FluxxDelegates _delegates = delegates;
    private readonly ActionContainer _actionContainer = actionContainer;
    private readonly IDrawUseProcesses _drawUseProcesses = drawUseProcesses;
    private readonly IPlayProcesses _playProcesses = playProcesses;
    private readonly IEverybodyOneProcesses _everybodyOneProcesses = everybodyOneProcesses;
    private readonly IRotateTradeHandProcesses _rotateTradeHandProcesses = rotateTradeHandProcesses;
    private readonly IFinalKeeperProcesses _finalKeeperProcesses = finalKeeperProcesses;
    private readonly IFinalRuleProcesses _finalRuleProcesses = finalRuleProcesses;

    async Task IFluxxEvent.CardChosenToPlayAtAsync(int deck, int selectedIndex)
    {
        int index = _actionContainer!.GetPlayerIndex(selectedIndex);
        await _playProcesses.PlayUseTakeAsync(deck, index);
    }
    async Task IFluxxEvent.CardToUseAsync(int deck)
    {
        await _drawUseProcesses.DrawUsedAsync(deck);
    }
    async Task IFluxxEvent.ChoseForEverybodyGetsOneAsync(BasicList<int> selectedList, int selectedIndex)
    {
        await _everybodyOneProcesses.EverybodyGetsOneAsync(selectedList, selectedIndex);
    }
    async Task IFluxxEvent.ChosePlayerForCardChosenAsync(int selectedIndex)
    {
        await _actionContainer!.ChosePlayerOnActionAsync(selectedIndex);
        await _gameContainer.ContinueTurnAsync!.Invoke();
    }
    async Task IFluxxEvent.CloseKeeperScreenAsync()
    {

        if (_gameContainer.CurrentAction == null)
        {
            if (_delegates.LoadMainScreenAsync == null)
            {
                throw new CustomBasicException("Nobody is loading the main screen when closing out the keeper.  Rethink");
            }
            await _delegates.LoadMainScreenAsync();
            return;
        }
        if (_delegates.LoadActionScreenAsync == null)
        {
            throw new CustomBasicException("Nobody is loading the action screen when closing out the keeper.  Rethink");
        }
        await _delegates.LoadActionScreenAsync();

    }
    async Task IFluxxEvent.DirectionChosenAsync(int selectedIndex)
    {
        await _rotateTradeHandProcesses.RotateHandAsync((EnumDirection)selectedIndex);
    }
    async Task IFluxxEvent.DoAgainSelectedAsync(int selectedIndex)
    {
        var thisCard = _actionContainer!.GetCardToDoAgain(selectedIndex);
        await _actionContainer!.DoAgainProcessPart1Async(selectedIndex);
        await _playProcesses.PlayCardAsync(thisCard);
    }
    async Task IFluxxEvent.FirstCardRandomChosenAsync(int deck)
    {
        await _playProcesses.PlayRandomCardAsync(deck);
    }
    async Task IFluxxEvent.KeepersExchangedAsync(KeeperPlayer keeperFrom, KeeperPlayer keeperTo)
    {
        await _finalKeeperProcesses.ProcessExchangeKeepersAsync(keeperFrom, keeperTo);
    }
    async Task IFluxxEvent.RulesSimplifiedAsync(BasicList<int> simpleList)
    {
        await _finalRuleProcesses.SimplifyRulesAsync(simpleList);
    }
    async Task IFluxxEvent.RuleTrashedAsync(int selectedIndex)
    {
        await _finalRuleProcesses.TrashNewRuleAsync(selectedIndex);
    }
    async Task IFluxxEvent.StealTrashKeeperAsync(KeeperPlayer currentKeeper, bool isTrashed)
    {
        await _finalKeeperProcesses.ProcessTrashStealKeeperAsync(currentKeeper, isTrashed);
    }
    async Task IFluxxEvent.TradeHandsAsync(int selectedIndex)
    {
        await _rotateTradeHandProcesses.TradeHandAsync(selectedIndex);
    }
}
