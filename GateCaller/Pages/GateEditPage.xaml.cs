using GateCaller.Classes;
using GateCaller.Helpers;

namespace GateCaller.Pages;

public partial class GateEditPage : ContentPage
{
    public int Index { get; set; }
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
        }
        else
        {
            GatePos.Maximum = GateHelper.Gates.Count + 1;
            GatePos.Value = GatePos.Maximum;
            Header.Text = "TWORZENIE BRAMY";
        }
        GatePosLabel.Text = $"Pozycja bramy na liście: {GatePos.Value}";
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
            GateHelper.MoveGates(Index, (int)GatePos.Value - 1);
            await GateHelper.UpdateGates();
        }
        else
        {
            var gate = new Gate(GateName.Text, GatePhonePrefix.Text + " " + GatePhone.Text);
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
}