using IniParser.Model;
using IniParser;
using System.Net;
using System.Text;
using static Rs3TrackerMAUI.Classes.DisplayClasses;
using Newtonsoft.Json;
using Rs3TrackerMAUI.ContentPages.Helper;

namespace Rs3TrackerMAUI.ContentPages;

public partial class BarKeybindsConfigurations : ContentPage {
    private List<BarKeybindClass> keybindingBarList = new List<BarKeybindClass>();
    string cacheDir = "";
    string mainDir = "";
    HttpListener listener = new HttpListener();
    CancellationTokenSource tokenSource2 = new CancellationTokenSource();
    Task ListenerTask;
    string IP = "";
    string PORT = "";

    public BarKeybindsConfigurations() {
#if WINDOWS
        cacheDir = Microsoft.Maui.Storage.FileSystem.AppDataDirectory;
#endif
#if MACCATALYST
        cacheDir = Microsoft.Maui.Storage.FileSystem.AppDataDirectory;
#endif
        InitializeComponent();
        Loaded += BarKeybindsConfigurations_Loaded;
    }

    private void BarKeybindsConfigurations_Loaded(object sender, EventArgs e) {
        if (File.Exists(Path.Combine(cacheDir, "Configuration.ini"))) {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(Path.Combine(cacheDir, "Configuration.ini"));
            mainDir = data["DATA"]["FOLDER"];
            IP = data["DATA"]["IP"];
            PORT = data["DATA"]["PORT"];
        }
        if (!string.IsNullOrEmpty(mainDir)) {
            if (File.Exists(Path.Combine(mainDir, "Bars.json"))) {
                var bars = JsonConvert.DeserializeObject<List<BarClass>>(File.ReadAllText(Path.Combine(mainDir, "Bars.json")));
                cmbBar.ItemsSource = bars;
            }
            if (File.Exists(Path.Combine(mainDir, "barkeybinds.json"))) {
                keybindingBarList = JsonConvert.DeserializeObject<List<BarKeybindClass>>(File.ReadAllText(Path.Combine(mainDir, "barkeybinds.json")));
                if (keybindingBarList != null) {
                    this.BindingContext = keybindingBarList;
                    dgSettings.ItemsSource = keybindingBarList;
                }
            }
        }
        CancellationToken ct = tokenSource2.Token;
        ListenerTask = Task.Factory.StartNew(() => StartListener(ct), tokenSource2.Token);
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

        SelectedKey.Text = modifier + KeybindConverter.KeyConversion(e.keycode);
    }

    private void btnSaveBarKeybind_Clicked(object sender, EventArgs e) {
        string json = JsonConvert.SerializeObject(keybindingBarList, Formatting.Indented);
        if (File.Exists(Path.Combine(mainDir, "barkeybinds.json")))
            File.Delete(Path.Combine(mainDir, "barkeybinds.json"));

        var stream = File.Create(Path.Combine(mainDir, "barkeybinds.json"));
        stream.Close();
        File.WriteAllText(Path.Combine(mainDir, "barkeybinds.json"), json);
        DisplayAlert("Saved", "Bar keybinds have been saved", "OK");
    }

    private void btnDeleteBarKeybind_Clicked(object sender, EventArgs e) {
        try {
            if (dgSettings.SelectedItem != null) {
                var abilSelected = (BarKeybindClass)dgSettings.SelectedItem;
                keybindingBarList.Remove(abilSelected);
                dgSettings.ItemsSource = keybindingBarList;
            }
        } catch (Exception ex) {

        }
    }

    private void btnClose_Clicked(object sender, EventArgs e) {
        MainPage.CloseBarKeybindsConfigMenu();
    }
    public void OnClose() {
        tokenSource2.Cancel();
        listener.Stop();
        ListenerTask.Wait();
        ListenerTask.Dispose();
        tokenSource2 = null;
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

            keybindingBarList.Add(new BarKeybindClass() {
                key = key,
                modifier = modifier,
                bar = new BarClass() {
                    name = bar
                }
            });
            dgSettings.ItemsSource = keybindingBarList;
        } catch (Exception ex) {

        }
    }
}