using GateCaller.Helpers;
using Thread = System.Threading.Thread;

namespace GateCaller;

public partial class App : Application
{
	public App()
    {
        var lang = Task.Run(LangHelper.GetLang).Result ?? Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        LangHelper.ChangeCulture(lang);
        InitializeComponent();
        MainPage = new AppShell();
    }
}
