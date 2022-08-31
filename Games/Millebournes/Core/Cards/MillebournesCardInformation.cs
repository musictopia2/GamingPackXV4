namespace Millebournes.Core.Cards;
public class MillebournesCardInformation : SimpleDeckObject, IDeckObject, IComparable<MillebournesCardInformation>
{
    public EnumCompleteCategories CompleteCategory { get; set; }
    private string _cardName = ""; // i could just directly do category.  must be public so it can be data bound.
    public string CardName
    {
        get
        {
            return _cardName;
        }
        set
        {
            if (SetProperty(ref _cardName, value) == true)
            {
                if (!string.IsNullOrEmpty(_cardName))
                {
                    CalculateCategory();// all needs names.  that is usually the last card to be filled in.
                }
            }
        }
    }
    private void CalculateCategory()
    {
        if (CompleteCategory != EnumCompleteCategories.None)
        {
            return;
        }
        if (Mileage > 0)
        {
            switch (Mileage)
            {
                case 25:
                    {
                        CompleteCategory = EnumCompleteCategories.Distance25;
                        break;
                    }
                case 50:
                    {
                        CompleteCategory = EnumCompleteCategories.Distance50;
                        break;
                    }
                case 75:
                    {
                        CompleteCategory = EnumCompleteCategories.Distance75;
                        break;
                    }
                case 100:
                    {
                        CompleteCategory = EnumCompleteCategories.Distance100;
                        break;
                    }
                case 200:
                    {
                        CompleteCategory = EnumCompleteCategories.Distance200;
                        break;
                    }
                default:
                    {
                        throw new CustomBasicException("Not Supported");
                    }
            }
            return;
        }
        if (CardType == EnumCardCategories.EndLimit)
        {
            CompleteCategory = EnumCompleteCategories.EndOfLimit;
            return;
        }
        if (CardType == EnumCardCategories.Speed)
        {
            CompleteCategory = EnumCompleteCategories.SpeedLimit;
            return;
        }
        switch (CardName)
        {
            case "Stop":
                {
                    CompleteCategory = EnumCompleteCategories.Stop;
                    break;
                }
            case "Accident":
                {
                    CompleteCategory = EnumCompleteCategories.Accident;
                    break;
                }
            case "Out Of Gas":
                {
                    CompleteCategory = EnumCompleteCategories.OutOfGas;
                    break;
                }
            case "Flat Tire":
                {
                    CompleteCategory = EnumCompleteCategories.FlatTire;
                    break;
                }
            case "Roll":
                {
                    CompleteCategory = EnumCompleteCategories.Roll;
                    break;
                }
            case "Repairs":
                {
                    CompleteCategory = EnumCompleteCategories.Repairs;
                    break;
                }
            case "Gasoline":
                {
                    CompleteCategory = EnumCompleteCategories.Gasoline;
                    break;
                }
            case "Spare Tire":
                {
                    CompleteCategory = EnumCompleteCategories.Spare;
                    break;
                }
            case "Right Of Way":
                {
                    CompleteCategory = EnumCompleteCategories.RightOfWay;
                    break;
                }
            case "Driving Ace":
                {
                    CompleteCategory = EnumCompleteCategories.DrivingAce;
                    break;
                }
            case "Extra Tank":
                {
                    CompleteCategory = EnumCompleteCategories.ExtraTank;
                    break;
                }
            case "Puncture Proof":
                {
                    CompleteCategory = EnumCompleteCategories.PunctureProof;
                    break;
                }
            default:
                {
                    throw new CustomBasicException(CardName + " not supported");
                }
        }
    }
    private int _mileage = 0;
    public int Mileage
    {
        get
        {
            return _mileage;
        }
        set
        {
            if (SetProperty(ref _mileage, value) == true)
            {
                if (value > 0)
                {
                    CardType = EnumCardCategories.Miles;// all miles show this.
                }
            }
        }
    }
    public EnumCardCategories CardType { get; set; }
    public MillebournesCardInformation()
    {
        DefaultSize = new SizeF(55, 72);
    }
    public void Populate(int chosen)
    {
        if (chosen == 0)
        {
            throw new CustomBasicException("Cannot choose 0");
        }
        if (chosen < 0 || chosen > 107)
        {
            throw new CustomBasicException("must be between 1 and 107");
        }
        Deck = chosen;
        if (chosen >= 47)
        {
            Mileage = 0;
        }
        switch (chosen)
        {
            case int _ when chosen < 11:
                Mileage = 25;
                CardName = "25 Miles";
                break;
            case int _ when chosen < 21:
                Mileage = 50;
                CardName = "50 Miles";
                break;
            case int _ when chosen < 31:
                Mileage = 75;
                CardName = "75 Miles";
                break;
            case int _ when chosen < 43:
                Mileage = 100;
                CardName = "100 Miles";
                break;
            case int _ when chosen < 47:
                Mileage = 200;
                CardName = "200 Miles";
                break;
            case int _ when chosen < 53:
                CardType = EnumCardCategories.Hazard;
                CardName = "Stop";
                break;
            case int _ when chosen < 56:
                CardType = EnumCardCategories.Hazard;
                CardName = "Flat Tire";
                break;
            case int _ when chosen < 59:
                CardType = EnumCardCategories.Hazard;
                CardName = "Out Of Gas";
                break;
            case int _ when chosen < 62:
                CardType = EnumCardCategories.Hazard;
                CardName = "Accident";
                break;
            case int _ when chosen < 66:
                CardType = EnumCardCategories.Speed;
                CardName = "Speed Limit";
                break;
            case int _ when chosen < 72:
                CardType = EnumCardCategories.Remedy;
                CardName = "Gasoline";
                break;
            case int _ when chosen < 78:
                CardType = EnumCardCategories.Remedy;
                CardName = "Repairs";
                break;
            case int _ when chosen < 84:
                CardType = EnumCardCategories.EndLimit;
                CardName = "End Of Limit";
                break;
            case int _ when chosen < 90:
                CardType = EnumCardCategories.Remedy;
                CardName = "Spare Tire";
                break;
            case int _ when chosen < 104:
                CardType = EnumCardCategories.Remedy;
                CardName = "Roll";
                break;
            case 104:
                CardType = EnumCardCategories.Safety;
                CardName = "Extra Tank";
                break;
            case 105:
                CardType = EnumCardCategories.Safety;
                CardName = "Puncture Proof";
                break;
            case 106:
                CardType = EnumCardCategories.Safety;
                CardName = "Right Of Way";
                break;
            case 107:
                CardType = EnumCardCategories.Safety;
                CardName = "Driving Ace";
                break;
            default:
                break;
        }
    }
    public void Reset() { }
    int IComparable<MillebournesCardInformation>.CompareTo(MillebournesCardInformation? other)
    {
        return Deck.CompareTo(other!.Deck);
    }
    public override BasicDeckRecordModel GetRecord => new(Deck, IsSelected, Drew, IsUnknown, IsEnabled, Visible, CardName); //i think.
    public override string GetKey()
    {
        return Guid.NewGuid().ToString();
    }
}