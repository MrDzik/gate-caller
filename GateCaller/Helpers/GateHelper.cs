using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using GateCaller.Classes;

namespace GateCaller.Helpers
{
    internal static class GateHelper
    {
        public static List<Gate> Gates = new List<Gate>();

        public static async Task LoadGates()
        {
            Gates.Clear();
            var gates = await SecureStorage.Default.GetAsync("Gates");
            if (string.IsNullOrEmpty(gates)) return;
            Gates = JsonSerializer.Deserialize<List<Gate>>(gates);
        }

        public static async Task UpdateGates()
        {
            SecureStorage.Default.Remove("Gates");
            await SecureStorage.Default.SetAsync("Gates", JsonSerializer.Serialize(Gates));
        }

        public static void ResetGates()
        {
            Gates.Clear();
            SecureStorage.Default.Remove("Gates");
        }
    }
}
