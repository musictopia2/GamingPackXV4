using System.Runtime.InteropServices;

namespace MonopolyCardGame.Core.Logic;
internal static class PopulateExtensions
{
    

    public static void PopulateManuelCards(this MonopolyCardGamePlayerItem player, MonopolyCardGameVMData model, bool start)
    {
        var firstList = player.MainHandList.Where(x => x.WhatCard != EnumCardType.IsMr && x.WhatCard != EnumCardType.IsGo).ToRegularDeckDict();
        if (start)
        {
            player.AdditionalCards.Clear();
        }
        else
        {
            firstList.AddRange(player.AdditionalCards);
        }
        //hopefully okay (?)
        //firstList.AddRange(player.AdditionalCards);
        //player.AdditionalCards.Clear(); //try now.
        firstList.ForEach(x => x.WasAutomated = false);

        var tempList = firstList.Where(x => x.WhatCard != EnumCardType.IsChance && x.WhatCard != EnumCardType.IsHouse && x.WhatCard != EnumCardType.IsHotel);

        var groups = tempList.GroupBy(x => x.WhatCard);

        BasicList<DeckRegularDict<MonopolyCardGameCardInformation>> fins = [];
        //BasicList<MonopolyCardGameCardInformation> others = [];
        //BasicList<MonopolyCardGameCardInformation> fins;
        foreach (var item in groups)
        {
            if (item.Key == EnumCardType.IsRailRoad && item.Count() > 1)
            {
                fins.Add(item.ToRegularDeckDict());
                continue;
            }
            if (item.Key == EnumCardType.IsUtilities && item.Count() == 2)
            {
                fins.Add(item.ToRegularDeckDict());
                continue;
            }
            var other = item.GroupBy(x => x.Money);
            foreach (var lasts in other)
            {
                //var card = lasts.First();
                int count = lasts.Count();
                if (lasts.Key == 50 || lasts.Key == 400)
                {
                    if (count == 2)
                    {
                        fins.Add(lasts.ToRegularDeckDict());
                    }
                    
                }
                else if (count == 3)
                {
                    fins.Add(lasts.ToRegularDeckDict());
                }
            }

            //var card = item.First();
            //var card = other.First().First();
            //if (card.Money == 50 || card.Money == 400)
            //{
            //    if (other.Count() == 2)
            //    {
            //        fins.Add(item.ToRegularDeckDict());
            //        continue;
            //    }
            //}
            //if (other.Count() == 3)
            //{
            //    fins.Add(item.ToRegularDeckDict());
            //}
        }
        BasicList<MonopolyCardGameCardInformation> temp = [];
        foreach (var firsts in fins)
        {
            foreach (var item in firsts)
            {
                firstList.RemoveSpecificItem(item); //because should be added to tempsets.
                item.WasAutomated = true; //this means cannot be selected.  but can still show the values though.
                if (start == false)
                {
                    temp.Add(item); //this means needs to be accounted for.
                }
                //SingleInfo.TempSets.Add(item);
                if (player.AdditionalCards.ObjectExist(item.Deck) == false)
                {
                    player.AdditionalCards.Add(item);
                    player.MainHandList.RemoveSpecificItem(item); //hopefully okay.  if it gets removed for good, okay when its testing.
                }
            }
        }
        int x = 0;
        if (start)
        {
            player.TempHands = firstList;
            model.TempHand1.HandList = player.TempHands;
            model.TempSets1.ClearBoard();
            foreach (var firsts in fins)
            {
                x++;
                model.TempSets1.AddCards(x, firsts);
            }
            return;
        }
        foreach (var firsts in fins)
        {
            //cannot clear the board this time.
            bool toEnd = false;
            do
            {
                x++;
                if (x > model.TempSets1.HowManySets)
                {
                    toEnd = true;
                    break;
                }
                if (model.TempSets1.HasAnyInSet(x) == false)
                {
                    break;
                }
                var card = model.TempSets1.GetFirstCardInSet(x);
                if (card.WhatCard == firsts.First().WhatCard)
                {
                    break; //try to add to this one instead.
                }
            } while (true);
            if (toEnd)
            {
                break;
            }
            //this means found one i can do to.
            model.TempSets1.AddCards(x, firsts);
            foreach (var card in firsts)
            {
                temp.RemoveSpecificItem(card); //because its accounted for now.
            }
        }
        foreach (var item in temp)
        {
            firstList.Add(item);
            player.AdditionalCards.RemoveSpecificItem(item); //because its now to main
            player.MainHandList.Add(item);
        }
        player.TempHands = firstList;
        model.TempHand1.HandList = player.TempHands;
    }
}