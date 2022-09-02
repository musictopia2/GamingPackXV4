namespace RageCardGame.Core.Logic;
[SingletonGame]
public class SpecificTrickAreaObservable : SeveralPlayersTrickObservable<EnumColor, RageCardGameCardInformation, RageCardGamePlayerItem, RageCardGameSaveInfo>
{
    private readonly RageCardGameGameContainer _gameContainer;
    private readonly RageDelgates _delgates;
    public SpecificTrickAreaObservable(RageCardGameGameContainer gameContainer,
        RageDelgates delgates
        ) : base(gameContainer)
    {
        _gameContainer = gameContainer;
        _delgates = delgates;
    }
    protected override void PopulateNewCard(RageCardGameCardInformation oldCard, ref RageCardGameCardInformation newCard)
    {
        newCard.Color = oldCard.Color;
        newCard.Value = oldCard.Value;
    }
    public override void ClearBoard()
    {
        base.ClearBoard();
        ShowLeadColor();
    }
    private void ShowLeadColor()
    {
        if (_gameContainer.ShowLeadColor == null)
        {
            throw new CustomBasicException("Nobody is showing lead color.  Rethink");
        }
        _gameContainer.ShowLeadColor.Invoke();
    }
    public override void LoadGame()
    {
        base.LoadGame();
        ShowLeadColor();
    }
    protected override async Task AfterPlayCardAsync(RageCardGameCardInformation thisCard)
    {
        if (thisCard.SpecialType != EnumSpecialType.None)
        {
            if (_gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self && _gameContainer.Test!.NoAnimations == false)
            {
                await _gameContainer.Delay!.DelaySeconds(.75);
            }
        }
        if (thisCard.SpecialType == EnumSpecialType.Wild || thisCard.SpecialType == EnumSpecialType.Change)
        {
            if (_gameContainer.ChooseColorAsync == null)
            {
                throw new CustomBasicException("Nobody is choosing colors.  Rethink");
            }
            await _gameContainer.ChooseColorAsync.Invoke();
            return;
        }
        if (thisCard.SpecialType == EnumSpecialType.Out)
        {
            _gameContainer.SaveRoot!.TrumpSuit = EnumColor.None;
        }
        ShowLeadColor();
        await base.AfterPlayCardAsync(thisCard);
    }
    public async Task ColorChosenAsync(EnumColor thisColor)
    {
        var thisCard = OrderList.Last();
        if (thisCard.SpecialType != EnumSpecialType.Wild && thisCard.SpecialType != EnumSpecialType.Change)
        {
            throw new CustomBasicException("Only change or wilds can change colors");
        }
        if (thisColor == EnumColor.None)
        {
            throw new CustomBasicException("Color chosen can't be none");
        }
        if (_delgates.CloseColorScreenAsync == null)
        {
            throw new CustomBasicException("Nobody is closing the color.  Rethink");
        }
        await _delgates.CloseColorScreenAsync.Invoke();
        _gameContainer.SaveRoot!.Status = EnumStatus.Regular;
        Visible = true; //now set this to visible.
        if (_gameContainer.Test!.NoAnimations == false)
        {
            await _gameContainer.Delay!.DelaySeconds(.25);
        }
        if (thisCard.SpecialType == EnumSpecialType.Change)
        {
            if (thisColor == _gameContainer.SaveRoot.TrumpSuit)
            {
                throw new CustomBasicException("Must choose a different suit for trump");
            }
            _gameContainer.SaveRoot.TrumpSuit = thisColor;
            await base.AfterPlayCardAsync(thisCard);
            return;
        }
        thisCard.Color = thisColor;
        thisCard.Value = 16; //show the highest of the suit.
        await base.AfterPlayCardAsync(thisCard);
    }
}