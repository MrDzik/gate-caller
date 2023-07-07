using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GateCaller.Controls
{
    internal class SettingsLangView : ContentView
    {
        public static readonly BindableProperty LangNameProperty = BindableProperty.Create(nameof(LangName), typeof(string), typeof(SettingsLangView), string.Empty);
        public static readonly BindableProperty LangCodeProperty = BindableProperty.Create(nameof(LangCode), typeof(string), typeof(SettingsLangView), string.Empty);
        public static readonly BindableProperty ColorProperty = BindableProperty.Create(nameof(Color), typeof(Color), typeof(SettingsLangView), Colors.Gray);


        public string LangName
        {
            get => (string)GetValue(LangNameProperty);
            set => SetValue(LangNameProperty, value);
        }
        public string LangCode
        {
            get => (string)GetValue(LangCodeProperty);
            set => SetValue(LangCodeProperty, value);
        }
        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }
    }
}
