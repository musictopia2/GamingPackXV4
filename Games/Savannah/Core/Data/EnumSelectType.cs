namespace Savannah.Core.Data;
public enum EnumSelectType
{
    None,
    FromHand,
    FromReserve,
    FromDiscard //since you have to specify player, then should be okay.
    //decided to not even allow a person to play from the public discard piles.  since we have extra deck, hopefully will be okay.
}