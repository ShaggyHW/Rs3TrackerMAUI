using IniParser;
using IniParser.Model;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Dispatching;
using Rs3TrackerMAUI.ContentPages;
using System.Security.Principal;

namespace Rs3TrackerMAUI;

public partial class MainPage : ContentPage {
#if WINDOWS
     string cacheDir = Microsoft.Maui.Storage.FileSystem.AppDataDirectory;
#endif
#if MACCATALYST
    string cacheDir = Microsoft.Maui.Storage.FileSystem.AppDataDirectory;
#endif
    string mainDir = "";
    public MainPage() {
        SetMainWindowStartSize(550, 320);
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

#if WINDOWS
        Microsoft.UI.Xaml.Window window = (Microsoft.UI.Xaml.Window)App.Current.Windows.First<Window>().Handler.PlatformView;
        IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
        Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
        Microsoft.UI.Windowing.AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
        appWindow.Resize(new Windows.Graphics.SizeInt32(width,height));      
#endif

    }

    private void MainPage_Loaded(object sender, EventArgs e) {
        if (File.Exists(Path.Combine(cacheDir, "Configuration.ini"))) {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile("Configuration.ini");
            mainDir = data["DATA"]["FOLDER"];
        }

        if (!string.IsNullOrEmpty(mainDir)) {
            if (!File.Exists(Path.Combine(mainDir, "mongoAbilities.json"))) {
                var file = File.Create(Path.Combine(mainDir, "mongoAbilities.json"));
                file.Close();
            }
        }
    }
    Window ablitiesWindow = null;
    private void btnAbilityConfig_Clicked(object sender, EventArgs e) {
        ablitiesWindow = new Window {
            Page = new AbilityConfigurations {

            },
            Title = "Abilities",
            Width = 850,
            Height = 500
        };

        Application.Current.OpenWindow(ablitiesWindow);
    }

    private void btnBars_Clicked(object sender, EventArgs e) {
        DisplayAlert("OK", Microsoft.Maui.Storage.FileSystem.CacheDirectory, "ok");
    }

    private void btnServer_Clicked(object sender, EventArgs e) {

    }

    private void btnClose_Clicked(object sender, EventArgs e) {
        Application.Current.Quit();
    }

    Window displayWindow = null;
    private void btnStartTracker_Clicked(object sender, EventArgs e) {
        if (displayWindow != null) {       
            var x = displayWindow.Page as Display;
            x.OnClose();
            Application.Current?.CloseWindow(displayWindow);
            displayWindow = null;
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
            displayWindow = new Window {
                Page = new Display() {

                },
                Title = "Display",
                Width = 800,
                Height = 80
            };
            Application.Current.OpenWindow(displayWindow);
        }
    }

    private void CanResize_CheckedChanged(object sender, CheckedChangedEventArgs e) {

    }

    private void cmbMode_SelectedIndexChanged(object sender, EventArgs e) {

    }
    static Window settingsWindow = null;
    private void btnSettings_Clicked(object sender, EventArgs e) {
        settingsWindow = new Window {
            Page = new Settings {

            },
            Title = "Settings",
            Width = 850,
            Height = 500
        };
        Application.Current.OpenWindow(settingsWindow);
    }
    public static void CloseSettings() {
        Application.Current?.CloseWindow(settingsWindow);     
    }

    private void btnKeybinds_Clicked(object sender, EventArgs e) {

    }
}

