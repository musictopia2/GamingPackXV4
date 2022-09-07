namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicGameClasses;
public class BeginningColorProcessorClass<E, P, S> : IBeginningColorProcesses<E>, IChoosePieceNM
    where E : struct, IFastEnumColorList<E>
    where P : class, IPlayerBoardGame<E>, new()
    where S : BasicSavedGameClass<P>, new()
{
    private readonly BeginningColorModel<E, P> _model;
    private readonly BasicGameContainer<P, S> _gameContainer;
    public BeginningColorProcessorClass(BeginningColorModel<E, P> model,
        BasicGameContainer<P, S> gameContainer
        )
    {
        _model = model;
        _gameContainer = gameContainer;
        MiscDelegates.ComputerChooseColorsAsync = ComputerChooseColorAsync;
        MiscDelegates.ContinueColorsAsync = ContinueColorsAsync;
        MiscDelegates.FillRestColors = () =>
        {
            _model.ColorChooser.FillInRestOfColors();
        };
        MiscDelegates.ManuelSetColors = () =>
        {
            _model.ColorChooser.IsEnabled = true;
        };
    }
    protected virtual void RecordColor() { }
    public Action<string>? SetTurn { get; set; }
    public Action<string>? SetInstructions { get; set; }
    public async Task ChoseColorAsync(E colorChosen)
    {
        if (_gameContainer.SaveRoot == null)
        {
            throw new CustomBasicException("Save root not set.  Rethink");
        }
        if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData))
        {
            await _gameContainer.Network!.SendAllAsync("chosepiece", colorChosen);
        }
        var tempList = _model.ColorChooser.ItemList.Select(x => x.EnumValue).ToBasicList();
        tempList.RemoveSpecificItem(colorChosen);
        _model.ColorChooser.ChooseItem(colorChosen);
        _gameContainer.Command.UpdateAll();
        if (_gameContainer.Test.NoAnimations == false)
        {
            await _gameContainer.Delay.DelaySeconds(.5);
        }
        _gameContainer.SingleInfo!.Color = colorChosen;
        RecordColor();
        bool finished = _gameContainer.SaveRoot.PlayerList.DidChooseColors();
        if (tempList.Count == 1 && finished == false)
        {
            _gameContainer.SaveRoot.PlayOrder.WhoTurn = await _gameContainer.SaveRoot.PlayerList.CalculateWhoTurnAsync();
            _gameContainer.SingleInfo = _gameContainer.SaveRoot.PlayerList.GetWhoPlayer();
            _gameContainer.SingleInfo.Color = tempList.Single();
            finished = true;
        }
        if (finished == false)
        {
            if (_gameContainer.EndTurnAsync == null)
            {
                throw new CustomBasicException("Nobody is handling end turn.  Rethink");
            }
            await _gameContainer.EndTurnAsync.Invoke();
            return;
        }
        if (MiscDelegates.ColorsFinishedAsync == null)
        {
            throw new CustomBasicException("Nobody is handling the finishing of the colors.  Rethink");
        }
        await MiscDelegates.ColorsFinishedAsync.Invoke();
    }
    private async Task ContinueColorsAsync()
    {
        _model.ColorChooser.PlayerList = _gameContainer.PlayerList;
        _model.ColorChooser.LoadColors();
        if (SetInstructions == null)
        {
            throw new CustomBasicException("Did not set instructions.  Rethink");
        }
        if (SetTurn == null)
        {
            throw new CustomBasicException("Did not set turn.  Rethink");
        }
        if (_gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            SetInstructions.Invoke("Choose A Color");
        }
        else
        {
            SetInstructions.Invoke($"Waiting for {_gameContainer.SingleInfo.NickName} to choose color");
        }
        SetTurn(_gameContainer.SingleInfo.NickName);
        await Task.CompletedTask;
    }
    public Task InitAsync()
    {
        if (_gameContainer.SingleInfo == null)
        {
            return Task.CompletedTask;
        }
        return ContinueColorsAsync();
    }
    private async Task ComputerChooseColorAsync()
    {
        E thisColor = _model.ColorChooser!.ItemToChoose();
        await ChoseColorAsync(thisColor);
    }
    async Task IChoosePieceNM.ChoosePieceReceivedAsync(string data)
    {
        E thisColor = await js.DeserializeObjectAsync<E>(data);
        await ChoseColorAsync(thisColor);
    }
}