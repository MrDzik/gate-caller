using System.ComponentModel;
using GateCaller.Classes;
using GateCaller.Helpers;
using GateCaller.Resources.Strings;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GateCaller.Pages;

public partial class GateEditPage : ContentPage
{
    public int Index { get; set; }
    public Location? Location { get; set; }
	public GateEditPage(int index)
	{
		InitializeComponent();
        Index = index;
        if (Index != -1)
        {
            GatePos.Maximum = GateHelper.Gates.Count;
            var gate = GateHelper.Gates[Index];
            GateName.Text = gate.Name;
            GatePhonePrefix.Text = gate.PhoneNumber.Split(" ")[0];
            GatePhone.Text = gate.PhoneNumber.Split(" ")[1];
            GatePos.Value = Index + 1;
            Header.Text = AppRes.GateEditPageEditHeader;
            Location = gate.Location;
        }
        else
        {
            GatePos.Maximum = GateHelper.Gates.Count + 1;
            GatePos.Value = GatePos.Maximum;
            Header.Text = AppRes.GateEditPageCreateHeader;
        }
        GatePosLabel.Text = $"{AppRes.GateEditPageGatePosition} {GatePos.Value}";
    }
    private void GateEditPage_OnLoaded(object sender, EventArgs e)
    {
#if ANDROID
        var access = MainActivity.CheckAndRequestForLocPermission();
        if (!access) return;
        Task.Run(UpdateLocation);
#endif
    }
    private void GatePos_OnValueChanged(object sender, ValueChangedEventArgs e)
    {
        var val = e.NewValue;
        var step = (int)Math.Round(val);
        GatePos.Value = step;
        GatePosLabel.Text = $"{AppRes.GateEditPageGatePosition} {step}";
    }

    private void SaveButton_OnClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(GateName.Text))
        {
            DisplayAlert(AppRes.Message_Error, AppRes.Error_NoGateName, "OK");
            return;
        }

        if (string.IsNullOrEmpty(GatePhone.Text) || string.IsNullOrEmpty(GatePhonePrefix.Text))
        {
            DisplayAlert(AppRes.Message_Error, AppRes.Error_NoPhoneNumber, "OK");
            return;
        }
        Task.Run(SaveGate);
    }

    private async Task SaveGate()
    {
        if (Index != -1)
        {
            GateHelper.Gates[Index].Name = GateName.Text;
            GateHelper.Gates[Index].PhoneNumber = GatePhonePrefix.Text + " " + GatePhone.Text;
            GateHelper.Gates[Index].Location = Location;
            GateHelper.MoveGates(Index, (int)GatePos.Value - 1);
            await GateHelper.UpdateGates();
        }
        else
        {
            var gate = new Gate(GateName.Text, GatePhonePrefix.Text + " " + GatePhone.Text, Location);
            GateHelper.Gates.Add(gate);
            GateHelper.MoveGates(GateHelper.Gates.Count - 1, (int)GatePos.Value - 1);
            await GateHelper.UpdateGates();
        }

        Dispatcher.Dispatch(() =>
        {
            DisplayAlert(AppRes.Message_Saved, AppRes.Message_GateSaved, "OK");
            Navigation.PopAsync();
        });
    }

    private async Task UpdateLocation()
    {
        while (IsLoaded)
        {
            try
            {
                if (Location == null)
                {
                    Dispatcher.Dispatch(() =>
                    {
                        LocationLabel.Text = AppRes.GateEditPageNoLocation;
                    });
                }
                else
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
                    var location = await Geolocation.Default.GetLocationAsync(request);
                    SetLocationText(location);
                }
            }
            catch (Exception)
            {
                Dispatcher.Dispatch(() =>
                {
                    LocationLabel.Text = $"{AppRes.GateEditPageCurrentDistance} {AppRes.GateEditPageLocationError}";
                });
            }
            await Task.Delay(5000);
        }
    }
    private void LocationButton_OnClicked(object sender, EventArgs e)
    {
#if ANDROID
        var access = MainActivity.CheckAndRequestForLocPermission();
        if (!access) {
            LocationLabel.Text = AppRes.GateEditPageLocationNoPermissions;
            return;
        }
        Task.Run(SetGateLocation);
#endif
    }
    private async Task SetGateLocation()
    {
        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
            var location = await Geolocation.Default.GetLocationAsync(request);
            if (location == null) throw new Exception("Null location");
            Location = location;
            Dispatcher.Dispatch(() =>
            {
                DisplayAlert(AppRes.Message_Location, AppRes.Message_LocationSuccess, "OK");
            });
            SetLocationText(location);
        }
        catch (Exception)
        {
            Dispatcher.Dispatch(() =>
            {
                DisplayAlert(AppRes.Message_Location, AppRes.Message_LocationFailure, "OK");
            });
        }
    }

    private void SetLocationText(Location location)
    {
        Dispatcher.Dispatch(() =>
        {
            if (location == null)
            {
                LocationLabel.Text = $"{AppRes.GateEditPageCurrentDistance} ?? m";
            }
            else
            {
                var distance = Location.CalculateDistance(location, Location,
                    DistanceUnits.Kilometers);
                LocationLabel.Text = $"{AppRes.GateEditPageCurrentDistance} {(int)Math.Round(distance * 1000)} m";
            }
        });
    }
}