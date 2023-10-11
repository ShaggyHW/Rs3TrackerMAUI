using IniParser.Model;
using IniParser;
using Newtonsoft.Json;
using static Rs3TrackerMAUI.Classes.DisplayClasses;

namespace Rs3TrackerMAUI.ContentPages;

public partial class BarConfigurations : ContentPage {
    string mainDir = "";
    string cacheDir = "";


    private List<BarClass> bars = new List<BarClass>();
    public BarConfigurations() {
#if WINDOWS
        cacheDir = Microsoft.Maui.Storage.FileSystem.AppDataDirectory;
#endif
#if MACCATALYST
        cacheDir = Microsoft.Maui.Storage.FileSystem.AppDataDirectory;
#endif
        InitializeComponent();        
        if (File.Exists(Path.Combine(cacheDir, "Configuration.ini"))) {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(Path.Combine(cacheDir, "Configuration.ini"));
            mainDir = data["DATA"]["FOLDER"];
        }
        Loaded += BarConfigurations_Loaded;
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
    private void BarConfigurations_Loaded(object sender, EventArgs e) {
        SetMainWindowStartSize(760, 290);
        if (File.Exists(Path.Combine(mainDir, "Bars.json"))) {
            bars = JsonConvert.DeserializeObject<List<BarClass>>(File.ReadAllText(Path.Combine(mainDir, "Bars.json")));
            dgSettings.ItemsSource = bars;
        }
    }

    private void btnAdd_Clicked(object sender, EventArgs e) {
        if (string.IsNullOrEmpty(txtBarName.Text)) {
            DisplayAlert("WARNING", "Bar Name Empty", "OK");
            return;
        }
        bars.Add(new BarClass() { name = txtBarName.Text });
        dgSettings.ItemsSource = null;
        dgSettings.ItemsSource = bars;
    }

    private void btnSave_Clicked(object sender, EventArgs e) {
        string json = JsonConvert.SerializeObject(bars, Formatting.Indented);
        if (File.Exists(Path.Combine(mainDir, "Bars.json")))
            File.Delete(Path.Combine(mainDir, "Bars.json"));

        var stream = File.Create(Path.Combine(mainDir, "Bars.json"));
        stream.Close();
        File.WriteAllText(Path.Combine(mainDir, "Bars.json"), json);
        DisplayAlert("Saved", "Bars have been saved", "OK");
    }

    private void btnDelete_Clicked(object sender, EventArgs e) {
        BarClass bar = dgSettings.SelectedItem as BarClass;
        bars.Remove(bar);
        dgSettings.ItemsSource = null;
        dgSettings.ItemsSource = bars;
    }

    private void btnClose_Clicked(object sender, EventArgs e) {
        MainPage.CloseBarsConfigMenu();
    }
}