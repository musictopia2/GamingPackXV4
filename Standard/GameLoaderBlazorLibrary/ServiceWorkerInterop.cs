namespace GameLoaderBlazorLibrary;
internal class ServiceWorkerInterop(IJSRuntime js) : BaseLibraryJavascriptClass(js)
{
    protected override string JavascriptFileName => "serviceWorkerInterop";
    private DotNetObjectReference<ServiceWorkerInterop>? _dotNetRef;
    private Func<Task>? _onUpdateAvailable;
    public async Task RegisterAsync(Func<Task> onUpdateAvailableCallback)
    {
        _onUpdateAvailable = onUpdateAvailableCallback;
        _dotNetRef = DotNetObjectReference.Create(this);
        await ModuleTask.InvokeVoidFromClassAsync("registerAndListen", _dotNetRef);
    }
    [JSInvokable]
    public async Task NotifyUpdateAvailable()
    {
        if (_onUpdateAvailable is not null)
        {
            await _onUpdateAvailable.Invoke();
        }
    }
    protected override ValueTask DisposeAsyncCore()
    {
        _dotNetRef?.Dispose();
        return base.DisposeAsyncCore();
    }
}