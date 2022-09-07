namespace BasicGameFrameworkLibrary.Core.BasicDrawables.Interfaces;
public interface ILocationDeck : IDeckObject
{
    PointF Location { get; set; } //location will be needed for the scattering pieces if i can ever get scattering to work
}