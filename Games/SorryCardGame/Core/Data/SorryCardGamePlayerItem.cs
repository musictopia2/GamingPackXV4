namespace SorryCardGame.Core.Data;
public partial class SorryCardGamePlayerItem : PlayerSingleHand<SorryCardGameCardInformation>, IPlayerBoardGame<EnumColorChoices>
{
    public EnumColorChoices Color { get; set; } = EnumColorChoices.None;
    public bool DidChooseColor => Color != EnumColorChoices.None;
    public void Clear()
    {
        Color = EnumColorChoices.None;
    }
    public bool OtherTurn { get; set; }
    private int _howManyAtHome;
    public int HowManyAtHome
    {
        get { return _howManyAtHome; }
        set
        {
            if (SetProperty(ref _howManyAtHome, value))
            {
                if (value > 4)
                {
                    throw new CustomBasicException("There can't ever be more than 4 at home.");
                }
                if (value < 0)
                {
                    throw new CustomBasicException("There can't ever be less than 0 at home.");
                }
            }
        }
    }
    SorryCardGameMainGameClass? _mainGame;
    [Command(EnumCommandCategory.Plain)]
    public async Task SorryPlayerAsync()
    {
        if (_mainGame!.BasicData!.MultiPlayer == true)
        {
            await _mainGame.Network!.SendAllAsync("sorryplayer", Id);
        }
        await _mainGame.SorryPlayerAsync(Id);
    }
    public bool CanSorryPlayer()
    {
        if (PlayerCategory == EnumPlayerCategory.Self)
        {
            return false; //you can't even click on your own pile.
        }
        if (_mainGame!.SaveRoot!.GameStatus != EnumGameStatus.ChoosePlayerToSorry)
        {
            return false;
        }
        return HowManyAtHome > 0;
    }
    public void Load(SorryCardGameMainGameClass mainGame, CommandContainer command)
    {
        _mainGame = mainGame;
        CreateCommands(command);
    }
    partial void CreateCommands(CommandContainer command);
}