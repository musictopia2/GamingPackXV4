namespace MonopolyCardGame.Core.Logic;
internal static class PopulateExtensions
{
    //public static void PopulateManuelCards(this MonopolyCardGamePlayerItem player, MonopolyCardGameVMData model, bool start)
    //{
    //    var firstList = player.MainHandList.Where(x => x.WhatCard != EnumCardType.IsMr && x.WhatCard != EnumCardType.IsGo).ToRegularDeckDict();
    //    if (start)
    //    {
    //        player.AdditionalCards.Clear();
    //    }
    //    else
    //    {
    //        firstList.AddRange(player.AdditionalCards);
    //    }
    //    firstList.ForEach(x => x.WasAutomated = false);
    //    var tempList = firstList.Where(x => x.WhatCard != EnumCardType.IsChance && x.WhatCard != EnumCardType.IsHouse && x.WhatCard != EnumCardType.IsHotel);
    //    var groups = tempList.GroupBy(x => x.WhatCard);
    //    BasicList<DeckRegularDict<MonopolyCardGameCardInformation>> fins = [];
    //    foreach (var item in groups)
    //    {
    //        if (item.Key == EnumCardType.IsRailRoad && item.Count() > 1)
    //        {
    //            fins.Add(item.ToRegularDeckDict());
    //            continue;
    //        }
    //        if (item.Key == EnumCardType.IsUtilities && item.Count() == 2)
    //        {
    //            fins.Add(item.ToRegularDeckDict());
    //            continue;
    //        }
    //        var other = item.GroupBy(x => x.Money);
    //        foreach (var lasts in other)
    //        {
    //            int count = lasts.Count();
    //            if (lasts.Key == 50 || lasts.Key == 400)
    //            {
    //                if (count == 2)
    //                {
    //                    fins.Add(lasts.ToRegularDeckDict());
    //                }

    //            }
    //            else if (count == 3)
    //            {
    //                fins.Add(lasts.ToRegularDeckDict());
    //            }
    //        }
    //    }
    //    BasicList<MonopolyCardGameCardInformation> temp = [];
    //    foreach (var firsts in fins)
    //    {
    //        foreach (var item in firsts)
    //        {
    //            firstList.RemoveSpecificItem(item);
    //            item.WasAutomated = true;
    //            if (start == false)
    //            {
    //                temp.Add(item);
    //            }
    //            if (player.AdditionalCards.ObjectExist(item.Deck) == false)
    //            {
    //                player.AdditionalCards.Add(item);
    //                player.MainHandList.RemoveSpecificItem(item); //hopefully okay.  if it gets removed for good, okay when its testing.
    //            }
    //        }
    //    }
    //    int x = 0;
    //    if (start)
    //    {
    //        player.TempHands = firstList;
    //        model.TempHand1.HandList = player.TempHands;
    //        model.TempSets1.ClearBoard();
    //        foreach (var firsts in fins)
    //        {
    //            x++;
    //            model.TempSets1.AddCards(x, firsts);
    //        }
    //        return;
    //    }
    //    foreach (var firsts in fins)
    //    {
    //        bool toEnd = false;
    //        do
    //        {
    //            x++;
    //            if (x > model.TempSets1.HowManySets)
    //            {
    //                toEnd = true;
    //                break;
    //            }
    //            if (model.TempSets1.HasAnyInSet(x) == false)
    //            {
    //                break;
    //            }
    //            var card = model.TempSets1.GetFirstCardInSet(x);
    //            if (card.WhatCard == firsts.First().WhatCard)
    //            {
    //                break; //try to add to this one instead.
    //            }
    //        } while (true);
    //        if (toEnd)
    //        {
    //            break;
    //        }
    //        model.TempSets1.AddCards(x, firsts);
    //        foreach (var card in firsts)
    //        {
    //            temp.RemoveSpecificItem(card); //because its accounted for now.
    //        }
    //    }
    //    foreach (var item in temp)
    //    {
    //        firstList.Add(item);
    //        player.AdditionalCards.RemoveSpecificItem(item); //because its now to main
    //        player.MainHandList.Add(item);
    //    }
    //    player.TempHands = firstList;
    //    model.TempHand1.HandList = player.TempHands;
    //}
}