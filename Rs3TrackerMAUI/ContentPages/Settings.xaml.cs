using CommunityToolkit.Maui.Storage;
using System.Threading;
using IniParser;
using IniParser.Model;

namespace Rs3TrackerMAUI.ContentPages;

public partial class Settings : ContentPage {
    string mainDir = "";

    public Settings() {
        InitializeComponent();


#if WINDOWS
      mainDir = ".\\Configuration\\";
#endif
#if MACCATALYST
        mainDir = ".\\Configuration\\";
#endif
        mainDir = Microsoft.Maui.Storage.FileSystem.AppDataDirectory;
        Loaded += Settings_Loaded;
    }

    private void Settings_Loaded(object sender, EventArgs e) {
        SetMainWindowStartSize(450, 270);
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
    }
    private void btnSave_Clicked(object sender, EventArgs e) {
        try {
            var parser = new FileIniDataParser();
            IniData data = new IniData();
            data["DATA"]["FOLDER"] = txtDataFolder.Text;
            data["DATA"]["IP"] = txtIP.Text;
            data["DATA"]["PORT"] = txtPort.Text;
            parser.WriteFile(Path.Combine(mainDir, "Configuration.ini"), data);
            DisplayAlert("SAVED", "Data has been saved", "OK");
        } catch (Exception ex) {
            DisplayAlert("OK", ex.Message, "OK");
        }
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