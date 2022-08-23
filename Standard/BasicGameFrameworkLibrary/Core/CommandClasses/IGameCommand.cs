namespace BasicGameFrameworkLibrary.Core.CommandClasses;

public interface IGameCommand : ICustomCommand
{
    EnumCommandBusyCategory BusyCategory { get; set; }
}