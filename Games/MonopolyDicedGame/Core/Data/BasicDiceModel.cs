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
        if (WhatCard == EnumBasicType.Railroad)
        {
            return 2500;
        }
        if (WhatCard == EnumBasicType.Utility)
        {
            return 800;
        }
        if (WhatCard == EnumBasicType.Chance)
        {
            throw new CustomBasicException("Should never have to get value for monopoly");
        }
        if (WhatCard != EnumBasicType.Property)
        {
            throw new CustomBasicException("Unable to get the monopoly value");
        }
        if (Group == 1)
        {
            return 600;
        }
        if (Group == 2)
        {
            return 1000;
        }
        if (Group == 3)
        {
            return 1500;
        }
        if (Group == 4)
        {
            return 1800;
        }
        if (Group == 5)
        {
            return 2200;
        }
        if (Group == 6)
        {
            return 2700;
        }
        if (Group == 7)
        {
            return 3000;
        }
        if (Group == 8)
        {
            return 3500;
        }
        throw new CustomBasicException("Unable to get the property value");
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
        if (Group <= 0)
        {
            throw new CustomBasicException("No need for color because no group found");
        }
        if (Group == 1)
        {
            return cc1.Brown.ToWebColor();
        }
        if (Group == 2)
        {
            return cc1.Cyan.ToWebColor();
        }
        if (Group == 3)
        {
            return cc1.MediumVioletRed.ToWebColor();
        }
        if (Group == 4)
        {
            return cc1.DarkOrange.ToWebColor();
        }
        if (Group == 5)
        {
            return cc1.Red.ToWebColor();
        }
        if (Group == 6)
        {
            return cc1.Yellow.ToWebColor();
        }
        if (Group == 7)
        {
            return cc1.Green.ToWebColor();
        }
        if (Group == 8)
        {
            return cc1.DarkBlue.ToWebColor();
        }
        throw new CustomBasicException("Only 1 to 8 are supported");
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