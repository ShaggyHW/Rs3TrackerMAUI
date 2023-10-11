using IniParser.Model;
using IniParser;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static Rs3TrackerMAUI.Classes.DisplayClasses;

namespace Rs3TrackerMAUI.ContentPages;

public partial class Display : ContentPage {
    string cacheDir = "";
    string mainDir = "";
    string IP = "";
    string PORT = "";
    List<KeybindClass> keybindClasses = new List<KeybindClass>();
    List<BarKeybindClass> keybindBarClasses = new List<BarKeybindClass>();
    int imgCounter = 0;
    public string style = "";
    public List<Keypressed> ListKeypressed = new List<Keypressed>();
    public List<Keypressed> ListPreviousKeypressed = new List<Keypressed>();
    public Stopwatch stopwatch = new Stopwatch();
    public bool control = false;
    private Keypressed previousKey = new Keypressed();
    private List<Keypressed> ListPreviousKeys = new List<Keypressed>();
    //private bool trackCD;
    private bool pause = false;

 

    public Display(string _style) {
      
        InitializeComponent();
#if WINDOWS
        cacheDir = Microsoft.Maui.Storage.FileSystem.AppDataDirectory;
#endif
#if MACCATALYST
        cacheDir = Microsoft.Maui.Storage.FileSystem.AppDataDirectory;
          SetMainWindowStartSize(800, 100);
#endif
        this.style = _style;
        Loaded += Display_Loaded;

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
    public void OnClose() {
        tokenSource2.Cancel();
        listener.Stop();
        stopwatch.Stop();
        ListenerTask.Wait();
        ListenerTask.Dispose();
        tokenSource2 = null;
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

    HttpListener listener = new HttpListener();
    CancellationTokenSource tokenSource2 = new CancellationTokenSource();
    Task ListenerTask;
    private void Display_Loaded(object sender, EventArgs e) {
        if (File.Exists(Path.Combine(cacheDir, "Configuration.ini"))) {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(Path.Combine(cacheDir, "Configuration.ini"));
            mainDir = data["DATA"]["FOLDER"];
            IP = data["DATA"]["IP"];
            PORT = data["DATA"]["PORT"];
        }


        keybindClasses = JsonConvert.DeserializeObject<List<KeybindClass>>(File.ReadAllText(Path.Combine(mainDir , "keybinds.json")));
        keybindBarClasses = JsonConvert.DeserializeObject<List<BarKeybindClass>>(File.ReadAllText(Path.Combine(mainDir , "barkeybinds.json")));
        stopwatch.Start();
        CancellationToken ct = tokenSource2.Token;
        ListenerTask = Task.Factory.StartNew(() => StartListener(ct), tokenSource2.Token);

    }

    private void HookKeyDown(ResquestInput e) {
        #region display
        if (!control) {
            control = true;
            Keypressed keypressed = new Keypressed();
            keypressed.ability = new Ability();
            string modifier = "";
            if (e.keycode.ToString().ToLower().Equals("none")) {
                control = false;
                return;
            }
            if (e.altKey)
                modifier = "ALT";
            else if (e.ctrlKey)
                modifier = "CTRL";
            else if (e.shiftKey)
                modifier = "SHIFT";
            else if (e.metaKey)//Windows Key
                modifier = "WIN";

            string keyPressedValue = Helper.KeybindConverter.KeyConversion(e.keycode);
            List<Ability> abilityList = (from r in keybindClasses
                                         where r.key.ToLower() == keyPressedValue.ToString().ToLower()
                                         where r.modifier.ToString().ToLower() == modifier.ToLower()
                                         select r.ability).ToList();

            if (abilityList.Count == 0) {
                if (keybindBarClasses != null) {
                    var listBarChange2 = keybindBarClasses.Where(p => p.key.ToLower().Equals(keyPressedValue.ToString().ToLower()) && p.modifier.ToLower().Equals(modifier.ToLower())).Select(p => p).FirstOrDefault();
                    if (listBarChange2 != null) {
                        if (listBarChange2.name.ToLower().Equals("clear")) {
                            displayImg1.Source = null;
                            displayImg2.Source = null;
                            displayImg3.Source = null;
                            displayImg4.Source = null;
                            displayImg5.Source = null;
                            displayImg6.Source = null;
                            displayImg7.Source = null;
                            displayImg8.Source = null;
                            displayImg9.Source = null;
                            displayImg10.Source = null;
                            control = false;
                            return;
                        } else if (listBarChange2.name.ToLower().Equals("pause")) {
                            pause = !pause;
                        }
                    }
                }
            }

            if (pause) {
                control = false;
                return;
            }

            foreach (var ability in abilityList) {

                if (ability == null)
                    continue;

                keypressed.modifier = modifier;
                keypressed.key = keyPressedValue.ToString();
                keypressed.ability.name = ability.name;
                keypressed.ability.img = ability.img;             
                keypressed.timepressed = stopwatch.Elapsed.TotalMilliseconds;

                for (int i = 0; i < ListPreviousKeypressed.Count; i++) {
                    var prevabil = ListPreviousKeypressed[i];
                    if ((keypressed.timepressed - prevabil.timepressed) > 1200) {
                        ListPreviousKeypressed.RemoveAt(i);
                        i--;
                    }
                }

                previousKey = ListPreviousKeypressed.Where(a => a.ability.img.Equals(keypressed.ability.img)).Select(a => a).FirstOrDefault();
                if (previousKey != null) {
                    control = false;
                    return;
                }
                ListKeypressed.Add(keypressed);
                previousKey = new Keypressed() {
                    timepressed = keypressed.timepressed,
                    ability = new Ability {
                        img = keypressed.ability.img,
                        name = keypressed.ability.name
                    }
                };
                ListPreviousKeypressed.Add(previousKey);


                ImageSource imageSource;
                //if (trackCD) {
                //    bool onCD = abilCoolDown(ListPreviousKeys, keypressed);
                //    if (onCD) {
                //        Image = Tint(bitmap, System.Drawing.Color.Red, 0.5f);
                //        imageSource = ImageSourceFromBitmap(Image);
                //    } else {
                //        imageSource = ImageSourceFromBitmap(bitmap);
                //        ListPreviousKeys.Add(previousKey);
                //    }
                //} else {
                imageSource = ImageSource.FromFile(ability.img);
                ListPreviousKeys.Add(previousKey);
                //}

                //Display
                switch (imgCounter) {
                    case 0:
                        displayImg10.Source = imageSource;
                        break;
                    case 1:
                        moveImgs(imgCounter);
                        displayImg10.Source = imageSource;
                        break;
                    case 2:
                        moveImgs(imgCounter);
                        displayImg10.Source = imageSource;
                        break;
                    case 3:
                        moveImgs(imgCounter);
                        displayImg10.Source = imageSource;
                        break;
                    case 4:
                        moveImgs(imgCounter);
                        displayImg10.Source = imageSource;
                        break;
                    case 5:
                        moveImgs(imgCounter);
                        displayImg10.Source = imageSource;
                        break;
                    case 6:
                        moveImgs(imgCounter);
                        displayImg10.Source = imageSource;
                        break;
                    case 7:
                        moveImgs(imgCounter);
                        displayImg10.Source = imageSource;
                        break;
                    case 8:
                        moveImgs(imgCounter);
                        displayImg10.Source = imageSource;
                        break;
                    case 9:
                        moveImgs(imgCounter);
                        displayImg10.Source = imageSource;
                        break;
                    default:
                        moveImgs(imgCounter);
                        displayImg10.Source = imageSource;
                        break;
                }
                if (imgCounter < 9)
                    imgCounter++;
            }
            if (keybindBarClasses != null) {
                var listBarChange = keybindBarClasses.Where(p => p.key.ToLower().Equals(keyPressedValue.ToString().ToLower()) && p.modifier.ToLower().Equals(modifier.ToLower()) && (p.bar.name.ToLower().Equals(style.ToLower()) || p.bar.name.Equals("ALL"))).Select(p => p).FirstOrDefault();
                if (listBarChange != null) {
                    if (!listBarChange.name.ToLower().Equals("pause") && !listBarChange.name.ToLower().Equals("clear")) {
                        style = listBarChange.name;
                        changeStyle();
                    }
                }
            }
            control = false;
        }
        #endregion
    }
    public void changeStyle() {
        keybindClasses = JsonConvert.DeserializeObject<List<KeybindClass>>(File.ReadAllText(Path.Combine(mainDir + "keybinds.json")));
        keybindClasses = keybindClasses.Where(p => p.bar.name.ToLower() == style.ToLower() || p.bar.name.ToLower() == "all").Select(p => p).ToList();
    }

    private void moveImgs(int counter) {
        switch (counter) {
            case 1:
                displayImg9.Source = displayImg10.Source;
                break;
            case 2:
                displayImg8.Source = displayImg9.Source;
                displayImg9.Source = displayImg10.Source;
                break;
            case 3:
                displayImg7.Source = displayImg8.Source;
                displayImg8.Source = displayImg9.Source;
                displayImg9.Source = displayImg10.Source;
                break;
            case 4:
                displayImg6.Source = displayImg7.Source;
                displayImg7.Source = displayImg8.Source;
                displayImg8.Source = displayImg9.Source;
                displayImg9.Source = displayImg10.Source;
                break;
            case 5:
                displayImg5.Source = displayImg6.Source;
                displayImg6.Source = displayImg7.Source;
                displayImg7.Source = displayImg8.Source;
                displayImg8.Source = displayImg9.Source;
                displayImg9.Source = displayImg10.Source;
                break;
            case 6:
                displayImg4.Source = displayImg5.Source;
                displayImg5.Source = displayImg6.Source;
                displayImg6.Source = displayImg7.Source;
                displayImg7.Source = displayImg8.Source;
                displayImg8.Source = displayImg9.Source;
                displayImg9.Source = displayImg10.Source;
                break;
            case 7:
                displayImg3.Source = displayImg4.Source;
                displayImg4.Source = displayImg5.Source;
                displayImg5.Source = displayImg6.Source;
                displayImg6.Source = displayImg7.Source;
                displayImg7.Source = displayImg8.Source;
                displayImg8.Source = displayImg9.Source;
                displayImg9.Source = displayImg10.Source;
                break;
            case 8:
                displayImg2.Source = displayImg3.Source;
                displayImg3.Source = displayImg4.Source;
                displayImg4.Source = displayImg5.Source;
                displayImg5.Source = displayImg6.Source;
                displayImg6.Source = displayImg7.Source;
                displayImg7.Source = displayImg8.Source;
                displayImg8.Source = displayImg9.Source;
                displayImg9.Source = displayImg10.Source;
                break;
            case 9:
                displayImg1.Source = displayImg2.Source;
                displayImg2.Source = displayImg3.Source;
                displayImg3.Source = displayImg4.Source;
                displayImg4.Source = displayImg5.Source;
                displayImg5.Source = displayImg6.Source;
                displayImg6.Source = displayImg7.Source;
                displayImg7.Source = displayImg8.Source;
                displayImg8.Source = displayImg9.Source;
                displayImg9.Source = displayImg10.Source;
                break;
        }
    }
}