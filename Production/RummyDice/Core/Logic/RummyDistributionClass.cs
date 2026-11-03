namespace RummyDice.Core.Logic;
public class RummyDistributionClass : IGenerateDice<int>
{
    public static RummyDicePlayerItem? CurrentPlayer { get; set; }
    GameTieredDistribution<int>? _tiers;
    int IGenerateDice<int>.GetRandomDiceValue(bool isLastItem)
    {
        if (isLastItem == false)
        {
            var list = Enumerable.Range(1, 52).ToBasicList();
            return list.GetRandomItem();
        }
        if (_tiers is null)
        {
            throw new CustomBasicException("No tiers.  Try to start roll first");
        }
        return _tiers.GetRandomItem();
    }
    void IGenerateDice<int>.StartRoll()
    {
        //this is where we have to do the distributions.
        if (CurrentPlayer is null)
        {
            throw new CustomBasicException("No player was set");
        }
        _tiers = new();
        int badWeight2 = 0;
        int veryluckyWeight2 = 0;
        int slightlyluckyWeight2 = 0;
        int averge2 = 0;
        int averge1;
        int slightlyluckyWeight1;
        int veryluckyWeight1;

        int badWeight1;
        if (CurrentPlayer.CurrentRepeats == 0)
        {
            badWeight1 = 3;
            badWeight2 = 5;
            veryluckyWeight1 = 3;
            veryluckyWeight2 = 5;
            slightlyluckyWeight1 = 5;
            slightlyluckyWeight2 = 10;
            averge1 = 20;
            averge2 = 30;
        }
        else if (CurrentPlayer.CurrentRepeats == 1)
        {
            badWeight1 = 2;
            veryluckyWeight1 = 5;
            slightlyluckyWeight1 = 15;
            averge1 = 10;
        }
        else if (CurrentPlayer.CurrentRepeats == 2)
        {
            badWeight1 = 1;
            veryluckyWeight1 = 5;
            veryluckyWeight2 = 10;
            slightlyluckyWeight1 = 10;
            slightlyluckyWeight2 = 20;
            averge1 = 10;
            averge2 = 15;
        }

        else if (CurrentPlayer.CurrentRepeats == 3)
        {
            //you repeated the most times.  this means you need the best chances to succeed.  otherwise, too boring.
            badWeight1 = 0; //can't be unlucky anymore.
            badWeight2 = 0;
            veryluckyWeight1 = 10;
            veryluckyWeight2 = 20;
            slightlyluckyWeight1 = 20;
            slightlyluckyWeight2 = 30;
            averge1 = 5;
            averge2 = 10;
        }
        else
        {
            //if you repeated more than 3 times, then you will most likely be very lucky for that roll to try to help you get the phase
            badWeight1 = 0; //can't be unlucky anymore.
            badWeight2 = 0;
            veryluckyWeight1 = 20;
            veryluckyWeight2 = 40;
            slightlyluckyWeight1 = 5;
            slightlyluckyWeight2 = 10;
            averge1 = 1;
            averge2 = 12;
        }
        if (CurrentPlayer.Phase == 10)
        {
            veryluckyWeight1 += 5;
            veryluckyWeight2 += 5;
            slightlyluckyWeight1 += 10;
            slightlyluckyWeight2 += 10;
            badWeight1 = 0;
            badWeight2 = 0;
            averge1 -= 2;
            averge2 -= 2; //lean less towards average on the last phase.
        }

        if (badWeight1 > 0 && badWeight2 > 0)
        {
            _tiers.AddTier(LuckStaticClass.Bad, badWeight1, badWeight2);
        }
        else if (badWeight1 > 0)
        {
            _tiers.AddTier(LuckStaticClass.Bad, badWeight1);
        }
        if (CurrentPlayer.CurrentRepeats == 1)
        {
            _tiers.AddTier(LuckStaticClass.Lucky, veryluckyWeight1)
                .AddTier(LuckStaticClass.SomewhatLucky, slightlyluckyWeight1)
                .AddTier(LuckStaticClass.Average, averge1);
        }
        else
        {
            _tiers.AddTier(LuckStaticClass.Lucky, veryluckyWeight1, veryluckyWeight2)
                .AddTier(LuckStaticClass.SomewhatLucky, slightlyluckyWeight1, slightlyluckyWeight2)
                .AddTier(LuckStaticClass.Average, averge1, averge2);
        }

        _tiers.Initialize(CurrentPlayer.Phase, CurrentPlayer.CurrentRepeats).PopulateVeryLuckTier(CurrentPlayer.Phase, CurrentPlayer.CurrentRepeats)
            .PopulateSomewhatLuckyTier(CurrentPlayer.Phase, CurrentPlayer.CurrentRepeats)
            .PopulateAverageTier(CurrentPlayer.Phase, CurrentPlayer.CurrentRepeats)
            .PopulateBadLuckTier(CurrentPlayer.Phase, CurrentPlayer.CurrentRepeats);
        //PopulateVeryLuckTier(tiers, CurrentPlayer.Phase, CurrentPlayer.CurrentRepeats);

    }
}