using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Test.AdvancedDownloadManager.Events;

namespace Test.AdvancedDownloadManager
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class Downloader
    {
        private const int BufferSize = 1024 * 5;
        private readonly File _file;
        private bool _running;
        private long _receivedBytes;
        private long _totalBytes;
        
        public Downloader(File file)
        {
            _file = file;
        }

        public Task Start()
        {
            Console.WriteLine("Starting download of " + _file.FileName);
            _running = true;
            return Download();
        }

        public void Pause()
        {
            _running = false;
        }

        public Task Resume()
        {
            return Start();
        }

        private async Task Download()
        {
            if (!_running)
                throw new InvalidOperationException();
            
            var file = Path.Combine(_file.Path, _file.FileName);
            if (_receivedBytes <= 0 && System.IO.File.Exists(file + ".downloading"))
            {
                var fileInfo = new FileInfo(file + ".downloading");
                _receivedBytes = fileInfo.Length; // Resume download from where it was left
            }
            
            var request = (HttpWebRequest) WebRequest.Create(_file.DownloadUri);
            request.Method = "GET";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
            request.AddRange(_receivedBytes);


            using (var response = await request.GetResponseAsync())
            {
                _totalBytes = response.ContentLength;
                using (var stream = response.GetResponseStream())
                {
                    if (stream == null)
                    {
                        OnDownloadCompleted(new DownloadCompleteArgs(_file, false));
                        throw new InvalidOperationException();
                    }
    
                    using (var fs = new FileStream(file + ".downloading", FileMode.Append, FileAccess.Write, FileShare.Read))
                    {
                        if (_receivedBytes > 0)
                            OnDownloadResumed(new DownloadResumeArgs(_file));
                        else 
                            OnDownloadStarted(new DownloadStartArgs(_file));
                        
                        var buffer = new byte[BufferSize];
                        int bytesRead;
                        
                        while (_running && (bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fs.WriteAsync(buffer, 0, bytesRead);
                            _receivedBytes += bytesRead;
                            OnDownloadProgressChanged(new DownloadProgressChangeArgs(_file, _receivedBytes, _totalBytes));
                        }
                        
                        await fs.FlushAsync();
                        fs.Close();
                    }
                }
            }

            if (_running)
            {
                System.IO.File.Move(file + ".downloading", file); // TODO: try-catch block for invalid names, permissions and file existance
                OnDownloadCompleted(new DownloadCompleteArgs(_file, true));
            }
            else 
                OnDownloadPaused(new DownloadPauseArgs(_file));
        }

        protected virtual void OnDownloadProgressChanged(DownloadProgressChangeArgs e)
        {
            DownloadProgressChanged?.Invoke(this, e);
        }

        protected virtual void OnDownloadStarted(DownloadStartArgs e)
        {
            DownloadStart?.Invoke(this, e);
        }

        protected virtual void OnDownloadPaused(DownloadPauseArgs e)
        {
            DownloadPause?.Invoke(this, e);
        }

        protected virtual void OnDownloadResumed(DownloadResumeArgs e)
        {
            DownloadResume?.Invoke(this, e);
        }

        protected virtual void OnDownloadCompleted(DownloadCompleteArgs e)
        {
            DownloadFinish?.Invoke(this, e);            
        }
        
        public event EventHandler<DownloadProgressChangeArgs> DownloadProgressChanged;
        public event EventHandler<DownloadStartArgs> DownloadStart;
        public event EventHandler<DownloadPauseArgs> DownloadPause;
        public event EventHandler<DownloadResumeArgs> DownloadResume;
        public event EventHandler<DownloadCompleteArgs> DownloadFinish;
    }
}