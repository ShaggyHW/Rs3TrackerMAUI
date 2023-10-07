using IniParser;
using IniParser.Model;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Dispatching;
using Newtonsoft.Json;
using Rs3TrackerMAUI.ContentPages;
using System.Security.Principal;
using static Rs3TrackerMAUI.Classes.DisplayClasses;

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
        SetMainWindowStartSize(650, 320);
        InitializeComponent();
        Loaded += MainPage_Loaded;
    }
    private void SetMainWindowStartSize(int width, int height) {
#if MACCATALYST
        Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(
            nameof(IWindow), (handler, view) => {
                var size = new CoreGraphics.CGSize(width, height);
                handler.PlatformView.WindowScene.SizeRestrictions.MinimumSize = size;
                handler.PlatformView.WindowScene.SizeRestrictions.MaximumSize = size;
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
            IniData data = parser.ReadFile(Path.Combine(cacheDir, "Configuration.ini"));
            mainDir = data["DATA"]["FOLDER"];
        }
        if (!string.IsNullOrEmpty(mainDir)) {
            if (!File.Exists(Path.Combine(mainDir, "mongoAbilities.json"))) {
                var file = File.Create(Path.Combine(mainDir, "mongoAbilities.json"));
                file.Close();
            }
            if (File.Exists(Path.Combine(mainDir, "Bars.json"))) {
                var bars = JsonConvert.DeserializeObject<List<BarClass>>(File.ReadAllText(Path.Combine(mainDir, "Bars.json")));
                cmbMode.ItemsSource = bars;
            }
        }
    }
    private void btnClose_Clicked(object sender, EventArgs e) {
        Application.Current.Quit();
    }

    static Window ablitiesWindow = null;
    private void btnAbilityConfig_Clicked(object sender, EventArgs e) {
        ablitiesWindow = new Window {
            Page = new AbilityConfigurations { },
            Title = "Abilities",
            Width = 850,
            Height = 500
        };

        Application.Current.OpenWindow(ablitiesWindow);
    }
    public static void CloseAbilityConfigMenu() {
        Application.Current?.CloseWindow(ablitiesWindow);

    }
    static Window barsWindow = null;
    private void btnBars_Clicked(object sender, EventArgs e) {
        barsWindow = new Window {
            Page = new BarConfigurations { },
            Title = "Bars",
            Width = 760,
            Height =270
        };
        Application.Current.OpenWindow(barsWindow);
    }
    public static void CloseBarsConfigMenu() {
        Application.Current?.CloseWindow(barsWindow);
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
            if (!File.Exists(Path.Combine(mainDir, "keybinds.json"))) {
                DisplayAlert("Alert", "Missing Keybinds", "OK");
                //MessageBox.Show("Missing Keybinds");
                return;
            }
            if (!File.Exists(Path.Combine(mainDir, "barkeybinds.json"))) {
                DisplayAlert("Alert", "Missing Bar Keybinds", "OK");
                return;
            }
            if (cmbMode.SelectedIndex.Equals(-1))
                return;

            btnStartTracker.Text = "Close Tracker";
            //display = new Display(cmbMode.Text.ToLower(), TrackCD.IsChecked.Value, onTop.IsChecked.Value, CanResize.IsChecked.Value, ServerCheck.IsChecked.Value);
            //display.Top = displayY;
            //display.Left = displayX;
            //display.Height = DisplayHeight;
            //display.Width = DisplayWidth;
            //display.Show();
            displayWindow = new Window {
                Page = new Display(cmbMode.SelectedItem.ToString()) {

                },
                Title = "Display",
                Width = 800,
                Height = 80
            };
            Application.Current.OpenWindow(displayWindow);
        }
    }

    static Window settingsWindow = null;
    private void btnSettings_Clicked(object sender, EventArgs e) {
        settingsWindow = new Window {
            Page = new Settings { },
            Title = "Settings",
            Width = 450,
            Height = 250
        };
        Application.Current.OpenWindow(settingsWindow);
    }

    public static void CloseSettingsMenu() {
        Application.Current?.CloseWindow(settingsWindow);
    }

    private void btnKeybinds_Clicked(object sender, EventArgs e) {

    }
}

