using GateCaller.Controls;
using GateCaller.Helpers;

namespace GateCaller;

public partial class MainPage : ContentPage
{
    public MainPage()
	{
		InitializeComponent();
    }

    private void MainPage_Reload(object sender, EventArgs e)
    {
        Task.Run(LoadData);
    }

    private async Task LoadData()
    {
        Dispatcher.Dispatch(() =>
        {
            LoadingLabel.IsVisible = false;
        });
        await GateHelper.LoadGates();
        var count = GateHelper.Gates.Count;
        Dispatcher.Dispatch(() =>
        {
            StackLayout.Children.Clear();
            if (count == 0)
            {
                StackLayout.Children.Add(new Label()
                {
                    Text = "Nie posiadasz jeszcze żadnych bram, może jakąś dodasz?",
                    HorizontalTextAlignment = TextAlignment.Center
                });
            }
            else
            {
                for (var i = 0; i < count; i++)
                {
                    StackLayout.Children.Add(new GateView()
                    {
                        GateName = GateHelper.Gates[i].Name,
                        GatePos = i,
                        ControlTemplate = (ControlTemplate)Resources["GateViewControlTemplate"]
                    });
                }
            }
            LoadingLabel.IsVisible = false;
        });
    }

    private void OpenButton_OnClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var gate = (GateView)button.BindingContext;
#if ANDROID
        MainActivity.CallPhone(GateHelper.Gates[gate.GatePos].PhoneNumber);
#endif
    }
}

