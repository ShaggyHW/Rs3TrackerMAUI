using IniParser;
using IniParser.Model;
using Newtonsoft.Json;
using Rs3TrackerMAUI.ContentPages.Helper;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using static Rs3TrackerMAUI.Classes.DisplayClasses;

namespace Rs3TrackerMAUI.ContentPages;

public partial class KeybindConfigurations : ContentPage {
    string cacheDir = "";
    string mainDir = "";
    public ObservableCollection<KeybindClass> keybindingList = new ObservableCollection<KeybindClass>();
    public List<BarKeybindClass> keybindingBarList = new List<BarKeybindClass>();
    public List<Ability> abilities = new List<Ability>();
    public static Ability chosenAbility = new Ability();
    public KeybindConfigurations() {
#if WINDOWS
        cacheDir = Microsoft.Maui.Storage.FileSystem.AppDataDirectory;
#endif
#if MACCATALYST
        cacheDir = Microsoft.Maui.Storage.FileSystem.AppDataDirectory;
#endif
        InitializeComponent();
     
  
        Loaded += KeybindConfigurations_Loaded;
    }

    HttpListener listener = new HttpListener();
    CancellationTokenSource tokenSource2 = new CancellationTokenSource();
    Task ListenerTask;
    string IP = "";
    string PORT = "";
    private void KeybindConfigurations_Loaded(object sender, EventArgs e) {
        SetMainWindowStartSize(670, 540);
        if (File.Exists(Path.Combine(cacheDir, "Configuration.ini"))) {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(Path.Combine(cacheDir, "Configuration.ini"));
            mainDir = data["DATA"]["FOLDER"];
            IP = data["DATA"]["IP"];
            PORT = data["DATA"]["PORT"];
        }
        if (!string.IsNullOrEmpty(mainDir)) {
            if (!File.Exists(Path.Combine(mainDir, "mongoAbilities.json"))) {
                var file = File.Create(Path.Combine(mainDir, "mongoAbilities.json"));
                file.Close();
            }
            if (File.Exists(Path.Combine(mainDir, "Bars.json"))) {
                var bars = JsonConvert.DeserializeObject<List<BarClass>>(File.ReadAllText(Path.Combine(mainDir, "Bars.json")));
                bars.Add(new BarClass() { name = "ALL" });
                cmbBar.ItemsSource = bars;
            }
            if (File.Exists(Path.Combine(mainDir, "keybinds.json"))) {
                keybindingList = JsonConvert.DeserializeObject<ObservableCollection<KeybindClass>>(File.ReadAllText(Path.Combine(mainDir, "keybinds.json")));
                if (keybindingList != null) {
                    var keybinds = keybindingList.OrderBy(i => i.bar.name).ToList();
                    this.BindingContext = keybinds;
                    dgSettings.ItemsSource = keybinds;
                }
            }

            //if (File.Exists(".\\barkeybinds.json")) {
            //    keybindingBarList = JsonConvert.DeserializeObject<List<BarKeybindClass>>(File.ReadAllText(".\\barkeybinds.json"));
            //    if (keybindingBarList != null)
            //        foreach (var barkey in keybindingBarList)
            //            dgSettingsBars.Items.Add(barkey);
            //}

        }
        CancellationToken ct = tokenSource2.Token;
        ListenerTask = Task.Factory.StartNew(() => StartListener(ct), tokenSource2.Token);
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
    private void StartListener(CancellationToken ct) {
        listener.Prefixes.Add($"http://{IP}:{PORT}/");

        listener.Start();
        while (true) {
            try {

                HttpListenerContext ctx = listener.GetContext();
                using (HttpListenerResponse resp = ctx.Response) {
                    string endpoint = ctx.Request.Url.LocalPath;
                    using (var reader = new StreamReader(ctx.Request.InputStream,
                                                                ctx.Request.ContentEncoding)) {
                        string text = reader.ReadToEnd();
                        var key = JsonConvert.DeserializeObject<ResquestInput>(text);
                        if (!key.keycode.Equals(42) && !key.keycode.Equals(29) && !key.keycode.Equals(3675) && !key.keycode.Equals(56)) {
                            MainThread.InvokeOnMainThreadAsync(() => HookKeyDown(key));
                        }
                    }
                    string data = "OK";
                    byte[] buffer = Encoding.UTF8.GetBytes(data);
                    resp.ContentLength64 = buffer.Length;

                    using (Stream ros = resp.OutputStream)
                        ros.Write(buffer, 0, buffer.Length);

                }

            } catch (Exception ex) { }
            try {
                if (ct.IsCancellationRequested) {
                    // Clean up here, then...
                    ct.ThrowIfCancellationRequested();
                }
            } catch (OperationCanceledException ex) {
                tokenSource2.Dispose();
                return;
            }
        }
    }

    private void HookKeyDown(ResquestInput e) {
        string modifier = "";
        if (e.keycode.ToString().ToLower().Equals("none")) {
            return;
        }
        if (e.altKey)
            modifier = "ALT+";
        else if (e.ctrlKey)
            modifier = "CTRL+";
        else if (e.shiftKey)
            modifier = "SHIFT+";
        else if (e.metaKey)//Windows Key
            modifier = "WIN+";

        MainThread.InvokeOnMainThreadAsync(() => SelectedKey.Text = modifier + KeybindConverter.KeyConversion(e.keycode));
    }

    private void btnAddKey_Clicked(object sender, EventArgs e) {
        try {
            if (cmbBar.SelectedIndex.Equals(-1)) {
                DisplayAlert("ERROR", "Please Select a bar!", "OK");
                return;
            }

            string key = "";
            string modifier = "";
            string bar = (cmbBar.SelectedItem as BarClass).name;
            if (SelectedKey.Text.Contains("+")) {
                modifier = SelectedKey.Text.Split('+')[0];
                key = SelectedKey.Text.Split('+')[1];
            } else {
                key = SelectedKey.Text;
            }

            keybindingList.Add(new KeybindClass() {
                key = key,
                modifier = modifier,
                ability = chosenAbility,
                bar = new BarClass() {
                    name = bar
                }
            });
            dgSettings.ItemsSource = keybindingList;
        } catch (Exception ex) {

        }
    }

    private void btnSaveAbilityKeybind_Clicked(object sender, EventArgs e) {
        string json = JsonConvert.SerializeObject(keybindingList, Formatting.Indented);
        if (File.Exists(Path.Combine(mainDir, "keybinds.json")))
            File.Delete(Path.Combine(mainDir, "keybinds.json"));

        var stream = File.Create(Path.Combine(mainDir, "keybinds.json"));
        stream.Close();
        File.WriteAllText(Path.Combine(mainDir, "keybinds.json"), json);
        DisplayAlert("Saved", "keybinds have been saved", "OK");
    }

    private void btnDeleteAbilityKeybind_Clicked(object sender, EventArgs e) {
        try {
            if (dgSettings.SelectedItem != null) {
                var abilSelected = (KeybindClass)dgSettings.SelectedItem;
                keybindingList.Remove(abilSelected);
                dgSettings.ItemsSource = keybindingList;
            }
        } catch (Exception ex) {

        }
    }

    private void btnClose_Clicked(object sender, EventArgs e) {
        MainPage.CloseKeybindsConfigMenu();
    }
    public void OnClose() {
        tokenSource2.Cancel();
        listener.Stop();
        ListenerTask.Wait();
        ListenerTask.Dispose();
        tokenSource2 = null;
    }

    static Window abilityWindow = null;
    private void btnPickAbility_Clicked(object sender, EventArgs e) {

        abilityWindow = new Window {
            Page = new AbilityPicker { },
            Title = "Bars",
            Width = 670,
            Height = 520
        };
        abilityWindow.Destroying += AbilityWindow_Destroying;
        Application.Current.OpenWindow(abilityWindow);
    }

    private void AbilityWindow_Destroying(object sender, EventArgs e) {
        var imageSource = ImageSource.FromFile(chosenAbility.img);
        displayImg1.Source = imageSource;
    }

    public static void CloseabilityWindowMenu() {
        var x = abilityWindow.Page as AbilityPicker;
        chosenAbility = x.PickedAbilty;
        //Application.Current.MainPage.Dispatcher.Dispatch(() => progressBarText.Text = String.Format("Downloading {0}%", e.ProgressPercentage));
        Application.Current?.CloseWindow(abilityWindow);
        abilityWindow = null;
    }
}