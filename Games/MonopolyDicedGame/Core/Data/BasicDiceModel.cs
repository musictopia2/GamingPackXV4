namespace MonopolyDicedGame.Core.Data;
public class BasicDiceModel : IBasicDice<int>, ISelectableObject, IDiceContainer<int>, IComparable<BasicDiceModel>
{
    public int HeightWidth => 50;
    public bool IsSelected { get; set; }
    public int Value { get; set; }
    public int Index { get; set; }
    public bool Visible { get; set; } = true;
    public EnumBasicType WhatDice { get; set; }
    public int Group { get; set; }
    public int GetMonopolyValue()
    {
        if (WhatDice == EnumBasicType.Railroad || WhatDice == EnumBasicType.Utility)
        {
            return WhatDice.GetMonopolyValue();
        }
        if (WhatDice == EnumBasicType.Chance)
        {
            throw new CustomBasicException("Should never have to get value for monopoly");
        }
        if (WhatDice != EnumBasicType.Property)
        {
            throw new CustomBasicException("Unable to get the monopoly value");
        }
        return Group.GetMonopolyValue();
    }
    public int GetRegularValue()
    {
        if (WhatDice == EnumBasicType.Railroad)
        {
            return 200;
        }
        if (WhatDice == EnumBasicType.Utility)
        {
            return 100;
        }
        if (WhatDice == EnumBasicType.Chance)
        {
            return 0;
        }
        if (WhatDice == EnumBasicType.Property)
        {
            if (Group == 8)
            {
                return 500;
            }
            if (Group == 7)
            {
                return 400;
            }
            return Group * 50;
        }
        throw new CustomBasicException("Regular Value Not Found");
    }
    public string GetColor()
    {
        return Group.GetColor();
    }
    public IGamePackageResolver? MainContainer { get; set; }
    public IGamePackageGeneratorDI? GeneratorContainer { get; set; }
    private static int UsedUp(int index)
    {
        int first;
        int second;
        //i think if the chance was used for one, then can't do 2 anymore obviously.
        if (index <= 8)
        {
            first = GlobalDiceHelpers.OwnWhenRolling.Count(x => x.Group == index);
            second = GlobalDiceHelpers.OwnedOnBoard.Count(x => x.Group == index);
            return first + second;
        }
        if (index == 9)
        {
            first = GlobalDiceHelpers.OwnWhenRolling.Count(x => x.Utility == EnumUtilityType.Water);
            second = GlobalDiceHelpers.OwnedOnBoard.Count(x => x.Utility == EnumUtilityType.Water);
            return first + second;
        }
        if (index == 10)
        {
            first = GlobalDiceHelpers.OwnWhenRolling.Count(x => x.Utility == EnumUtilityType.Electric);
            second = GlobalDiceHelpers.OwnedOnBoard.Count(x => x.Utility == EnumUtilityType.Electric);
            return first + second;
        }
        if (index == 11)
        {
            first = GlobalDiceHelpers.OwnWhenRolling.Count(x => x.UsedOn == EnumBasicType.Railroad);
            second = GlobalDiceHelpers.OwnedOnBoard.Count(x => x.UsedOn == EnumBasicType.Railroad);
            return first + second;
        }
        if (index == 12)
        {
            first = GlobalDiceHelpers.OwnWhenRolling.Count(x => x.WasChance);
            second = GlobalDiceHelpers.OwnedOnBoard.Count(x => x.WasChance);
            return first + second;
        }
        throw new CustomBasicException("Not Found");
    }
    public BasicList<int> GetPossibleList
    {
        get
        {
            //this will show the possibilities.
            BasicList<int> output;
            WeightedAverageLists<int> weights = new();
            int used;
            int upTo;
            bool ask;
            upTo = 8;
            used = UsedUp(upTo);
            ask = MonopolyDicedGameGameContainer.GlobalRandom!.NextBool(20);
            if (ask)
            {
                if (used == 0)
                {
                    weights.AddWeightedItem(upTo, 3); //for now pretend like its most likely to be boardwalk and parkplace.
                }
                else if (used == 1)
                {
                    weights.AddWeightedItem(upTo, 1); //want to make it much harder to get what is needed for monopoly.
                }
            }
            upTo = 7;
            used = UsedUp(upTo);
            if (used == 0)
            {
                weights.AddWeightedItem(upTo, 5);
            }
            else if (used == 1)
            {
                weights.AddWeightedItem(upTo, 3);
            }
            else if (used == 2)
            {
                weights.AddWeightedItem(upTo, 1);
            }
            upTo = 6;
            used = UsedUp(upTo);
            if (used == 0)
            {
                weights.AddWeightedItem(upTo, 6);
            }
            else if (used == 1)
            {
                weights.AddWeightedItem(upTo, 4);
            }
            else if (used == 2)
            {
                weights.AddWeightedItem(upTo, 1);
            }
            upTo = 5;
            used = UsedUp(upTo);
            if (used == 0)
            {
                weights.AddWeightedItem(upTo, 6);
            }
            else if (used == 1)
            {
                weights.AddWeightedItem(upTo, 4);
            }
            else if (used == 2)
            {
                weights.AddWeightedItem(upTo, 1);
            }
            upTo = 4;
            used = UsedUp(upTo);
            if (used == 0)
            {
                weights.AddWeightedItem(upTo, 8);
            }
            else if (used == 1)
            {
                weights.AddWeightedItem(upTo, 6);
            }
            else if (used == 2)
            {
                weights.AddWeightedItem(upTo, 1);
            }
            upTo = 3;
            used = UsedUp(upTo);
            if (used == 0)
            {
                weights.AddWeightedItem(upTo, 10);
            }
            else if (used == 1)
            {
                weights.AddWeightedItem(upTo, 7);
            }
            else if (used == 2)
            {
                weights.AddWeightedItem(upTo, 1);
            }
            upTo = 2;
            used = UsedUp(upTo);
            if (used == 0)
            {
                weights.AddWeightedItem(upTo, 12);
            }
            else if (used == 1)
            {
                weights.AddWeightedItem(upTo, 9);
            }
            else if (used == 2)
            {
                weights.AddWeightedItem(upTo, 1);
            }
            upTo = 1;
            used = UsedUp(upTo);
            if (used == 0)
            {
                weights.AddWeightedItem(upTo, 15);
            }
            else if (used == 1)
            {
                weights.AddWeightedItem(upTo, 2);
            }
            int waterUsed;
            int electricUsed;
            waterUsed = UsedUp(9);
            electricUsed = UsedUp(10);
            if (waterUsed == 0 && electricUsed == 0)
            {
                weights.AddWeightedItem(9, 4);
                weights.AddWeightedItem(10, 4);
            }
            else if (waterUsed > 0 && electricUsed == 0)
            {
                //electric alone.
                weights.AddWeightedItem(10, 1);
            }
            else if (waterUsed == 0 && electricUsed > 0)
            {
                weights.AddWeightedItem(9, 1);
            }
            int railroads = UsedUp(11);
            if (railroads == 0)
            {
                weights.AddWeightedItem(11, 8);
            }
            else if (railroads == 1)
            {
                weights.AddWeightedItem(11, 6);
            }
            else if (railroads == 2)
            {
                weights.AddWeightedItem(11, 3);
            }
            else if (railroads == 3)
            {
                weights.AddWeightedItem(11, 1);
            }
            int chances = UsedUp(12);
            ask = MonopolyDicedGameGameContainer.GlobalRandom!.NextBool(10);
            if (ask)
            {
                if (chances == 0)
                {
                    weights.AddWeightedItem(12, 2);
                }
                else if (chances == 1)
                {
                    weights.AddWeightedItem(12, 1);
                }
            }
            output = weights.GetWeightedList();
            return output;
        }
    }
    public void UseUtility(EnumUtilityType utility)
    {
        if (Index > 0)
        {
            throw new CustomBasicException("Must be blank dice first");
        }
        if (utility == EnumUtilityType.None)
        {
            throw new CustomBasicException("No utility was chosen");
        }
        Populate((int) utility);
    }
    public void UseChance()
    {
        if (Index > 0)
        {
            throw new CustomBasicException("Must be blank dice first");
        }
        Populate(12);
    }
    public void Populate(int chosen)
    {
        Index = chosen;
        if (chosen <= 8)
        {
            WhatDice = EnumBasicType.Property;
            Group = chosen;
            return;
        }
        if (chosen == 9 || chosen == 10)
        {
            WhatDice = EnumBasicType.Utility;
            return;
        }
        if (chosen == 11)
        {
            WhatDice = EnumBasicType.Railroad;
            return;
        }
        if (chosen == 12)
        {
            WhatDice = EnumBasicType.Chance;
            return;
        }
        throw new CustomBasicException("Must be between 1 and 12 for basic dice");
    }

    int IComparable<BasicDiceModel>.CompareTo(BasicDiceModel? other)
    {

        if (other!.Group > 0 && Group > 0)
        {
            return Group.CompareTo(other.Group);
        }
        return WhatDice.CompareTo(other.WhatDice);
    }
}