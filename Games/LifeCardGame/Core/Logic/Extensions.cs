﻿namespace LifeCardGame.Core.Logic;
public static class Extensions
{
    public static DeckRegularDict<LifeCardGameCardInformation> OpponentStory(this PlayerCollection<LifeCardGamePlayerItem> thisList)
    {
        var newList = thisList.Where(items => items.PlayerCategory != EnumPlayerCategory.Self).ToBasicList();
        DeckRegularDict<LifeCardGameCardInformation> output = new();
        newList.ForEach(thisPlayer => output.AddRange(thisPlayer.LifeStory!.HandList));
        return output;
    }
    public static bool HasYears(this PlayerCollection<LifeCardGamePlayerItem> thisList)
    {
        return thisList.Any(thisPlayer => thisPlayer.MainHandList.Any(thisCard => thisCard.Points == 0));
    }
    public static async Task RepositionCardsAsync(this PlayerCollection<LifeCardGamePlayerItem> thisList, LifeCardGameMainGameClass mainGame, LifeCardGameGameContainer gameContainer, LifeCardGameVMData model)
    {
        bool rets;
        rets = model.CurrentPile!.PileEmpty();
        LifeCardGameCardInformation? thisCard = null;
        EnumSelectMode thisMode = EnumSelectMode.ChoosePlayer; // must prove its card
        if (rets == false)
        {
            thisCard = model.CurrentPile.GetCardInfo();
            if (thisCard.Action == EnumAction.Lawsuit && mainGame.OtherTurn > 0)
            {
                thisMode = EnumSelectMode.ChooseCard;
            }
            else if (thisCard.Action == EnumAction.DonateToCharity && mainGame.OtherTurn > 0)
            {
                thisMode = EnumSelectMode.ChooseCard;
            }
            else if (thisCard.Action == EnumAction.CareerSwap || thisCard.Action == EnumAction.MovingHouse || thisCard.Action == EnumAction.YourStory || thisCard.Action == EnumAction.SecondChance || thisCard.Action == EnumAction.AdoptBaby || thisCard.Action == EnumAction.LongLostRelative || thisCard.Action == EnumAction.MixUpAtVets)
            {
                thisMode = EnumSelectMode.ChooseCard;
            }
        }
        if (thisMode == EnumSelectMode.ChoosePlayer)
        {
            if (gameContainer.CloseOtherScreenAsync != null)
            {
                await gameContainer.CloseOtherScreenAsync();
            }
        }
        else
        {
            if (thisCard!.Action == EnumAction.Lawsuit)
            {
                model.OtherText = "Give Card"; // lawsuit is larger than second chance
            }
            else if (thisCard.Action == EnumAction.DonateToCharity)
            {
                model.OtherText = "Donate Card";
            }
            else if (thisCard.Action == EnumAction.SecondChance)
            {
                model.OtherText = "Take Card";
            }
            else if (thisCard.Action == EnumAction.AdoptBaby)
            {
                model.OtherText = "Adopt Baby";
            }
            else if (thisCard.Action == EnumAction.CareerSwap)
            {
                model.OtherText = "Switch Careers";
            }
            else if (thisCard.Action == EnumAction.MovingHouse)
            {
                model.OtherText = "Move House";
            }
            else if (thisCard.Action == EnumAction.YourStory)
            {
                model.OtherText = "Take Adventure";
            }
            else if (thisCard.Action == EnumAction.MixUpAtVets)
            {
                model.OtherText = "Switch Pets";
            }
            else if (thisCard.Action == EnumAction.LongLostRelative)
            {
                model.OtherText = "Take Family";
            }
            else
            {
                throw new Exception("Don't know for " + thisCard.Action.ToString());
            }
            if (gameContainer.LoadOtherScreenAsync != null)
            {
                await gameContainer.LoadOtherScreenAsync();
            }
        }
        thisList.ForEach(thisPlayer =>
        {
            var index = thisPlayer.Id;
            if (index == gameContainer.PlayerChosen || gameContainer.PlayerChosen == 0)
            {
                thisPlayer.LifeStory!.Visible = true;
            }
            else
            {
                thisPlayer.LifeStory!.Visible = false;
            }
            thisPlayer.LifeStory.Mode = thisMode;
        });
    }
    public static void EnableLifeStories(this PlayerCollection<LifeCardGamePlayerItem> thisList, LifeCardGameMainGameClass mainGame, LifeCardGameVMData model, bool enables)
    {
        bool newEnables = true;
        bool rets;
        rets = model.CurrentPile!.PileEmpty();
        if (rets == true && enables == true)
        {
            newEnables = false;
        }
        LifeCardGameCardInformation? thisCard = null;
        if (rets == false)
        {
            thisCard = model.CurrentPile.GetCardInfo();
            if (thisCard.Action == EnumAction.None && enables == true)
            {
                newEnables = false;
            }
            if (thisCard.Action == EnumAction.TurnBackTime && enables == true)
            {
                newEnables = false;
            }
        }
        if (enables == false)
            newEnables = false;
        if (newEnables == false)
        {
            thisList.ForEach(thisPlayer =>
            {
                thisPlayer.LifeStory!.IsEnabled = false;
            });
            return;
        }
        newEnables = false; //now has to be proven true again.
        if (thisCard!.Action == EnumAction.CareerSwap || thisCard.Action == EnumAction.MixUpAtVets || thisCard.Action == EnumAction.MovingHouse)
        {
            thisList.ForEach(player =>
            {
                player.LifeStory!.IsEnabled = true;
            });
            return;
        }
        thisList.ForEach(player =>
        {
            if (thisCard.Action == EnumAction.AdoptBaby || thisCard.Action == EnumAction.IMTheBoss || thisCard.Action == EnumAction.LifeSwap || thisCard.Action == EnumAction.LongLostRelative || thisCard.Action == EnumAction.LostPassport || thisCard.Action == EnumAction.MidlifeCrisis || thisCard.Action == EnumAction.SecondChance || thisCard.Action == EnumAction.YoureFired || thisCard.Action == EnumAction.YourStory)
            {
                if (player.PlayerCategory != EnumPlayerCategory.Self)
                {
                    newEnables = true;
                }
                else
                {
                    newEnables = false;
                }
            }
            else if (thisCard.Action == EnumAction.Lawsuit && mainGame.OtherTurn == 0)
            {
                if (player.PlayerCategory != EnumPlayerCategory.Self)
                {
                    newEnables = true;
                }
                else
                {
                    newEnables = false;
                }
            }
            else if (thisCard.Action == EnumAction.DonateToCharity && mainGame.OtherTurn == 0)
            {
                if (player.PlayerCategory != EnumPlayerCategory.Self)
                {
                    newEnables = true;
                }
                else
                {
                    newEnables = false;
                }
            }
            else if (thisCard.Action == EnumAction.DonateToCharity)
            {
                if (player.PlayerCategory == EnumPlayerCategory.Self)
                {
                    newEnables = true;
                }
                else
                {
                    newEnables = false;
                }
            }
            else if (thisCard.Action == EnumAction.Lawsuit)
            {
                if (player.PlayerCategory == EnumPlayerCategory.Self)
                {
                    newEnables = true;
                }
                else
                {
                    newEnables = false;
                }
            }
            else
            {
                throw new CustomBasicException("Needs to be accounted for");
            }
            player.LifeStory!.IsEnabled = newEnables;
        });
    }
}