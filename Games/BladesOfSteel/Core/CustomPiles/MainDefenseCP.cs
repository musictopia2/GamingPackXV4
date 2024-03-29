﻿namespace BladesOfSteel.Core.CustomPiles;
public class MainDefenseCP : HandObservable<RegularSimpleCard>
{
    private readonly BladesOfSteelGameContainer _gameContainer;
    protected override Task PrivateBoardSingleClickedAsync()
    {
        if (HandList.Count > 0)
        {
            return Task.CompletedTask;
        }
        return base.PrivateBoardSingleClickedAsync();
    }
    protected override void AfterPopulateObjects()
    {
        HandList.ForEach(thisCard =>
        {
            thisCard.Visible = true;
            thisCard.Drew = false;
            thisCard.IsSelected = false;
            thisCard.IsUnknown = false;
        });
    }
    public MainDefenseCP(BladesOfSteelGameContainer gameContainer) : base(gameContainer.Command)
    {
        Maximum = 3;
        Text = "Defense Pile Played";
        AutoSelect = EnumHandAutoType.None;
        _gameContainer = gameContainer;
    }
    public bool HasCards => HandList.Count > 0;
    public bool CanAddDefenseCards(IBasicList<RegularSimpleCard> thisList)
    {
        if (_gameContainer.GetAttackStage == null)
        {
            throw new CustomBasicException("Nobody is handling the getattackstage.  Rethink");
        }
        if (_gameContainer.GetDefenseStage == null)
        {
            throw new CustomBasicException("Nobody is handling the getdefensestage.  Rethink");
        }
        BladesOfSteelPlayerItem thisPlayer = _gameContainer.PlayerList!.GetWhoPlayer();
        var attackStage = _gameContainer.GetAttackStage(thisPlayer.AttackList);
        if (attackStage == EnumAttackGroup.GreatOne)
        {
            return false;
        }
        var defenseStage = _gameContainer.GetDefenseStage(thisList);
        if (defenseStage == EnumDefenseGroup.StarGoalie)
        {
            return true;
        }
        if ((int)defenseStage > (int)attackStage)
        {
            return true;
        }
        if ((int)attackStage > (int)defenseStage)
        {
            return false;
        }
        int attackPoints = thisPlayer.AttackList.Sum(items => items.Value.Value);
        int defensePoints = thisList.Sum(items => items.Value.Value);
        return defensePoints >= attackPoints;
    }
}