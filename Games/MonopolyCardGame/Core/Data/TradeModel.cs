namespace MonopolyCardGame.Core.Data;
public record struct TradeModel(BasicList<MonopolyCardGameCardInformation> YouReceive, 
    BasicList<MonopolyCardGameCardInformation> OpponentReceive,
    int OpponentPlayer
    );