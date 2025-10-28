#nullable enable

using SolidSilnique.Core.Interfaces;

namespace SolidSilnique.Core.Diagnostics
{
    public static class AdvancedFpsLoggerFactory
    {
        private static AdvancedFpsLogger? logger;

        public static IFileManager<float> Logger
        {
            get
            {
                logger ??= new AdvancedFpsLogger("advanced_report.csv");

                return logger;
            }
        }

        public static ISceneSwapable<string> GetSceneSwapableLogger()
        {
            logger ??= new AdvancedFpsLogger("advanced_report.csv");

            return logger;
        }
    }
}
