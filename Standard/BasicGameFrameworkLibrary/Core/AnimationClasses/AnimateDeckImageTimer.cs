namespace BasicGameFrameworkLibrary.Core.AnimationClasses;
public class AnimateDeckImageTimer
{
    public Action? StateChanged { get; set; }
    public int LongestTravelTime { get; set; }
    private double _startY;
    private double _destinationY;
    public double LocationYFrom { get; set; }
    public double LocationYTo { get; set; }
    public double CurrentYLocation { get; set; } = -1000;
    public async Task DoAnimateAsync()
    {
        CurrentYLocation = LocationYFrom;
        await FastSubmitAsync();
        _destinationY = LocationYTo;
        double totalYDistance;
        _startY = LocationYFrom;
        totalYDistance = _destinationY - _startY;
        var fins = totalYDistance / 2;
        CurrentYLocation += fins;
        await FastSubmitAsync();
        FinishAnimation();
    }
    private async Task FastSubmitAsync()
    {
        StateChanged!.Invoke();
        await Task.Delay(5);
    }
    private void FinishAnimation()
    {
        CurrentYLocation = -1000;
        StateChanged!.Invoke();
        EventExtensions.AnimationCompleted = true;
    }
}