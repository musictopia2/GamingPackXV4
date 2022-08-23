﻿namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfacesForHelpers;
public interface IDiceMainProcesses<P> : IBasicGameProcesses<P>
    where P : class, IPlayerItem, new()
{
    Task HoldUnholdDiceAsync(int index);
}