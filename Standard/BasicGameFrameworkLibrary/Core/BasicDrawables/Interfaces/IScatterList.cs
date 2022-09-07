namespace BasicGameFrameworkLibrary.Core.BasicDrawables.Interfaces;
public interface IScatterList<D> : ISimpleList<D>, IDeckShuffler<D> where D : IDeckObject, new() { }