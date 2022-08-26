﻿namespace BasicMultiplayerGames.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class BasicMultiplayerGamesVMData : IViewModelData
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";

    //any other ui related properties will be here.
    //can copy/paste for the actual view model.

}