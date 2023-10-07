using CommunityToolkit.Maui.Storage;
using System.Threading;
using IniParser;
using IniParser.Model;

namespace Rs3TrackerMAUI.ContentPages;

public partial class Settings : ContentPage {
#if WINDOWS
     string mainDir = Microsoft.Maui.Storage.FileSystem.AppDataDirectory;
#endif
#if MACCATALYST
    string mainDir = Microsoft.Maui.Storage.FileSystem.AppDataDirectory;
#endif
    public Settings() {
        InitializeComponent();
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