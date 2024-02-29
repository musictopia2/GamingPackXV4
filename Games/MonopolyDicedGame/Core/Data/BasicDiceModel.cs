namespace MonopolyDicedGame.Core.Data;
public class BasicDiceModel : IBasicDice<int>, IDiceContainer<int>
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

    

    public BasicList<int> GetPossibleList
    {
        get
        {
            //this will show the possibilities.
            BasicList<int> output = [];

            //for now, not sure how to populate.
            
            //anything on this list can be chosen.
            //this will show percentages of how likely something is to be chosen.
            //if the shared dice has nothing to do something else, needs to think about that possibility.
            //since this can have exceptions.

            //obviously if everything has been filled, then nothing can be chosen.



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
}