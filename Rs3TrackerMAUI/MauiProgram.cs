﻿using CommunityToolkit.Maui;
using Microsoft.Maui.LifecycleEvents;


namespace Rs3TrackerMAUI;

public static class MauiProgram {

    public static MauiApp CreateMauiApp() {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>().UseMauiCommunityToolkit()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        int width = 650;
        int height = 320;
#if MACCATALYST
        Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(
            nameof(IWindow), (handler, view) => {
                var size = new CoreGraphics.CGSize(width, height);
                handler.PlatformView.WindowScene.SizeRestrictions.MinimumSize = size;
                handler.PlatformView.WindowScene.SizeRestrictions.MaximumSize = size;
                handler.PlatformView.WindowScene.Titlebar.TitleVisibility = UIKit.UITitlebarTitleVisibility.Hidden;
                Task.Run(() => {
                    Thread.Sleep(1000);
                    MainThread.BeginInvokeOnMainThread(() => {
                        handler.PlatformView.WindowScene.SizeRestrictions.MinimumSize = new CoreGraphics.CGSize(width, height);
                        handler.PlatformView.WindowScene.SizeRestrictions.MaximumSize = new CoreGraphics.CGSize(width, height);
                    });
                });

            });
#endif

#if WINDOWS
        builder.ConfigureLifecycleEvents(events => {
            // Make sure to add "using Microsoft.Maui.LifecycleEvents;" in the top of the file 
            events.AddWindows(windowsLifecycleBuilder => {
                windowsLifecycleBuilder.OnWindowCreated(window => {
                    window.ExtendsContentIntoTitleBar = false;
                    var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                    var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
                    var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);
                    switch (appWindow.Presenter) {
                        case Microsoft.UI.Windowing.OverlappedPresenter overlappedPresenter:                          
                            overlappedPresenter.SetBorderAndTitleBar(false, false);
                            overlappedPresenter.IsResizable = false;
                          
                            //var DisplayInfo = DeviceDisplay.MainDisplayInfo;
                            //appWindow.MoveAndResize(new Windows.Graphics.RectInt32((int)DisplayInfo.Width / 2 - width / 2, (int)DisplayInfo.Height / 2 - height / 2, width, height));
                            //overlappedPresenter.Maximize();                                                    
                            break;
                    }
                });
            });
        });
#endif




        return builder.Build();
    }
}
