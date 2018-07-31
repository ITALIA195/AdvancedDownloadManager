using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Documents;
using AdvancedDownloadManager.Annotations;
using AdvancedDownloadManager.Events;

namespace AdvancedDownloadManager
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class Downloader : INotifyPropertyChanged
    {
        private const string DownloadingSuffix = ".downloading";
        private const int BufferSize = 1024 * 5;
        private readonly FileProperties _file;
        
        private double _progress;
        private DownloadState _state;
        
        public Downloader(FileProperties file)
        {
            _file = file;
        }

        public Task Start()
        {
            Console.WriteLine("Starting download of " + _file.FileName);
            return Download();
        }

        public void Pause()
        {
            State = DownloadState.Paused;
        }

        public void Cancel()
        {
            State = DownloadState.Stopped;
            
        }

        public void Resume()
        {
            State = DownloadState.Running;
        }

        private static bool GetRange(string file, out long range)
        {
            range = 0;
            if (!File.Exists(file)) 
                return false;

            try
            {
                var fileInfo = new FileInfo(file);
                range = fileInfo.Length;
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        private async Task Download()
        {
            var file = Path.Combine(_file.Path, _file.FileName);
            
            var request = (HttpWebRequest) WebRequest.Create(_file.DownloadUri);
            request.Method = "GET";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
            if (GetRange(file + DownloadingSuffix, out long range))
                request.AddRange(range);


            using (var response = (HttpWebResponse) await request.GetResponseAsync())
            {
                Console.WriteLine("HTTP " + (int) response.StatusCode);
                using (var stream = response.GetResponseStream())
                {
                    if (stream == null)
                    {
                        State = DownloadState.Failed;
                        throw new InvalidOperationException();
                    }
                    
                    using (var fs = new FileStream(file + DownloadingSuffix, FileMode.Append, FileAccess.Write, FileShare.Read))
                    {
                        State = DownloadState.Running;
                        
                        var buffer = new byte[BufferSize];

                        int bytesRead;
                        long receivedBytes = 0;
                        long totalBytes = response.ContentLength;
                        while ((State == DownloadState.Running || State == DownloadState.Paused) && (bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            while (State == DownloadState.Paused)
                                Thread.Sleep(1000);
                            await fs.WriteAsync(buffer, 0, bytesRead);
                            receivedBytes += bytesRead;
                            Progress = (double) (receivedBytes + range) / (totalBytes + range) * 100;
                        }

                        await fs.FlushAsync();
                        fs.Close();
                    }
                }
            }

            if (!File.Exists(file + DownloadingSuffix))
            {
                State = DownloadState.Failed;
            }
            else if (State == DownloadState.Running)
            {
                if (!File.Exists(file))
                {
                    File.Move(file + DownloadingSuffix, file);
                }
                else
                {
                    var extension = Path.GetExtension(file);
                    var newPosition = Path.Combine(_file.Path, Path.GetFileNameWithoutExtension(file));
                    
                    int i = 1;
                    while (File.Exists($"{newPosition} ({i}){extension}"))
                        i++;
                    
                    File.Move(file + DownloadingSuffix, $"{newPosition} ({i}){extension}");
                }

                State = DownloadState.Completed;
            }
            else
            {
                State = DownloadState.Paused;
            }
        }

        public double Progress
        {
            get => _progress;
            private set
            {
                if (_progress == value)
                    return;
                
                _progress = value;
                NotifyChanged(nameof(Progress));
            }
        }

        public DownloadState State
        {
            get => _state;
            private set
            {
                if (_state == value)
                    return;
                
                _state = value;
                NotifyChanged(nameof(State));
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void NotifyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum DownloadState
    {
        Stopped,    
        Running,
        Completed,    
        Paused,
        Failed,
        Queued
    }
}