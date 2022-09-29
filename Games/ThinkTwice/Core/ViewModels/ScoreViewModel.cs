namespace ThinkTwice.Core.ViewModels;
[InstanceGame]
public partial class ScoreViewModel : ScreenViewModel, IBlankGameVM
{
    public ThinkTwiceVMData VMData { get; set; }
    private readonly ThinkTwiceGameContainer _gameContainer;
    private readonly IMessageBox _message;

    //public int ItemSelected { get; set; }
    public bool IsEnabled { get; set; }
    private string GetDescriptionText()
    {
        if (VMData.ItemSelected == 5)
        {
            return "2 of a kind:  10 points" + Constants.VBCrLf + "3 of a kind:  20 points" + Constants.VBCrLf + "4 of a kind:  30 points" + Constants.VBCrLf + "5 of a kind:  50 points" + Constants.VBCrLf + "6 of a kind:  100 points";
        }
        return "Sum of all the dice";
    }
    public ScoreViewModel(ThinkTwiceVMData model, 
        ThinkTwiceGameContainer gameContainer,
        IEventAggregator aggregator,
        IMessageBox message
        ) : base(aggregator)
    {
        VMData = model;
        _gameContainer = gameContainer;
        _message = message;
        _gameContainer.Command.ExecutingChanged = Command_ExecutingChanged;
        CommandContainer = _gameContainer.Command;
        VMData.ItemSelected = VMData.ItemSelected;
        CreateCommands(CommandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    private bool CanClickCommands(bool isMain)
    {
        if (_gameContainer.SaveRoot.CategoryRolled == -1)
        {
            return false;
        }
        if (isMain == false)
        {
            return true;
        }
        return VMData.ItemSelected > -1;
    }
    private void Command_ExecutingChanged()
    {
        IsEnabled = !_gameContainer.Command.IsExecuting;
    }
    #region Command Functions
    public bool CanScoreDescription => CanClickCommands(true);
    [Command(EnumCommandCategory.Plain)]
    public async Task ScoreDescriptionAsync()
    {
        if (VMData.ItemSelected == -1)
        {
            throw new CustomBasicException("Nothing Selected");
        }
        var text = GetDescriptionText();
        await _message.ShowMessageAsync(text);
    }
    public bool CanChangeSelection => CanClickCommands(false);
    [Command(EnumCommandCategory.Plain)]
    public void ChangeSelection(string text)
    {
        VMData.ItemSelected = VMData.TextList.IndexOf(text);
    }
    public bool CanCalculateScore => CanClickCommands(true);
    public CommandContainer CommandContainer { get; set; }
    [Command(EnumCommandCategory.Plain)]
    public async Task CalculateScoreAsync()
    {
        if (_gameContainer.ScoreClickAsync == null)
        {
            throw new CustomBasicException("Nobody is handling the score click.  Rethink");
        }
        await _gameContainer.ScoreClickAsync.Invoke();
    }
    #endregion
}