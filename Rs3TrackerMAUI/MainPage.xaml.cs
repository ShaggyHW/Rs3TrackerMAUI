using Microsoft.Maui.Dispatching;
using SharpHook;
using SharpHook.Reactive;

namespace Rs3TrackerMAUI;

public partial class MainPage : ContentPage {
    int count = 0;

    public MainPage() {
        InitializeComponent();
        Loaded += MainPage_Loaded;        
    }

    private void MainPage_Loaded(object sender, EventArgs e) {
        var hook = new TaskPoolGlobalHook();
        hook.KeyPressed += Hook_KeyPressed;

        hook.RunAsync();
    }

    private void Hook_KeyPressed(object sender, KeyboardHookEventArgs e) {
        MainThread.BeginInvokeOnMainThread(() => {
            CounterBtn.Text = e.Data.KeyCode.ToString();
        });
    }

    private void OnCounterClicked(object sender, EventArgs e) {
        count++;

        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }
}

