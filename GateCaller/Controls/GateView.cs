using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GateCaller.Controls
{
    // This class is now a mirror of SettingsGateView,
    // but this will change when new features comes in 
    internal class GateView : ContentView
    {
        public static readonly BindableProperty GateNameProperty = BindableProperty.Create(nameof(GateName), typeof(string), typeof(GateView), string.Empty);
        public static readonly BindableProperty GatePosProperty = BindableProperty.Create(nameof(GatePos), typeof(int), typeof(GateView));
        public static readonly BindableProperty GateLocationProperty = BindableProperty.Create(nameof(GateLocation), typeof(Location), typeof(GateView), null);
        public static readonly BindableProperty GateDistanceProperty = BindableProperty.Create(nameof(GateDistance), typeof(string), typeof(GateView), "( Sprawdzam... )");
        
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
        public Location? GateLocation
        {
            get => (Location?)GetValue(GateLocationProperty);
            set => SetValue(GateLocationProperty, value);
        }
        public string GateDistance
        {
            get => (string)GetValue(GateDistanceProperty);
            set => SetValue(GateDistanceProperty, value);
        }
    }
}
