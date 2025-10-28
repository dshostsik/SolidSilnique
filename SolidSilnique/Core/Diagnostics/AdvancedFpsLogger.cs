using CsvHelper;
using SolidSilnique.Core.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace SolidSilnique.Core.Diagnostics
{
    internal class AdvancedFpsLogger : IFileManager<float>, IDisposable, ISceneSwapable<string>
    {

        private string _path;
        private readonly Task task;
        private BlockingCollection<string> values;
        private CancellationTokenSource _cts;
        public string CurrentScene
        {
            get; set;
        } = "";

        internal AdvancedFpsLogger(string path)
        {
            _path = path;
            _cts = new CancellationTokenSource();
            values = new BlockingCollection<string>();
            task = Task.Run(Execute);
        }

        private async void Execute()
        {
            try
            {
                using StreamWriter writer = new StreamWriter(_path);
                using CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                foreach (var value in values.GetConsumingEnumerable(_cts.Token))
                {
                    csv.WriteField(value);
                    csv.NextRecord();
                    await writer.FlushAsync();
                }
            }
            catch (OperationCanceledException)
            {

            }
        }

        public float Read()
        {
            throw new NotImplementedException();
        }

        public void Write(float input)
        {
            values.Add($"{CurrentScene}:{input}");
        }

        public void Dispose()
        {
            values.CompleteAdding();
            _cts.Cancel();

            try
            {
                task.Wait();
            }
            catch (AggregateException)
            {
            }


            try
            {
                task?.Dispose();
            }
            catch (InvalidOperationException)
            {
            }


            _cts.Dispose();
            values.Dispose();
        }
    }
}
