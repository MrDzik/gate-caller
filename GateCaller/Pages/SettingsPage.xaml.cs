using GateCaller.Classes;
using GateCaller.Controls;
using GateCaller.Helpers;
using GateCaller.Pages;
using GateCaller.Resources.Strings;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GateCaller;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
    }

    private void SettingsPage_OnNavigatedTo(object sender, NavigatedToEventArgs e)
    {
        Task.Run(LoadData);
    }

    private async Task LoadData()
    {
        Dispatcher.Dispatch(() =>
        {
            LoadingLabel.IsVisible = false;
        });
        var lang = await LangHelper.GetLang();
        await GateHelper.LoadGates();
        var count = GateHelper.Gates.Count;
        Dispatcher.Dispatch(() =>
        {
            StackLayout.Children.Clear();
            if (count == 0)
            {
                StackLayout.Children.Add(new Label()
                {
                    Text = AppRes.Message_NoGates,
                    HorizontalTextAlignment = TextAlignment.Center
                });
            }
            else
            {
                for (var i = 0; i < count; i++)
                {
                    StackLayout.Children.Add(new SettingsGateView()
                    {
                        GateName = GateHelper.Gates[i].Name,
                        GatePos = i,
                        ControlTemplate = (ControlTemplate)Resources["SettingsGateViewTemplate"]
                    });
                }
            }

            lang ??= Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            LangStackLayout.Children.Add(new SettingsLangView()
            {
                LangName = "Polski",
                LangCode = "pl",
                Color = lang == "pl" ? Color.FromArgb("#512BD4") : Colors.Gray,
                ControlTemplate = (ControlTemplate)Resources["SettingsLangViewTemplate"]
            });
            LangStackLayout.Children.Add(new SettingsLangView()
            {
                LangName = "English",
                LangCode = "en",
                Color = lang == "en" ? Color.FromArgb("#512BD4") : Colors.Gray,
                ControlTemplate = (ControlTemplate)Resources["SettingsLangViewTemplate"]
            });

            var button = new Button()
            {
                Style = (Style)Resources["AddButtonStyle"],
            };
            button.Clicked += AddGate_OnClicked;
            StackLayout.Children.Add(button);
            LoadingLabel.IsVisible = false;
        });
    }

    private void AddGate_OnClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new GateEditPage(-1), true);
    }


    private void EditButton_OnClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var gate = (SettingsGateView)button.BindingContext;
        Navigation.PushAsync(new GateEditPage(gate.GatePos), true);
    }
    private void DeleteButton_OnClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var gate = (SettingsGateView)button.BindingContext;
        Task.Run(() =>
        {
            RemoveGate(gate.GatePos).Wait();
        });
    }

    private async Task RemoveGate(int gatePos)
    {
        GateHelper.Gates.RemoveAt(gatePos);
        await GateHelper.UpdateGates();
        await Task.Run(LoadData);
    }

    private void LangButton_OnClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var lang = (SettingsLangView)button.BindingContext;
        Task.Run(async () =>
        {
            await LangHelper.ChangeLang(lang.LangCode);
            Dispatcher.Dispatch(() =>
            {
                LangHelper.ChangeCulture(lang.LangCode);
                DisplayAlert(AppRes.SettingsLangChangeAlertTitle, AppRes.SettingsLangChangeAlertMessage, "OK");
                (App.Current as App).MainPage = new AppShell();
            });
        });
    }
}