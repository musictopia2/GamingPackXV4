namespace MonopolyCardGame.Core.Data;
public enum EnumCalculatorStatus
{
    None, //this means no screen opened.
    ChooseCardCategory, //if you are starting, needs to start with this.  if you choose utilities, then its easy.
    ChooseNumberOfRailroads,
    ChoosePropertyInformation
    //EditRailroads,
    //EditProperty
}