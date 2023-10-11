using CommunityToolkit.Maui.Storage;
using System.Threading;
using IniParser;
using IniParser.Model;

namespace Rs3TrackerMAUI.ContentPages;

public partial class Settings : ContentPage {
    string mainDir = "";

    public Settings() {
        SetMainWindowStartSize(450, 250);
        InitializeComponent();
#if WINDOWS
      mainDir = Microsoft.Maui.Storage.FileSystem.AppDataDirectory;
#endif
#if MACCATALYST
        mainDir = Microsoft.Maui.Storage.FileSystem.AppDataDirectory;
#endif
        Loaded += Settings_Loaded;
    }

    private void Settings_Loaded(object sender, EventArgs e) {
        if (File.Exists(Path.Combine(mainDir, "Configuration.ini"))) {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(Path.Combine(mainDir, "Configuration.ini"));
            txtDataFolder.Text = data["DATA"]["FOLDER"];
            txtIP.Text = data["DATA"]["IP"];
            txtPort.Text = data["DATA"]["PORT"];
        }
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
    private void btnSave_Clicked(object sender, EventArgs e) {
        var parser = new FileIniDataParser();
        IniData data = new IniData();
        data["DATA"]["FOLDER"] = txtDataFolder.Text;
        data["DATA"]["IP"] = txtIP.Text;
        data["DATA"]["PORT"] = txtPort.Text;
        parser.WriteFile(Path.Combine(mainDir, "Configuration.ini"), data);
        DisplayAlert("SAVED", "Data has been saved", "OK");
    }

    private void btnClose_Clicked(object sender, EventArgs e) {
        MainPage.CloseSettingsMenu();
    }

    private async void btnDataFolder_Clicked(object sender, EventArgs e) {
        var result = await FolderPicker.Default.PickAsync(default);
        if (result.Folder != null) {
            txtDataFolder.Text = result.Folder.Path;
            DisplayAlert("WARNING", "Restart the aplication for the folder changes to take effect!", "OK");
        }
    }
}