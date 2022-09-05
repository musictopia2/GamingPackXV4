﻿global using BasicGameFrameworkLibrary.Core.GeneratorHelpers;
global using CommonBasicLibraries.CollectionClasses;
global using BasicGameFrameworkLibrary.Core.BasicDrawables.Dictionary;
global using BasicGameFrameworkLibrary.Core.BasicDrawables.Interfaces;
global using MessengingHelpers;
global using BasicGameFrameworkLibrary.Core.BasicGameDataClasses;
global using BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicPlayerClasses;
global using BasicGameFrameworkLibrary.Core.MultiplayerClasses.SavedGameClasses;
global using CommonBasicLibraries.BasicDataSettingsAndProcesses;
global using BasicGameFrameworkLibrary.Core.Attributes;
global using BasicGameFrameworkLibrary.Core.CommonInterfaces;
global using BasicGameFrameworkLibrary.Core.MiscProcesses;
global using BasicGameFrameworkLibrary.Core.CommandClasses;
global using BasicGameFrameworkLibrary.Core.DIContainers;
global using BasicGameFrameworkLibrary.Core.NetworkingClasses.Interfaces;
global using static CommonBasicLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
global using js = CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.SystemTextJsonStrings; //this is very common too
global using CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
global using System.Collections;
global using cs = CommonBasicLibraries.BasicDataSettingsAndProcesses.SColorString;
global using BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceMessages;
global using BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfacesForHelpers;
global using BasicGameFrameworkLibrary.Core.MultiplayerClasses.Extensions;
global using BasicGameFrameworkLibrary.Core.Dice;
global using CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.MapHelpers;
global using MVVMFramework.EventArgClasses;
global using CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
global using static BasicGameFrameworkLibrary.Core.DIContainers.Helpers;
global using System.Drawing;
global using BasicGameFrameworkLibrary.Core.BasicDrawables.BasicClasses;
global using nm = BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceMessages;
global using CommonBasicLibraries.BasicUIProcesses;
global using BasicGameFrameworkLibrary.Core.TestUtilities;
global using BasicGameFrameworkLibrary.Core.MultiplayerClasses.GameContainers;
global using BasicGameFrameworkLibrary.Core.MultiplayerClasses.MainGameInterfaces;
global using BasicGameFrameworkLibrary.Core.MultiplayerClasses.MiscHelpers;
global using BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceModels;
global using BasicGameFrameworkLibrary.Core.GamePieceModels;
global using BasicGameFrameworkLibrary.Core.Extensions;
global using BasicGameFrameworkLibrary.Core.RegularDeckOfCards;
global using BasicGameFrameworkLibrary.Core.MultiplePilesObservable;
global using BasicGameFrameworkLibrary.Core.BasicEventModels;
global using BasicGameFrameworkLibrary.Core.DrawableListsObservable;
global using MVVMFramework.ViewModels;
global using BasicGameFrameworkLibrary.Core.NetworkingClasses.Data;
global using BasicGameFrameworkLibrary.Core.Dominos;
global using BasicGameFrameworkLibrary.Core.NetworkingClasses.Extensions;
global using BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Containers;
global using BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicGameClasses;
global using BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Data;
global using BasicGameFrameworkLibrary.Core.SpecializedGameTypes.TrickClasses;
global using BasicGameFrameworkLibrary.Core.MultiplayerClasses.EventModels;
global using BasicGameFrameworkLibrary.Core.ViewModelInterfaces;
global using BasicGameFrameworkLibrary.Core.MultiplayerClasses.LoadingClasses;
global using MVVMFramework.EventModels;
global using BasicGameFrameworkLibrary.Core.ChooserClasses;
global using BasicGameFrameworkLibrary.Core.ViewModels;
global using BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Logic;
global using BasicGameFrameworkLibrary.Core.MultiplayerClasses.MainViewModels;
global using BasicGameFrameworkLibrary.Core.SolitaireClasses.Cards;
global using BasicGameFrameworkLibrary.Core.SolitaireClasses.PileInterfaces;
global using BasicGameFrameworkLibrary.Core.SolitaireClasses.BasicVMInterfaces;
global using BasicGameFrameworkLibrary.Core.SolitaireClasses.DataClasses;
global using BasicGameFrameworkLibrary.Core.SolitaireClasses.MiscClasses;
global using BasicGameFrameworkLibrary.Core.SolitaireClasses.PileObservable;
global using BasicGameFrameworkLibrary.Core.SolitaireClasses.GraphicsObservable;
global using Microsoft.JSInterop;
global using BasicGameFrameworkLibrary.Blazor.StartupClasses;
global using Microsoft.AspNetCore.Components;
global using BasicBlazorLibrary.Components.BaseClasses;
global using BasicBlazorLibrary.Helpers;
global using aa = BasicGameFrameworkLibrary.Core.DIContainers.Helpers;
global using BasicGameFrameworkLibrary.Blazor.Helpers; //try here.
global using BasicGameFrameworkLibrary.Blazor.Extensions;
global using BasicBlazorLibrary.Components.MediaQueries.ParentClasses;
global using SvgHelper.Blazor.Logic.Classes.SubClasses;
global using SvgHelper.Blazor.Logic.Classes.Interfaces;
global using Microsoft.AspNetCore.Components.Rendering;
global using SvgHelper.Blazor.Logic;
global using BasicGameFrameworkLibrary.Core.ColorCards;
global using BasicGameFrameworkLibrary.Blazor.GameGraphics.MiscClasses;
global using BasicGameFrameworkLibrary.Blazor.GameGraphics.Base;
global using System;
global using BasicGameFrameworkLibrary.Blazor.GameGraphics.GamePieces;
global using System.Threading.Tasks;
global using System.Collections.Generic;
global using System.Linq;
global using BasicGameFrameworkLibrary.Core.SpecializedGameTypes.CheckersChessHelpers;
global using BasicGameFrameworkLibrary.Core.GameBoardCollections;
global using BasicGameFrameworkLibrary.Core.AnimationClasses;
global using BasicGameFrameworkLibrary.Core.ScoreBoardClassesCP;
global using BasicGameFrameworkLibrary.Core.SolitaireClasses.ClockClasses;
global using BasicGameFrameworkLibrary.Core.SolitaireClasses.TriangleClasses;
global using BasicGameFrameworkLibrary.Blazor.BasicControls.ChoicePickers;
global using BasicGameFrameworkLibrary.Core.SpecializedGameTypes.RummyClasses;
global using BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.ViewModels;
global using ab = BasicBlazorLibrary.Components.CssGrids.Helpers;
global using BasicGameFrameworkLibrary.Blazor.BasicControls.GameBoards;
global using BasicGameFrameworkLibrary.Core.StandardImplementations.AutoResumeNativeFileAccessClasses; //looks like became common enough because transfer to desktop.

//global using static BasicGameFrameworkLibrary.Core.MiscProcesses.GlobalDelegates;