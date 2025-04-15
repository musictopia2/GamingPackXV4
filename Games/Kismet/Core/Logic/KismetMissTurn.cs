using BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Logic;

namespace Kismet.Core.Logic;
[SingletonGame]
public class KismetMissTurn : IMissTurnClass<YahtzeePlayerItem<KismetDice>>
{
    private readonly YahtzeeVMData<KismetDice> _model;
    private readonly TestOptions _thisTest;
    private readonly IAsyncDelayer _delay;
    private readonly IScoreLogic _scoreLogic;
    private readonly ScoreContainer _scoreContainer;
    private readonly YahtzeeGameContainer<KismetDice> _gameContainer;
    public KismetMissTurn(
        YahtzeeVMData<KismetDice> model,
        TestOptions thisTest,
        IAsyncDelayer delay,
        IScoreLogic scoreLogic,
        ScoreContainer scoreContainer,
        YahtzeeGameContainer<KismetDice> gameContainer
        )
    {
        _model = model;
        _thisTest = thisTest;
        _delay = delay;
        _scoreLogic = scoreLogic;
        _scoreContainer = scoreContainer;
        _gameContainer = gameContainer;
    }
    public async Task PlayerMissTurnAsync(YahtzeePlayerItem<KismetDice> player)
    {
        _model.NormalTurn = player.NickName;
        _scoreContainer.RowList = player.RowList;
        _scoreLogic.ClearRecent();
        RowInfo thisItem = player.RowList.Where(x => x.RowSection == EnumRow.Regular
            && x.HasFilledIn() == false).Take(1).SingleOrDefault()!;
        if (thisItem != null)
        {
            _scoreLogic.MarkScore(thisItem);
            player.Points = _scoreLogic.TotalScore; //their total score can change because of getting bonus.
            if (_thisTest.NoAnimations == false)
            {
                if (_gameContainer.GetNewScoreAsync == null)
                {
                    throw new CustomBasicException("Nobody is handling the getting new scores.  Rethink");
                }
                await _gameContainer.GetNewScoreAsync();
                await _delay.DelayMilli(1200);
            }
        }
    }
}