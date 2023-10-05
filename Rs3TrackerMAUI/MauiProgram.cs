using Microsoft.Maui.LifecycleEvents;


namespace Rs3TrackerMAUI;

public static class MauiProgram {
    public static MauiApp CreateMauiApp() {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
#if MACCATALYST
        //Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(
        //    nameof(IWindow), (handler, view) => {
        //        var size = new CoreGraphics.CGSize(550, 320);
        //        handler.PlatformView.WindowScene.SizeRestrictions.MinimumSize = size;
        //        handler.PlatformView.WindowScene.SizeRestrictions.MaximumSize = size;
        //        Task.Run(() => {
        //            Thread.Sleep(1000);
        //            MainThread.BeginInvokeOnMainThread(() => {
        //                handler.PlatformView.WindowScene.SizeRestrictions.MinimumSize = new CoreGraphics.CGSize(100, 100);
        //                handler.PlatformView.WindowScene.SizeRestrictions.MaximumSize = new CoreGraphics.CGSize(5000, 5000);
        //            });
        //        });

        //    });
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
                            int width = 550;
                            int height = 320;
                            //var DisplayInfo = DeviceDisplay.MainDisplayInfo;
                            //appWindow.MoveAndResize(new Windows.Graphics.RectInt32((int)DisplayInfo.Width / 2 - width / 2, (int)DisplayInfo.Height / 2 - height / 2, width, height));
                            //overlappedPresenter.Maximize();                                                    
                            break;
                    }
                });
            });
        });
#endif
#if WINDOWS
        //  builder.ConfigureLifecycleEvents(events => {
        //    events.AddWindows(wndLifeCycleBuilder => {
        //        wndLifeCycleBuilder.OnWindowCreated(window => {
        //            IntPtr nativeWindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
        //            WindowId win32WindowsId = Win32Interop.GetWindowIdFromWindow(nativeWindowHandle);
        //            AppWindow winuiAppWindow = AppWindow.GetFromWindowId(win32WindowsId);
        //            if (winuiAppWindow.Presenter is OverlappedPresenter p) {
        //                //p.Maximize();
        //                ////p.IsAlwaysOnTop=true;
        //                //p.IsResizable = false;
        //                //p.IsMaximizable = false;
        //                //p.IsMinimizable = false;  const int width = 1920;                        
        //                int width = 550;
        //                int height = 320;
        //                winuiAppWindow.MoveAndResize(new RectInt32(1920 / 2 - width / 2, 1080 / 2 - height / 2, width, height));
        //                p.IsResizable = false;
        //            } else {
        //                const int width = 1920;
        //                const int height = 1080;
        //                winuiAppWindow.MoveAndResize(new RectInt32(1920 / 2 - width / 2, 1080 / 2 - height / 2, width, height));
        //            }
        //        });
        //    });
        //});
#endif



        return builder.Build();
    }
}
