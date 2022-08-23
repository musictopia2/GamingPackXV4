﻿namespace BasicGameFrameworkLibrary.Core.NetworkingClasses.Interfaces;

public interface ISignalRInfo : ITCPInfo
{
    Task<bool> IsAzureAsync();
    Task<string> GetEndPointAsync();
}