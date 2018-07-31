using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
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
        private const int BufferSize = 1024 * 5;
        private readonly FileProperties _file;
        
        private double _progress;
        private DownloadState _state;
        
        private long _receivedBytes;
        private long _totalBytes;
        
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
//            if (File.Exists(Path.Combine(_file.Path, _file.FileName + ".downloading")))
//                File.Delete(Path.Combine(_file.Path, _file.FileName + ".downloading"));
            _receivedBytes = 0;
            _totalBytes = -1;
            
        }

        public Task Resume()
        {
            return Start();
        }
        
        private async Task Download()
        {
            var file = Path.Combine(_file.Path, _file.FileName);
            if (_receivedBytes <= 0 && File.Exists(file + ".downloading"))
            {
                var fileInfo = new FileInfo(file + ".downloading");
                _receivedBytes = fileInfo.Length; // Resume download from where it was left
            }
            
            var request = (HttpWebRequest) WebRequest.Create(_file.DownloadUri);
            request.Method = "GET";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
            if (_receivedBytes > 0)
                request.AddRange(_receivedBytes);


            using (var response = (HttpWebResponse) await request.GetResponseAsync())
            {
                Console.WriteLine("Server response: " + response.StatusCode);
                if (response.StatusCode != HttpStatusCode.PartialContent)
                    _receivedBytes = 0;
                _totalBytes = response.ContentLength;
                using (var stream = response.GetResponseStream())
                {
                    if (stream == null)
                    {
                        State = DownloadState.Failed;
                        throw new InvalidOperationException();
                    }
                    
                    using (var fs = new FileStream(file + ".downloading", FileMode.Append, FileAccess.Write, FileShare.Read))
                    {
                        State = DownloadState.Running;
                        
                        var buffer = new byte[BufferSize];
                        int bytesRead;
                        
                        while (State == DownloadState.Running && (bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fs.WriteAsync(buffer, 0, bytesRead);
                            _receivedBytes += bytesRead;
                            Progress = (double) _receivedBytes / _totalBytes * 100;
                        }

                        await fs.FlushAsync();
                        fs.Close();
                    }
                }
            }

            if (!File.Exists(file + ".downloading"))
            {
                State = DownloadState.Failed;
            }
            else if (State == DownloadState.Running)
            {
                if (!File.Exists(file))
                {
                    File.Move(file + ".downloading", file);
                }
                else
                {
                    var extension = Path.GetExtension(file);
                    var newPosition = Path.Combine(_file.Path, Path.GetFileNameWithoutExtension(file));
                    
                    int i = 1;
                    while (File.Exists($"{newPosition} ({i}){extension}"))
                        i++;
                    
                    File.Move(file + ".downloading", $"{newPosition} ({i}){extension}");
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