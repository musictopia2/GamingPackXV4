namespace Mancala.Core.ViewModels;
[SingletonGame]
[AutoReset]
public class GameBoardVM
{
    public GameBoardVM(MancalaMainGameClass mainGame,
        CommandContainer command,
        BasicData basicData,
        GameBoardProcesses gameBoard1,
        MancalaVMData vMData
        )
    {
        _mainGame = mainGame;
        _command = command;
        _basicData = basicData;
        GameBoard1 = gameBoard1;
        _network = _basicData.GetNetwork();
        GameData = vMData;
    }
    private readonly IGameNetwork? _network;
    private readonly MancalaMainGameClass _mainGame;
    private readonly CommandContainer _command;
    private readonly BasicData _basicData;
    public GameBoardProcesses GameBoard1 { get; }
    public BasicList<SpaceInfo> SpaceList => GameData.SpaceList.Values.ToBasicList();
    public MancalaVMData GameData { get; }
    public int GetIndex(SpaceInfo space) => GameData.SpaceList.GetKey(space);
    public async Task MakeMoveAsync(SpaceInfo space)
    {
        int index = GameData.SpaceList.GetKey(space);
        await MakeMoveAsync(index);
    }
    private async Task MakeMoveAsync(int space)
    {
        if (_command.IsExecuting)
        {
            return; //has to do manually this time because of how it works.
        }
        if (_mainGame!.SingleInfo!.ObjectList.Any(x => x.Index == space) == false)
        {
            return;
        }
        _command.IsExecuting = true;

        _mainGame!.OpenMove();
        if (_basicData!.MultiPlayer == true)
        {
            await _network!.SendMoveAsync(space + 7); //because reversed.
        }
        await GameBoard1!.AnimateMoveAsync(space);
    }
}