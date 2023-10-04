using Microsoft.Maui.Dispatching;
using Rs3TrackerMAUI.ContentPages;

namespace Rs3TrackerMAUI;

public partial class MainPage : ContentPage {
 
    Display display = null;
    public MainPage() {
        InitializeComponent();
        Loaded += MainPage_Loaded;        
    }

    private void MainPage_Loaded(object sender, EventArgs e) {
       
    }

    private void btnAbilityConfig_Clicked(object sender, EventArgs e) {

    }

    private void btnBars_Clicked(object sender, EventArgs e) {

    }

    private void btnSettings_Clicked(object sender, EventArgs e) {

    }

    private void btnServer_Clicked(object sender, EventArgs e) {

    }

    private void btnClose_Clicked(object sender, EventArgs e) {

    }

    private void btnStartTracker_Clicked(object sender, EventArgs e) {
        if (display != null) {
            //displayX = display.Left;
            //displayY = display.Top;
            //DisplayHeight = display.Height;
            //DisplayWidth = display.Width;
         //   display.Close();
            display = null;
            btnStartTracker.Text = "Start Tracker";

        } else {
            string mainDir = "";
#if WINDOWS
            mainDir = AppDomain.CurrentDomain.BaseDirectory;
#endif
#if MACCATALYST
            mainDir = AppDomain.CurrentDomain.BaseDirectory.Replace("Rs3TrackerMAUI.app/Content/MonoBundle","");
#endif
            if (!File.Exists(mainDir+"\\keybinds.json")) {
                 DisplayAlert("Alert", "Missing Keybinds", "OK");
                //MessageBox.Show("Missing Keybinds");
                return;
            }
            if (!File.Exists(mainDir+"\\barkeybinds.json")) {
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
            var secondWindow = new Window {
                Page = new Display {
                    // ...
                }
            };

            Application.Current.OpenWindow(secondWindow);
        }
    }

    private void CanResize_CheckedChanged(object sender, CheckedChangedEventArgs e) {

    }

    private void cmbMode_SelectedIndexChanged(object sender, EventArgs e) {

    }
}

