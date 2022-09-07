namespace BasicGameFrameworkLibrary.Core.BasicEventModels;
public static class EventExtensions
{
    public static async Task SendGameOverAsync(this IAggregatorContainer aggregator, ISystemError error)
    {
        try
        {
            await aggregator.Aggregator.PublishAsync(new GameOverEventModel());
        }
        catch (Exception ex)
        {
            error.ShowSystemError(ex.Message);
        }
    }
    public static bool AnimationCompleted { get; set; }
    public static async Task AnimateMovePiecesAsync<S>(this IEventAggregator thisE, Vector previousSpace,
        Vector moveToSpace, S temporaryObject, bool useColumn = false) where S : class
    {
        AnimatePieceEventModel<S> thisA = new();
        thisA.MoveToSpace = moveToSpace;
        thisA.PreviousSpace = previousSpace;
        thisA.TemporaryObject = temporaryObject;
        thisA.UseColumn = useColumn;
        await thisE.PublishAsync(thisA);
    }
    public static void RepaintBoard(this IEventAggregator thisE)
    {
        thisE.RepaintMessage(EnumRepaintCategories.Main);
    }
    public static void RepaintMessage(this IEventAggregator thisE, EnumRepaintCategories thisCategory)
    {
        thisE.PublishAll(new RepaintEventModel(), thisCategory.ToString());
    }
    #region Animation Objects Helpers
    public static async Task AnimatePlayAsync<D>(this IEventAggregator thisE,
        D thisCard, Action? finalAction = null) where D : class, IDeckObject, new()
    {
        await thisE.AnimateCardAsync(thisCard, EnumAnimcationDirection.StartUpToCard, "maindiscard", finalAction: finalAction!);
    }
    public static async Task AnimatePlayAsync<D>(this IEventAggregator thisE, D thisCard,
        EnumAnimcationDirection direction, Action? finalAction = null) where D : class, IDeckObject, new()
    {
        await thisE.AnimateCardAsync(thisCard, direction, "maindiscard", finalAction: finalAction!);
    }
    public static async Task AnimateDrawAsync<D>(this IEventAggregator thisE, D thisCard) where D : class, IDeckObject, new()
    {
        await thisE.AnimateDrawAsync(thisCard, EnumAnimcationDirection.StartCardToDown);
    }
    public static async Task AnimateDrawAsync<D>(this IEventAggregator thisE, D thisCard
        , EnumAnimcationDirection direction) where D : class, IDeckObject, new()
    {
        await thisE.AnimateCardAsync(thisCard, direction, "maindeck");
    }
    public static async Task AnimatePickUpDiscardAsync<D>(this IEventAggregator thisE, D thisCard) where D : class, IDeckObject, new()
    {
        await thisE.AnimatePickUpDiscardAsync(thisCard, EnumAnimcationDirection.StartCardToDown);
    }
    public static async Task AnimatePickUpDiscardAsync<D>(this IEventAggregator thisE, D thisCard
        , EnumAnimcationDirection direction) where D : class, IDeckObject, new()
    {
        await thisE.AnimateCardAsync(thisCard, direction, "maindiscard");
    }
    private static async Task CompleteAnimationAsync()
    {
        do
        {
            await Task.Delay(1);
            if (AnimationCompleted == true)
            {
                return;
            }
        } while (true);
    }
    private static void ResetDiscard(this IEventAggregator thisE)
    {
        thisE.PublishAll(new ResetCardsEventModel());
    }
    public static async Task AnimateCardAsync<D>(this IEventAggregator thisE,
        D thisCard, EnumAnimcationDirection direction, string tag
        , BasicPileInfo<D>? pile1 = null, Action? finalAction = null) where D : class, IDeckObject, new()
    {
        AnimateCardInPileEventModel<D> thisA = new();
        thisA.Direction = direction;
        thisA.ThisCard = thisCard;
        thisA.ThisPile = pile1;
        AnimationCompleted = false;
        if (thisE.HandlerAsyncExistsFor<AnimateCardInPileEventModel<D>>(tag))
        {
            await thisE.PublishAsync(thisA, tag);
            await CompleteAnimationAsync();
        }
        if (finalAction != null)
        {
            finalAction.Invoke();
        }
        thisE.ResetDiscard();
    }
    #endregion
}