namespace YahtzeeHandsDown.Core.Cards;
public interface ICard
{
    int FirstValue { get; set; }
    int SecondValue { get; set; }
    bool IsWild { get; set; }
    EnumColor Color { get; set; }
}