namespace Fluxx.Core.Logic;
[SingletonGame]
[AutoReset]
public class AnalyzeProcesses(FluxxGameContainer gameContainer) : IAnalyzeProcesses
{
    async Task IAnalyzeProcesses.AnalyzeQueAsync()
    {
        if (gameContainer.AnimatePlayAsync == null)
        {
            throw new CustomBasicException("Nobody is animating plays.  Rethink");
        }
        if (gameContainer!.QuePlayList.Count == 0 || gameContainer.TempActionHandList.Count > 0 || gameContainer.CurrentAction != null)
        {
            if (gameContainer.TempActionHandList.Count == 0 && gameContainer.CurrentAction == null)
            {
                await gameContainer.SaveRoot!.SavedActionData.TempDiscardList.ForEachAsync(async thisDiscard =>
                {
                    var thisCard = gameContainer.DeckList!.GetSpecificItem(thisDiscard);
                    await gameContainer.AnimatePlayAsync(thisCard);
                });
                gameContainer.SaveRoot.SavedActionData.TempDiscardList.Clear();
            }
            gameContainer.SaveRoot!.DoAnalyze = false;
            await gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        AnalyzeRules();
        if (gameContainer.LeftToDraw > 0)
        {
            await gameContainer.DrawAsync!.Invoke();
            return;
        }
        var tempList = gameContainer.PlayerList!.AllPlayersExceptForCurrent();
        if (tempList.Any(items => items.ObeyedRulesWhenNotYourTurn() == false))
        {
            gameContainer.SaveRoot!.DoAnalyze = false;
            await gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        gameContainer.OtherTurn = 0;
        gameContainer.SingleInfo = gameContainer.PlayerList.GetWhoPlayer();
        if (gameContainer.NeedsToRemoveGoal())
        {
            gameContainer.SaveRoot!.DoAnalyze = false;
            await gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        int wins = gameContainer.WhoWonGame();
        if (wins > 0)
        {
            gameContainer.SingleInfo = gameContainer.PlayerList[wins];
            await gameContainer.ShowWinAsync!.Invoke();
            return;
        }
        gameContainer.SaveRoot!.DoAnalyze = true;
        if (gameContainer.SaveStateAsync == null)
        {
            throw new CustomBasicException("Nobody is saving state.  Rethink");
        }
        await gameContainer.SaveStateAsync();
        if (gameContainer.PlayCardAsync == null)
        {
            throw new CustomBasicException("Nobody is playing card.  Rethink");
        }
        await gameContainer.PlayCardAsync(gameContainer.QuePlayList.First().Deck);
    }
    private void AnalyzeRules()
    {
        int extraAmount = gameContainer!.IncreaseAmount();
        int possibleBonus = extraAmount + 1;
        if (gameContainer.SaveRoot!.RuleList.Any(items => items.Deck == EnumRuleText.PoorBonus))
        {
            if (gameContainer.SaveRoot.DrawBonus < possibleBonus && gameContainer.HasFewestKeepers())
            {
                gameContainer.SaveRoot.DrawBonus = possibleBonus;
            }
            else
            {
                gameContainer.SaveRoot.DrawBonus = 0; //try this as well.
            }
        }
        if (gameContainer.SaveRoot.RuleList.Any(items => items.Deck == EnumRuleText.RichBonus))
        {
            if (gameContainer.SaveRoot.PlayBonus < possibleBonus && gameContainer.HasMostKeepers())
            {
                gameContainer.SaveRoot.PlayBonus = possibleBonus;
            }
            else
            {
                gameContainer.SaveRoot.PlayBonus = 0;
            }
        }
        RuleCard thisRule;
        thisRule = gameContainer.SaveRoot.RuleList.Where(items => items.Category == EnumRuleCategory.Draw).SingleOrDefault()!;
        if (thisRule == null)
        {
            gameContainer.SaveRoot.DrawRules = possibleBonus;
        }
        else
        {
            if (thisRule.HowMany == 0)
            {
                throw new CustomBasicException("Cannot be 0");
            }
            gameContainer.SaveRoot.DrawRules = extraAmount + thisRule.HowMany;
        }
        thisRule = gameContainer.SaveRoot.RuleList.Where(items => items.Category == EnumRuleCategory.Play).SingleOrDefault()!;
        if (thisRule == null)
        {
            gameContainer.SaveRoot.PlayLimit = possibleBonus;
        }
        else if (thisRule.HowMany == -1)
        {
            gameContainer.SaveRoot.PlayLimit = -1; //means unlimited.
        }
        else
        {
            if (thisRule.HowMany == 0)
            {
                throw new CustomBasicException("Cannot be 0");
            }
            gameContainer.SaveRoot.PlayLimit = extraAmount + thisRule.HowMany;
        }
        thisRule = gameContainer.SaveRoot.RuleList.Where(items => items.Category == EnumRuleCategory.Hand).SingleOrDefault()!;
        if (thisRule == null)
        {
            gameContainer.SaveRoot.HandLimit = -1; //means unlimited
        }
        else
        {
            gameContainer.SaveRoot.HandLimit = extraAmount + thisRule.HowMany;
        }
        thisRule = gameContainer.SaveRoot.RuleList.Where(items => items.Category == EnumRuleCategory.Keeper).SingleOrDefault()!;
        if (thisRule == null)
        {
            gameContainer.SaveRoot.KeeperLimit = -1; //means unlimited
        }
        else
        {
            gameContainer.SaveRoot.KeeperLimit = extraAmount + thisRule.HowMany;
        }
        if (gameContainer.SaveRoot.PlayLimit == -1)
        {
            gameContainer.SaveRoot.PlaysLeft = gameContainer.SingleInfo!.MainHandList.Count;
        }
        else
        {
            gameContainer.SaveRoot.PlaysLeft = gameContainer.SaveRoot.PlayLimit + gameContainer.SaveRoot.PlayBonus - gameContainer.SaveRoot.CardsPlayed;
        }
        if (gameContainer.SaveRoot.PlaysLeft < 0)
        {
            gameContainer.SaveRoot.PlaysLeft = 0;
        }
        if (gameContainer.SaveRoot.PlaysLeft > gameContainer.SingleInfo!.MainHandList.Count)
        {
            gameContainer.SaveRoot.PlaysLeft = gameContainer.SingleInfo.MainHandList.Count;
        }
        gameContainer.LeftToDraw = ExtraCardsToDraw();
        if (gameContainer.LeftToDraw > 0)
        {
            gameContainer.SaveRoot.CardsDrawn += gameContainer.LeftToDraw;
        }
    }
    private int ExtraCardsToDraw()
    {
        int howMany = gameContainer.SaveRoot!.PreviousBonus + gameContainer.SaveRoot.DrawRules + gameContainer.SaveRoot.DrawBonus - gameContainer.SaveRoot.CardsDrawn;
        if (howMany <= 0)
        {
            return 0;
        }
        return howMany;
    }
    void IAnalyzeProcesses.AnalyzeRules()
    {
        AnalyzeRules();
    }
}