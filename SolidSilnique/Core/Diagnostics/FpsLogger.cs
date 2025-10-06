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
    internal class FpsLogger : IFileManager<float>, IDisposable
    {
        /// <summary>
        /// Path where report will be written
        /// </summary>
        private string _path;
        private BlockingCollection<float> _averagedValues;
        /// <summary>
        /// Task that writes values to csv file
        /// </summary>
        private Task InputOutputTask;

        private CancellationTokenSource _cts = new();

        /// <summary>
        /// Values sent by main thread to be written
        /// </summary>
        public BlockingCollection<float> AveragedValues
        {
            get
            {
                return _averagedValues;
            }
        }

        internal FpsLogger(string path)
        {
            _path = path;
            _averagedValues = new BlockingCollection<float>();
            InputOutputTask = Task.Run(WriteRecord);
        }

        public string Path
        {
            get { return _path; }
        }

        public float Read()
        {
            throw new NotImplementedException();
        }

        public void Write(float value)
        {
            AveragedValues.Add(value);
        }

        private async Task WriteRecord()
        {
            try
            {
                using StreamWriter writer = new StreamWriter(Path);
                using CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                foreach (float fps in AveragedValues.GetConsumingEnumerable(_cts.Token))
                {
                    csv.WriteField(fps);
                    await csv.FlushAsync();
                }
            } catch (OperationCanceledException)
            {

            }
        }

        public void Dispose()
        {
            _averagedValues.CompleteAdding();
            _cts.Cancel();

            try
            {
                InputOutputTask.Wait();
            }
            catch (AggregateException) { }

            _cts.Dispose();
            _averagedValues.Dispose();
        }
    }
}
