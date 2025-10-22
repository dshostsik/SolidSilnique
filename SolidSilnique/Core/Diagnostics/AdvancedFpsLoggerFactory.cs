#nullable enable

using SolidSilnique.Core.Interfaces;

namespace SolidSilnique.Core.Diagnostics
{
    public static class AdvancedFpsLoggerFactory
    {
        private static IFileManager<(string scene, float fps)>? logger;

        public static IFileManager<(string scene, float fps)> Logger
        {
            get
            {
                logger ??= new AdvancedFpsLogger("advanced_report.csv");

                return logger;
            }
        }

    }
}
