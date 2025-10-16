using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Management;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Versioning;
using System.IO;

namespace SolidSilnique.Core.Diagnostics
{
    public static class UserConfigurationInfo
    {
        private static string path = "configuration.txt";
        public async static Task writeUserConfiguration(string graphicsDeviceDescription) {
            await Task.Run(() => {
                // MaGiC of LINQ
                ManagementObject cpuInfo = new ManagementObjectSearcher("select * from win32_Processor")
                    .Get()
                    .Cast<ManagementObject>()
                    .First();
                string cpu = (string)cpuInfo["Name"];
                /// Check if it is laptop or pc. if laptop then check of it is plugged in
                PowerLineStatus status = SystemInformation.PowerStatus.PowerLineStatus;
                string powerConfiguration = status switch
                {
                    PowerLineStatus.Online => "Runs on AC power",
                    PowerLineStatus.Offline => "Runs on battery",
                    PowerLineStatus.Unknown => "Stationary setup",
                    _ => "Unknown"
                };
                //float totalMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / (1024 * 1024 * 1024);
                ulong totalMemory = 0;
                // mAgIc of LINQ
                var searcher = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory");
                // rotates over EVERY RAM STICK
                foreach (ManagementObject obj in searcher.Get())
                {
                    totalMemory += (ulong)obj["Capacity"];
                }

                double totalMemoryGB = totalMemory / 1024.0 / 1024.0 / 1024.0;
                using StreamWriter writer = new StreamWriter(path);
                writer.Write($"CPU: {cpu};\nRAM: {totalMemoryGB}GB;\nGPU: {graphicsDeviceDescription};\n{powerConfiguration}");
                writer.FlushAsync();
            });
        }
    }
}
