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

    private void MainPage_OnLoaded(object sender, EventArgs e)
    {
#if ANDROID
        var access = MainActivity.CheckAndRequestForLocPermission();
        if (!access) return;
        Task.Run(UpdateLocation);
#endif
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
            if (count != 0)
            {
                NoGatesLabel.IsVisible = false;
                for (var i = 0; i < count; i++)
                {
                    StackLayout.Children.Add(new GateView()
                    {
                        GateName = GateHelper.Gates[i].Name,
                        GateLocation = GateHelper.Gates[i].Location,
                        GatePos = i,
                        ControlTemplate = (ControlTemplate)Resources["GateViewControlTemplate"]
                    });
                }
            }
            else
            {
                NoGatesLabel.IsVisible = true;
            }
            LoadingLabel.IsVisible = false;
        });
    }

    private async Task UpdateLocation()
    {
        while (true)
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
                var location = await Geolocation.Default.GetLocationAsync(request);
                Dispatcher.Dispatch(() =>
                {
                    if (location != null)
                    {
                        foreach (var stackLayoutChild in StackLayout.Children)
                        {
                            var gateLocation = ((GateView)stackLayoutChild).GateLocation;
                            if (gateLocation != null)
                            {
                                var distance = Location.CalculateDistance(location, gateLocation,
                                    DistanceUnits.Kilometers);
                                ((GateView)stackLayoutChild).GateDistance = $"( {(int)Math.Round(distance * 1000)} m )";
                            }
                            else
                            {
                                ((GateView)stackLayoutChild).GateDistance = "( Brak lokalizacji )";
                            }
                        }
                    }
                    else
                    {
                        foreach (var stackLayoutChild in StackLayout.Children)
                        {
                            ((GateView)stackLayoutChild).GateDistance = "( ??? m )";
                        }
                    }
                });
            }
            catch (Exception)
            {
                Dispatcher.Dispatch(() =>
                {
                    foreach (var stackLayoutChild in StackLayout.Children)
                    {
                        ((GateView)stackLayoutChild).GateDistance = "(Nie można ustalić lokalizacji)";
                    }
                });
            }
            await Task.Delay(5000);
        }
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

