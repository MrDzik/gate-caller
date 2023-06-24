using System.ComponentModel;
using GateCaller.Classes;
using GateCaller.Helpers;
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
            Header.Text = "EDYCJA BRAMY";
            Location = gate.Location;
        }
        else
        {
            GatePos.Maximum = GateHelper.Gates.Count + 1;
            GatePos.Value = GatePos.Maximum;
            Header.Text = "TWORZENIE BRAMY";
        }
        GatePosLabel.Text = $"Pozycja bramy na liście: {GatePos.Value}";
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
        GatePosLabel.Text = $"Pozycja bramy na liście: {step}";
    }

    private void SaveButton_OnClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(GateName.Text))
        {

            DisplayAlert("Błąd", "Musisz podać nazwę bramy.", "OK");
            return;
        }

        if (string.IsNullOrEmpty(GatePhone.Text) || string.IsNullOrEmpty(GatePhonePrefix.Text))
        {
            DisplayAlert("Błąd", "Musisz podać numer telefonu.", "OK");
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
            DisplayAlert("Zapisano", "Poprawnie zapisano bramę.", "OK");
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
                        LocationLabel.Text = "Brak lokalizacji.";
                    });
                }
                else
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
                    var location = await Geolocation.Default.GetLocationAsync(request);
                    Dispatcher.Dispatch(() =>
                    {
                        if (location == null)
                        {
                            LocationLabel.Text = "Aktualna odległość: ?? m";
                        }
                        else
                        {
                            var distance = Location.CalculateDistance(location, Location,
                                DistanceUnits.Kilometers);
                            LocationLabel.Text = "Aktualna odległość: " + (int)Math.Round(distance * 1000) + " m"; 
                        }
                    });
                }
            }
            catch (Exception)
            {
                Dispatcher.Dispatch(() =>
                {
                    LocationLabel.Text = "Aktualna odległość: Bład, sprawdź uprawnienia";
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
            LocationLabel.Text = "Brak uprawnień do lokalizacji";
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
                DisplayAlert("Lokalizacja", "Lokalizacja poprawnie zczytana.", "OK");
            });
        }
        catch (Exception)
        {
            Dispatcher.Dispatch(() =>
            {
                DisplayAlert("Lokalizacja", "Wystąpił problem przy pobieraniu lokalizacji, sprawdź uprawnienia i status GPS.", "OK");
            });
        }
    }
}