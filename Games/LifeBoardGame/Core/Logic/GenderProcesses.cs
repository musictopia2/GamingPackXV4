namespace LifeBoardGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class GenderProcesses : IGenderProcesses
{
    private readonly LifeBoardGameVMData _model;
    private readonly LifeBoardGameGameContainer _gameContainer;
    public Action<string>? SetTurn { get; set; }
    public Action<string>? SetInstructions { get; set; }
    public GenderProcesses(LifeBoardGameVMData model, LifeBoardGameGameContainer gameContainer)
    {
        _model = model;
        _gameContainer = gameContainer;
        _gameContainer.ComputerChooseGenderAsync = ComputerChooseGenderAsync;
        _gameContainer.ShowGenderAsync = ShowGenderAsync;
    }
    private async Task ShowGenderAsync()
    {
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
            SetInstructions.Invoke("Choose Gender");
        }
        else
        {
            SetInstructions.Invoke($"Waiting for {_gameContainer.SingleInfo.NickName} to choose gender");
        }
        SetTurn(_gameContainer.SingleInfo.NickName);
        _model.GenderChooser.LoadEntireList(); //show here would be best now.
        _gameContainer.Aggregator.Publish(new NewTurnEventModel());
        await Task.Delay(300);
    }
    private async Task ComputerChooseGenderAsync()
    {
        if (_gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Computer)
        {
            throw new CustomBasicException("You are not computer.  Rethink");
        }
        await Task.Delay(200);
        EnumGender gender = _model.GenderChooser!.ItemToChoose();
        await ChoseGenderAsync(gender);
    }
    public async Task ChoseGenderAsync(EnumGender gender)
    {
        if (_gameContainer.SaveRoot == null)
        {
            throw new CustomBasicException("Save root not set.  Rethink");
        }
        if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData))
        {
            await _gameContainer.Network!.SendAllAsync("gender", gender);
        }
        _model.GenderChooser.ChooseItem(gender);
        _gameContainer.Command.ReportAll();
        if (_gameContainer.Test!.NoAnimations == false)
        {
            await _gameContainer.Delay!.DelayMilli(500);
        }
        _gameContainer.SingleInfo!.Gender = gender;
        if (_gameContainer.PlayerList!.Any(x => x.Gender == EnumGender.None) == false)
        {
            await _gameContainer.Aggregator.PublishAsync(new StartEventModel());
            return;
        }
        if (_gameContainer.EndTurnAsync == null)
        {
            throw new CustomBasicException("Nobody is handling end turn.  Rethink");
        }
        await _gameContainer.EndTurnAsync.Invoke();
    }
}
