﻿namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceMessages;
public interface IReshuffledCardsNM
{
    Task ReshuffledCardsReceived(string data); //whoever is implementing has to decide what to do from here.
}