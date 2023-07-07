using GateCaller.Controls;
using GateCaller.Helpers;
using System.Threading;
using GateCaller.Resources.Strings;

namespace GateCaller;

public partial class MainPage : ContentPage
{
    public MainPage()
	{
		InitializeComponent();
    }
    private CancellationTokenSource _cancellationTokenSource;
    private CancellationToken _cancellationToken;

    private void MainPage_OnAppearing(object sender, EventArgs e)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
        Task.Run(LoadData, _cancellationToken).Wait(_cancellationToken);

#if ANDROID
        var access = MainActivity.CheckAndRequestForLocPermission();
        if (!access) return;
        Task.Run(UpdateLocation);
#endif
    }
    private void MainPage_OnDisappearing(object sender, EventArgs e)
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
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
        while (!_cancellationToken.IsCancellationRequested)
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
                                ((GateView)stackLayoutChild).GateDistance = $"( {AppRes.MainPageNoLocation} )";
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
                        ((GateView)stackLayoutChild).GateDistance = $"( {AppRes.MainPageLocationError} )";
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

