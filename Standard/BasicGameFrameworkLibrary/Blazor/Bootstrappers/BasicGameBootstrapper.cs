﻿using BasicGameFrameworkLibrary.Core.NetworkingClasses.Misc; //not common enough at this point.
namespace BasicGameFrameworkLibrary.Blazor.Bootstrappers;
public abstract partial class BasicGameBootstrapper<TViewModel> : IGameBootstrapper, IHandleAsync<SocketErrorEventModel>,
    IHandleAsync<DisconnectEventModel>, IDisposable
    where TViewModel : IMainGPXShellVM //needs generic so its able to do the part to active a screen if any.
{
    private readonly IStartUp? _startInfo;
    private readonly EnumGamePackageMode _mode;
    private ISystemError? _error;
    private IMessageBox? _message;
    public BasicGameBootstrapper(IStartUp starts, EnumGamePackageMode mode)
    {
        _startInfo = starts;
        _mode = mode;
        _aggregator = new EventAggregator();
        InitalizeAsync();
    }
    private partial void Subscribe();
    private partial void Unsubscribe();
    bool _isInitialized;
    private static void ResetGlobals()
    {
        MiscDelegates.GetMiscObjectsToReplace = null;
        MiscDelegates.ColorsFinishedAsync = null; //needs to set all to null.  best to just do this way.
        MiscDelegates.ComputerChooseColorsAsync = null;
        MiscDelegates.ContinueColorsAsync = null;
        MiscDelegates.FillRestColors = null;
        MiscDelegates.ManuelSetColors = null;
        MiscDelegates.GetAutoGeneratedObjectsToReplace = null;
        RegularSimpleCard.ClearSavedList(); //i think should be here instead.  the fact others do it should not hurt.  best to be safe than sorry.
    }
    public async void InitalizeAsync()
    {
        if (_isInitialized)
        {
            return;
        }
        _error = BlazorUIHelpers.SystemError;
        _message = BlazorUIHelpers.MessageBox;
        MVVMFramework.EventAggravatorProcesses.GlobalEventAggravatorClass.ClearSubscriptions(_aggregator);
        EventAggravatorProcesses.GlobalEventAggravatorClass.ClearSubscriptions(_aggregator);
        js1.RequireCustomSerialization = true;
        ResetGlobals();
        _isInitialized = true;
        await StartRuntimeAsync();
    }
    protected BasicData? GameData;
    protected TestOptions? TestData;
    private readonly IEventAggregator _aggregator;

    /// <summary>
    /// Called by the bootstrapper's constructor at runtime to start the framework.
    /// </summary>
    protected async Task StartRuntimeAsync()
    {
        if (_mode == EnumGamePackageMode.None)
        {
            Console.WriteLine("Closing out because must be debug or production.");
            return;
        }
        StartUp();
        SetPersonalSettings();
        _container = new GamePackageDIContainer();
        Resolver = _container;
        FirstRegister();
        await ConfigureAsync(_container);
        if (_mode == EnumGamePackageMode.Debug)
        {
            await RegisterTestsAsync();
        }
        if (UseMultiplayerProcesses)
        {
            RegisterMultiplayer(_container);
        }
        await DisplayRootViewForAsync();
    }
    private void RegisterMultiplayer(IGamePackageRegister register)
    {
        register.RegisterType<BasicMessageProcessing>(true);
        IRegisterNetworks tempnets = _container!.Resolve<IRegisterNetworks>();
        tempnets.RegisterMultiplayerClasses(_container);
    }
    protected virtual Task RegisterTestsAsync() { return Task.CompletedTask; }
    protected abstract bool UseMultiplayerProcesses { get; }
    protected virtual void StartUp() { }
    private void FirstRegister()
    {
        Core.AutoResumeContexts.GlobalRegistrations.Register();
        _container!.RegisterStartup(_startInfo!);
        _container.RegisterSingleton(BlazorUIHelpers.Toast);
        _container.RegisterSingleton(_message);
        _container.RegisterSingleton(_error);
        EventAggregator thisEvent = new();
        MessengingGlobalClass.Aggregator = thisEvent;
        Subscribe(); //now i can use this.
        _container!.RegisterSingleton(thisEvent);
        TestData = new TestOptions();
        _container.RegisterSingleton(TestData);
        CommandContainer thisCommand = new();
        _container.RegisterSingleton(thisCommand);
        BasicRegistrations(_container);
        MiscRegisterFirst(_container);
        _container.RegisterSingleton(_container);
        GameData = new()
        {
            GamePackageMode = _mode
        };
        _container.RegisterSingleton(GameData);
        _startInfo!.RegisterCustomClasses(_container, UseMultiplayerProcesses, GameData);
    }
    private static void BasicRegistrations(IGamePackageRegister register)
    {
        register.RegisterType<NewGameViewModel>(false);
        register.RegisterSingleton<IAsyncDelayer, AsyncDelayer>();
    }
    protected virtual void MiscRegisterFirst(IGamePackageRegister register) { }

    /// <summary>
    /// if we need custom registrations but still need standard, then override but do the regular functions too.
    /// </summary>
    /// <returns></returns>
    protected abstract Task ConfigureAsync(IGamePackageRegister register);
    private GamePackageDIContainer? _container;
    protected IGamePackageDIContainer GetDIContainer => _container!;
    protected virtual void SetPersonalSettings() { }
    /// <summary>
    /// this will allow source generators to run to finish the dependency injection registrations.
    /// </summary>
    protected abstract void FinishRegistrations(IGamePackageRegister register);
    protected async Task DisplayRootViewForAsync()
    {
        Core.DIFinishProcesses.GlobalDIFinishClass.FinishDIRegistrations(_container!);
        FinishRegistrations(_container!);
        object item = _container!.Resolve<TViewModel>()!;
        if (item is IScreen screen)
        {
            await screen.ActivateAsync();
        }
    }
    async Task IHandleAsync<SocketErrorEventModel>.HandleAsync(SocketErrorEventModel message)
    {
        if (message.Category == EnumSocketCategory.Client)
        {
            await _message!.ShowMessageAsync($"Client Socket Error. The message was {message.Message}");
        }
        else if (message.Category == EnumSocketCategory.Server)
        {
            await _message!.ShowMessageAsync($"Server Socket Error. The message was {message.Message}");
        }
        else
        {
            _error!.ShowSystemError("No Category Found For Socket Error");
        }
    }
    async Task IHandleAsync<DisconnectEventModel>.HandleAsync(DisconnectEventModel message)
    {

        await _message!.ShowMessageAsync("Disconnected.  May have to refresh which starts all over again");
    }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
    {
        Unsubscribe();
    }
}