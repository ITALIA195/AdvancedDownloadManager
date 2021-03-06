﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ADM.Core
{
    public class Downloader
    {
        private const string DownloadingSuffix = ".downloading";
        private const int BufferSize = 1024 * 5;
        
        private readonly FileProperties _file;
        private readonly DownloadTracker _tracker;
        private DownloadState _state;
        
        public Downloader(FileProperties file)
        {
            _file = file;
            _tracker = new DownloadTracker();
        }

        public async void Start()
        {
            var file = _file.Path + _file.FileName + "." + _file.Extension;
            var tmp = file + DownloadingSuffix;
            
            await Download(tmp);
            if (State == DownloadState.Completed)
                SafeMoveFile(tmp, _file);
        }

        public void Pause() => State = DownloadState.Paused;
        public void Resume() => Start();
        public void Cancel() => State = DownloadState.Failed;

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
        
        private async Task Download(string downloadPath)
        {
            var request = (HttpWebRequest) WebRequest.Create("http://" + _file.DownloadUri); //TODO: Ensure http doesn't get repeated
            request.Method = "GET";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
            if (GetRange(downloadPath, out long range))
                request.AddRange(range);

            using (var response = (HttpWebResponse) await request.GetResponseAsync())
            using (var stream = response.GetResponseStream())
            {
                if (stream == null)
                {
                    State = DownloadState.Failed;
                    return;
                }
                
                if (response.ContentLength > 0)
                    _tracker.SetContentLength(response.ContentLength);
                
                if (response.StatusCode == HttpStatusCode.PartialContent)
                    _tracker.AddPartialContent(range);
                
                using (var fs = new FileStream(downloadPath, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    _tracker.Start();
                    State = DownloadState.Running;
                    
                    int bytesRead;
                    var buffer = new byte[BufferSize];
                    while (State == DownloadState.Running && (bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fs.WriteAsync(buffer, 0, bytesRead);
                        _receivedBytes += bytesRead;
                        _receivedBytesLastSecond += bytesRead;

                        long elapsed = Time.ElapsedTime - _lastUpdate; 
                        if (elapsed >= 1000)
                        {
                            DownloadSpeed = _receivedBytesLastSecond / (elapsed / 1000f);
                            _receivedBytesLastSecond = 0;
                            _lastUpdate = Time.ElapsedTime;
                        }
                        
                        if (_totalBytes > 0)
                            Progress = (double) _receivedBytes / _totalBytes * 100;
                    }

                    if (State == DownloadState.Running)
                        State = DownloadState.Completed;
                    
                    await fs.FlushAsync();
                    fs.Close();
                }
            }
        }

        private static bool SafeMoveFile(string tempFile, FileProperties file)
        {
            if (!File.Exists(tempFile))
                return false;

            var dst = file.Path + file.FileName + "." + file.Extension;
            
            int i = 0;
            while (File.Exists(dst))
                dst = $"{file.Path}{file.FileName} ({++i}).{file.Extension}";

            File.Move(tempFile, dst);
            return true;
        }

        public double DownloadSpeed
        {
            get => _downloadSpeed;
            private set
            {
                if (_downloadSpeed == value)
                    return;
                
                _downloadSpeed = value;
                NotifyChanged(nameof(DownloadSpeed));
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

        public FileProperties FileProperties => _file;
        public long ReceivedBytes => _receivedBytes;
        public long TotalBytes => _totalBytes;
        public event EventHandler StateChanged;
        public event EventHandler DownloadFailed;
        public event EventHandler ProgressChanged;
    }

    public enum DownloadState
    {
        None,
        Running,
        Completed,
        Paused,
        Failed
    }
}