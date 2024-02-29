namespace MonopolyDicedGame.Core.Data;
public class BasicDiceModel : IBasicDice<int>, IDiceContainer<int>
{
    public int HeightWidth => 50;
    public bool IsSelected { get; set; }
    public int Value { get; set; }
    public int Index { get; set; }
    public bool Visible { get; set; } = true;
    public EnumBasicType WhatCard { get; set; }
    public int Group { get; set; }
    public int GetMonopolyValue()
    {
        if (WhatCard == EnumBasicType.Railroad || WhatCard == EnumBasicType.Utility)
        {
            return WhatCard.GetMonopolyValue();
        }
        if (WhatCard == EnumBasicType.Chance)
        {
            throw new CustomBasicException("Should never have to get value for monopoly");
        }
        if (WhatCard != EnumBasicType.Property)
        {
            throw new CustomBasicException("Unable to get the monopoly value");
        }
        return Group.GetMonopolyValue();
    }
    public int GetRegularValue()
    {
        if (WhatCard == EnumBasicType.Railroad)
        {
            return 200;
        }
        if (WhatCard == EnumBasicType.Utility)
        {
            return 100;
        }
        if (WhatCard == EnumBasicType.Chance)
        {
            return 0;
        }
        if (WhatCard == EnumBasicType.Property)
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
    public void Populate(int chosen)
    {
        Index = chosen;
        if (chosen <= 8)
        {
            WhatCard = EnumBasicType.Property;
            Group = chosen;
            return;
        }
        if (chosen == 9 || chosen == 10)
        {
            WhatCard = EnumBasicType.Utility;
            return;
        }
        if (chosen == 11)
        {
            WhatCard = EnumBasicType.Railroad;
            return;
        }
        if (chosen == 12)
        {
            WhatCard = EnumBasicType.Chance;
            return;
        }
        throw new CustomBasicException("Must be between 1 and 12 for basic dice");
    }
}