﻿namespace DominosMexicanTrain.Core.Logic;
[SingletonGame]
[AutoReset]
public class GlobalClass
{
    public AnimateBasicGameBoard? Animates { get; set; }
    public MexicanDomino? MovingDomino { get; set; }
    internal DominosBoneYardClass<MexicanDomino>? BoneYard { get; set; }
    internal TrainStationBoardProcesses? TrainStation1 { get; set; }
}