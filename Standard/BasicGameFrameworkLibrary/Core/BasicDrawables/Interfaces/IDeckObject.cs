namespace BasicGameFrameworkLibrary.Core.BasicDrawables.Interfaces;
public interface IDeckObject : ICommonObject, IPopulateObject<int>
{
    int Deck { get; set; }
    bool Drew { get; set; }
    bool IsUnknown { get; set; }
    Action? ChangeSelectAction { get; set; }
    bool Rotated { get; set; }
    SizeF DefaultSize { get; set; }
    void Reset(); //sometimes needs to be reset.  everything can need it.
    /// <summary>
    /// this is needed to return a record.
    /// intended to be used to first return a record after something renders.
    /// then return again to determine whether to render so it knows whether to render to help in performance.  especially with blazor.
    /// </summary>
    /// <returns></returns>
    BasicDeckRecordModel GetRecord { get; }
    /// <summary>
    /// this is needed because many things needs a key to determine whether to dispose to recreate for cases that it gets updated.
    /// </summary>
    /// <returns></returns>
    string GetKey();
}