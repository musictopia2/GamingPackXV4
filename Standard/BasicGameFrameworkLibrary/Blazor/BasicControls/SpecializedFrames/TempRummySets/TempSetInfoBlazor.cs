namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SpecializedFrames.TempRummySets;
public class TempSetInfoBlazor<SU, CO, RU>
    where SU : IFastEnumSimple
    where CO : IFastEnumColorSimple
    where RU : class, IRummmyObject<SU, CO>, IDeckObject, new()
{
    public RU? Image { get; set; }
    public HandObservable<RU>? Hand { get; set; }
}