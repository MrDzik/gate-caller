using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GateCaller.Controls
{
    internal class SettingsGateView : ContentView
    {
        public static readonly BindableProperty GateNameProperty = BindableProperty.Create(nameof(GateName), typeof(string), typeof(SettingsGateView), string.Empty);
        public static readonly BindableProperty GatePosProperty = BindableProperty.Create(nameof(GatePos), typeof(int), typeof(SettingsGateView));


        public string GateName
        {
            get => (string)GetValue(GateNameProperty);
            set => SetValue(GateNameProperty, value);
        }
        public int GatePos
        {
            get => (int)GetValue(GatePosProperty);
            set => SetValue(GatePosProperty, value);
        }
    }
}
