using CsvHelper;
using SolidSilnique.Core.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSilnique.Core.Diagnostics
{
    internal class AdvancedFpsLogger : IFileManager<(string scene, float fps)>, IDisposable
    {

        private string _path;
        private readonly Task task;
        private BlockingCollection<(string scene, float fps)> values;
        private CancellationTokenSource _cts;

        internal AdvancedFpsLogger(string path)
        {
            _path = path;
            _cts = new CancellationTokenSource();
            values = new BlockingCollection<(string scene, float fps)>();
            task = Task.Run(Execute);
        }

        private async void Execute()
        {
            try
            {
                using StreamWriter writer = new StreamWriter(_path);
                using CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                foreach (var tuple in values.GetConsumingEnumerable(_cts.Token))
                {
                    csv.WriteField($"{tuple.scene} {tuple.fps}");
                    await writer.FlushAsync();
                }
            } catch (OperationCanceledException)
            {

            }
        }

        public (string scene, float fps) Read()
        {
            throw new NotImplementedException();
        }

        public void Write((string scene, float fps) input)
        {
            values.Add(input);
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
            catch (InvalidOperationException) {
            }


            _cts.Dispose();
            values.Dispose();
        }
    }
}
