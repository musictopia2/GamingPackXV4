namespace Mastermind.Core.Logic;
[SingletonGame]
public class MastermindMainGameClass : IAggregatorContainer, IFinishGuess
{
    private readonly ISystemError _error;
    private readonly IToast _toast;
    private readonly GlobalClass _global;
    private readonly LevelClass _level;
    public MastermindMainGameClass(IEventAggregator aggregator,
        ISystemError error,
        IToast toast,
        GlobalClass global,
        LevelClass level
        )
    {
        Aggregator = aggregator;
        _error = error;
        _toast = toast;
        _global = global;
        _level = level;
    }
    public IEventAggregator Aggregator { get; }
    public async Task NewGameAsync(GameBoardViewModel guess)
    {
        bool canRepeat = _level.LevelChosen == 2 || _level.LevelChosen == 4 || _level.LevelChosen == 6;
        int level = _level.LevelChosen;
        BasicList<Bead> possibleList = new();
        if (level == 5 || level == 6)
        {
            possibleList.Add(new Bead(EnumColorPossibilities.Aqua));
            possibleList.Add(new Bead(EnumColorPossibilities.Black));
        }
        possibleList.Add(new Bead(EnumColorPossibilities.Blue));
        possibleList.Add(new Bead(EnumColorPossibilities.Green));
        if (level > 2)
        {
            possibleList.Add(new Bead(EnumColorPossibilities.Purple));
        }
        possibleList.Add(new Bead(EnumColorPossibilities.Red));
        possibleList.Add(new Bead(EnumColorPossibilities.White));
        if (level > 2)
        {
            possibleList.Add(new Bead(EnumColorPossibilities.Yellow));
        }
        IBasicList<Bead> tempList;
        if (canRepeat == false)
        {
            tempList = possibleList.GetRandomList(false, 4);
        }
        else
        {
            int x;
            tempList = new BasicList<Bead>();
            for (x = 1; x <= 4; x++)
            {
                var ThisBead = possibleList.GetRandomItem();
                tempList.Add(ThisBead); // can have repeat
            }
        }
        _global.Solution = tempList.ToBasicList();
        await guess.NewGameAsync();
        await guess.StartNewGuessAsync();
        _global.ColorList = possibleList.Select(items => items.ColorChosen).ToBasicList();
    }
    public async Task GiveUpAsync()
    {
        _toast.ShowWarningToast("Sorry you are giving up");
        await this.SendGameOverAsync(_error);
    }
    async Task IFinishGuess.FinishGuessAsync(int howManyCorrect, GameBoardViewModel board)
    {
        bool handled = false;
        if (howManyCorrect == 4)
        {
            _toast.ShowSuccessToast("Congratuations, you won");
            handled = true;
        }
        if (board.GuessList.Last().IsCompleted)
        {
            _toast.ShowWarningToast("You ran out of guesses."); //maybe warning.  if i need something else, rethink.
            handled = true;
        }
        if (handled)
        {
            await this.SendGameOverAsync(_error);
            return;
        }
        await board.StartNewGuessAsync();
    }
}