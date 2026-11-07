namespace BasicGameFrameworkLibrary.Core.NetworkingClasses.Interfaces;
public interface ISignalRInfo
{
    Task<string> GetIPAddressAsync();
    Task<int?> GetPortAsync(); //now optional.
    Task<string> GetEndPointAsync();
}