namespace BasicGameFrameworkLibrary.Core.CommandClasses;
public static class InProgressHelpers
{
    private static bool _moveInProgress;
    private static bool _reconnecting;
    private static bool _starting;

    public static event Action? StateChanged;

    public static bool MoveInProgress
    {
        get => _moveInProgress;
        set
        {
            if (_moveInProgress == value)
            {
                return;
            }

            _moveInProgress = value;
            StateChanged?.Invoke();
        }
    }

    public static bool Reconnecting
    {
        get => _reconnecting;
        set
        {
            if (_reconnecting == value)
            {
                return;
            }

            _reconnecting = value;
            StateChanged?.Invoke();
        }
    }

    public static bool Starting
    {
        get => _starting;
        set
        {
            if (_starting == value)
            {
                return;
            }

            _starting = value;
            StateChanged?.Invoke();
        }
    }
}