using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Dispatching;
using Rs3TrackerMAUI.ContentPages;
using System.Security.Principal;

namespace Rs3TrackerMAUI;

public partial class MainPage : ContentPage {
#if WINDOWS
     string  mainDir = Microsoft.Maui.Storage.FileSystem.CacheDirectory;
#endif
#if MACCATALYST
    string mainDir = AppDomain.CurrentDomain.BaseDirectory.Replace("Rs3TrackerMAUI.app/Contents/MonoBundle", "");
#endif

    public MainPage() {
    
        InitializeComponent();
    
        Loaded += MainPage_Loaded;
    }
    private void SetMainWindowStartSize(int width, int height) {
#if MACCATALYST
        //Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(
        //    nameof(IWindow), (handler, view) => {
        //        var size = new CoreGraphics.CGSize(width, height);
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
        var DisplayInfo = DeviceDisplay.MainDisplayInfo;

#if WINDOWS
        Microsoft.UI.Xaml.Window window = (Microsoft.UI.Xaml.Window)App.Current.Windows.First<Window>().Handler.PlatformView;
        IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
        Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
        Microsoft.UI.Windowing.AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
        appWindow.Resize(new Windows.Graphics.SizeInt32(width,height));      
#endif

    }

    private void MainPage_Loaded(object sender, EventArgs e) {
        if (!File.Exists(mainDir + "mongoAbilities.json")) {
            var file = File.Create(mainDir + "mongoAbilities.json");
            file.Close();
        }
        SetMainWindowStartSize(550, 320);
    }

    private void btnAbilityConfig_Clicked(object sender, EventArgs e) {
        secondWindow = new Window {
            Page = new AbilityConfigurations {

            },
            Title = "Abilities",
            Width = 850,
            Height = 500

        };

        Application.Current.OpenWindow(secondWindow);
    }

    private void btnBars_Clicked(object sender, EventArgs e) {
        DisplayAlert("OK", mainDir, "ok");
    }

    private void btnSettings_Clicked(object sender, EventArgs e) {

    }

    private void btnServer_Clicked(object sender, EventArgs e) {

    }

    private void btnClose_Clicked(object sender, EventArgs e) {
        Application.Current.Quit();
    }

    Window secondWindow = null;
    private void btnStartTracker_Clicked(object sender, EventArgs e) {
        if (secondWindow != null) {
            //displayX = display.Left;
            //displayY = display.Top;
            //DisplayHeight = display.Height;
            //DisplayWidth = display.Width;
            //Application.CloseWindow(secondWindow);
            var x = secondWindow.Page as Display;
            x.OnClose();
            Application.Current?.CloseWindow(secondWindow);
            secondWindow = null;
            btnStartTracker.Text = "Start Tracker";

        } else {


            if (!File.Exists(mainDir + "keybinds.json")) {
                DisplayAlert("Alert", "Missing Keybinds", "OK");
                //MessageBox.Show("Missing Keybinds");
                return;
            }
            if (!File.Exists(mainDir + "barkeybinds.json")) {
                DisplayAlert("Alert", "Missing Bar Keybinds", "OK");

                return;
            }
            //if (cmbMode.SelectedIndex.Equals(-1))
            //    return;

            btnStartTracker.Text = "Close Tracker";
            //display = new Display(cmbMode.Text.ToLower(), TrackCD.IsChecked.Value, onTop.IsChecked.Value, CanResize.IsChecked.Value, ServerCheck.IsChecked.Value);
            //display.Top = displayY;
            //display.Left = displayX;
            //display.Height = DisplayHeight;
            //display.Width = DisplayWidth;
            //display.Show();
            secondWindow = new Window {
                Page = new Display {

                },
                Title = "Display",
                Width = 800,
                Height = 80

            };

            Application.Current.OpenWindow(secondWindow);
        }
    }

    private void CanResize_CheckedChanged(object sender, CheckedChangedEventArgs e) {

    }

    private void cmbMode_SelectedIndexChanged(object sender, EventArgs e) {

    }
}

