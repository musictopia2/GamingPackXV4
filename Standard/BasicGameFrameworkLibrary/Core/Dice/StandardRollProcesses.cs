namespace BasicGameFrameworkLibrary.Core.Dice;

/// <summary>
/// this is intended for standard dice.  try to make it work with colored dice.
/// however, in order to make this one easier, its used when the tag is rolled.
/// this will implement the interface for rolled for the message.
/// maybe use generics.  however, it has to be related to standard dice (even risk dice should qualify).
/// was going to be simpledice but instead instead be IStandardDice.
/// could not do for risk because i need to roll 2 different kinds.  not worth breaking all of this just to accomodate risk.
/// </summary>
public class StandardRollProcesses<D, P> : IRolledNM, ISelectDiceNM, IStandardRollProcesses
    where D : IStandardDice, new()
    where P : class, IPlayerItem, new()
{
    public ICup<D> CupModel;
    private readonly BasicData _basicData;
    private readonly IGameNetwork? _network;
    public StandardRollProcesses(ICup<D> cup, IGamePackageResolver resolver, BasicData basicData)
    {
        CupModel = cup;
        _basicData = basicData;
        if (basicData.MultiPlayer)
        {
            _network = resolver.Resolve<IGameNetwork>();
        }
    }
    public int HowManySections { get; set; } = 6;
    public async Task RollReceivedAsync(string data)
    {
        BasicList<BasicList<D>> thisList = await CupModel.Cup!.GetDiceList(data);
        await RollDiceAsync(thisList);
    }
    public async Task SelectUnSelectDiceAsync(int id)
    {
        if (CurrentPlayer!.Invoke().CanSendMessage(_basicData) == true)
        {
            await _network!.SendAllAsync("selectdice", id);
        }
        CupModel.Cup!.SelectUnselectDice(id);
        if (AfterSelectUnselectDiceAsync == null)
        {
            throw new CustomBasicException("The selectunselect dice was never used.  Rethink");
        }
        await AfterSelectUnselectDiceAsync.Invoke();
    }
    public async Task SelectDiceReceivedAsync(int id)
    {
        await SelectUnSelectDiceAsync(id);
    }
    public async Task RollDiceAsync()
    {
        if (CupModel.Cup!.HowManyDice == 0)
        {
            throw new CustomBasicException("Can't have 0 dice.  This means forgot to set the number of dice in the view models");
        }
        if (CanRollAsync != null && BeforeRollingAsync != null)
        {
            if (await CanRollAsync() == false)
            {
                return;
            }
            await BeforeRollingAsync.Invoke();
        }
        var list = CupModel.Cup.RollDice(HowManySections);
        if (CurrentPlayer == null)
        {
            throw new CustomBasicException("No player action.  Rethink");
        }
        if (CurrentPlayer.Invoke().CanSendMessage(_basicData))
        {
            await CupModel.Cup.SendMessageAsync(list);
        }
        await RollDiceAsync(list);
    }
    public Func<Task>? AfterSelectUnselectDiceAsync { get; set; }
    public Func<Task<bool>>? CanRollAsync { get; set; }
    public Func<Task>? BeforeRollingAsync { get; set; }
    public Func<Task>? AfterRollingAsync { get; set; }
    public Func<P>? CurrentPlayer { get; set; }
    private async Task RollDiceAsync(BasicList<BasicList<D>> list)
    {
        await CupModel.Cup!.ShowRollingAsync(list);
        if (AfterRollingAsync == null)
        {
            throw new CustomBasicException("Must have code for after rolling.  Rethink.");
        }
        await AfterRollingAsync.Invoke();
    }
}