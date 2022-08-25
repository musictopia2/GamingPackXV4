namespace Mastermind.Core.ViewModels;
/// <summary>
/// this is the hint view model.  however, had to make it a gameboard so it fits the new pattern.
/// </summary>
[InstanceGame]
public partial class GameBoardViewModel : ScreenViewModel, IBlankGameVM
{
    private readonly GlobalClass _global;
    private readonly IFinishGuess _mainGame;
    public BasicList<Guess> GuessList = new();
    internal Task NewGameAsync()
    {
        var tempList = new BasicList<Guess>();
        int guesses = _global.HowManyGuess;
        guesses.Times(x =>
        {
            Guess thisGuess = new();
            if (x == 1)
                thisGuess.GetNewBeads();
            tempList.Add(thisGuess);
        });
        GuessList.ReplaceRange(tempList);
        return Task.CompletedTask; //to keep compatibility.  if i need to scroll to guess, then rethink.
        //return _aggregator.ScrollToGuessAsync(GuessList.First());
    }
    public static bool CanChangeMind(Bead bead)
    {
        if (bead.CurrentGuess!.IsCompleted)
        {
            return false;
        }
        if (bead.CurrentGuess.YourBeads.Count == 0)
        {
            throw new CustomBasicException("Not Sure");
        }
        return true;
    }
    [Command(EnumCommandCategory.Plain)]
    public static void ChangeMind(Bead bead)
    {
        bead.ColorChosen = EnumColorPossibilities.None;
    }
    public GameBoardViewModel(GlobalClass global, IFinishGuess mainGame, CommandContainer commandContainer, IEventAggregator aggregator) : base(aggregator)
    {
        _global = global;
        _mainGame = mainGame;
        CommandContainer = commandContainer;
        CreateCommands(CommandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    private Guess GetCurrentGuess()
    {
        if (GuessList.Count == 0)
        {
            throw new CustomBasicException("No guess even found.  Rethink");
        }
        return GuessList.Single(items => items.YourBeads.Count == 4 && items.IsCompleted == false);
    }
    internal void SelectedColorForCurrentGuess(EnumColorPossibilities thisColor)
    {
        var thisGuess = GetCurrentGuess();
        foreach (var thisBead in thisGuess.YourBeads)
        {
            if (thisBead.ColorChosen == EnumColorPossibilities.None)
            {
                thisBead.ColorChosen = thisColor;
                return;
            }
        }
    }
    internal bool HasMoreGuessesLeft => !GuessList.Last().IsCompleted;
    public CommandContainer CommandContainer { get; set; }
    internal Task StartNewGuessAsync()
    {
        var nextGuess = GuessList.Where(items => items.IsCompleted == false).Take(1).Single();
        nextGuess.GetNewBeads();
        return Task.CompletedTask; //hopefully no problem (?)
        //await _aggregator.ScrollToGuessAsync(nextGuess);
    }
    internal bool DidFillInGuess()
    {
        if (GuessList.Count == 0)
        {
            return false;
        }
        var thisGuess = GetCurrentGuess();
        return !thisGuess.YourBeads.Any(items => items.ColorChosen == EnumColorPossibilities.None);
    }
    internal async Task SubmitGuessAsync()
    {
        var thisGuess = GetCurrentGuess();
        if (thisGuess.YourBeads.Any(items => items.ColorChosen == EnumColorPossibilities.None))
        {
            return;
        }
        if (thisGuess.YourBeads.Count != 4)
        {
            throw new CustomBasicException("You must have all 4 beads in order to submit the guess.");
        }
        thisGuess.IsCompleted = true;
        var (howManyCorrect, howManyOutOfOrder) = GetHintData(thisGuess);
        thisGuess.HowManyBlacks = howManyCorrect;
        thisGuess.HowManyAquas = howManyOutOfOrder;
        await _mainGame.FinishGuessAsync(thisGuess.HowManyBlacks, this);
    }
    private (int howManyCorrect, int howManyOutOfOrder) GetHintData(Guess thisGuess)
    {
        _global!.StartCheckingSolution();
        if (_global.Solution!.Count != 4 || thisGuess.YourBeads.Count != 4)
        {
            throw new CustomBasicException("The solution must have 4 items and your guess must also have 4 items");
        }
        int completeCorrect = 0;
        int semiCorrect = 0;
        for (int x = 0; x < 4; x++)
        {
            if (thisGuess.YourBeads[x].ColorChosen == _global.Solution[x].ColorChosen)
            {
                completeCorrect++;
                BeadChecked(thisGuess.YourBeads[x], _global.Solution[x]);
            }
        }
        var tempList = thisGuess.YourBeads.Where(items => items.DidCheck == false).ToBasicList();
        tempList.ForEach(yourBead =>
        {
            var correctBead = _global.Solution.Where(items => items.ColorChosen == yourBead.ColorChosen && items.DidCheck == false).Take(1).SingleOrDefault();
            if (correctBead != null)
            {
                semiCorrect++;
                correctBead.DidCheck = true;
            }
        });
        return (completeCorrect, semiCorrect);
    }
    private static void BeadChecked(Bead YourBead, Bead CorrectBead)
    {
        YourBead.DidCheck = true;
        CorrectBead.DidCheck = true;
    }
}