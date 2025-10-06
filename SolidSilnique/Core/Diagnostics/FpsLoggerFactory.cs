using SolidSilnique.Core.Interfaces;

#nullable enable

namespace SolidSilnique.Core.Diagnostics
{
    public static class FpsLoggerFactory
    {
        private static IFileManager<float>? logger;

        public static IFileManager<float> Logger
        {
            get
            {
                logger ??= new FpsLogger("report.csv");

                return logger;
            }
        }
    }
}
