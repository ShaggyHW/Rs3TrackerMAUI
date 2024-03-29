using IniParser.Model;
using IniParser;
using static Rs3TrackerMAUI.Classes.DisplayClasses;
using Newtonsoft.Json;

namespace Rs3TrackerMAUI.ContentPages.Helper;

public partial class AbilityPicker : ContentPage {
    private List<Ability> abilities = new List<Ability>();
    string mainDir = "";
    string cacheDir = "";
    public Ability PickedAbilty = new Ability();

    public AbilityPicker() {
        InitializeComponent();
       
    
#if WINDOWS
        cacheDir = ".\\Configuration\\";
#endif
#if MACCATALYST
        cacheDir = ".\\Configuration\\";
#endif
        cacheDir = Microsoft.Maui.Storage.FileSystem.AppDataDirectory;
        if (File.Exists(Path.Combine(cacheDir, "Configuration.ini"))) {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(Path.Combine(cacheDir, "Configuration.ini"));
            mainDir = data["DATA"]["FOLDER"];
        }

        Loaded += AbilityPicker_Loaded;
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
    private void AbilityPicker_Loaded(object sender, EventArgs e) {
        SetMainWindowStartSize(670, 540);
        if (File.Exists(Path.Combine(mainDir, "mongoAbilities.json"))) {
            abilities = JsonConvert.DeserializeObject<List<Ability>>(File.ReadAllText(Path.Combine(mainDir, "mongoAbilities.json")));
            if (abilities != null) {
                //var keybinds = abilities.OrderBy(i => i.name).ToList();
                //foreach (var key in keybinds) {
                //    //dgSettings.Items.Add(key);
                //}
                abilities = abilities.OrderBy(i => i.name).ToList();
                dgSettings.ItemsSource = abilities;
            }
        }
    }

    private void btnPickAbilty_Clicked(object sender, EventArgs e) {
        PickedAbilty = dgSettings.SelectedItem as Ability;    
        KeybindConfigurations.CloseabilityWindowMenu();
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e) {
        if (string.IsNullOrEmpty(e.NewTextValue)) {
            abilities = JsonConvert.DeserializeObject<List<Ability>>(File.ReadAllText(Path.Combine(mainDir, "mongoAbilities.json")));
            abilities = abilities.OrderBy(i => i.name).ToList();
            dgSettings.ItemsSource = null;
            dgSettings.ItemsSource = abilities;
        } else {
            abilities = JsonConvert.DeserializeObject<List<Ability>>(File.ReadAllText(Path.Combine(mainDir, "mongoAbilities.json")));
            abilities = abilities.OrderBy(i => i.name).Where(i => i.name.ToLower().Contains(e.NewTextValue.ToLower())).Select(i => i).ToList();
            dgSettings.ItemsSource = null;
            dgSettings.ItemsSource = abilities;
        }
    }
}