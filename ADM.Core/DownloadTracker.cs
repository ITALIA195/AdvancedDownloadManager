using System.Diagnostics;

namespace ADM.Core
{
    public class DownloadTracker
    {
        private readonly Stopwatch _stopwatch;
        private DownloadState _state;

        private long _receivedBytes;
        private long _totalBytes;
        
        private long _bytesSinceUpdate;
        private long _lastUpdate;
        private double _downloadSpeed;

        public DownloadTracker()
        {
            _stopwatch = new Stopwatch();
        }

        public void Start()
        {
            _stopwatch.Reset();
            _stopwatch.Start();

            _bytesSinceUpdate = 0;
            _lastUpdate = 0;
            _downloadSpeed = 0;
        }

        public void Receive(long bytes)
        {
            _receivedBytes += bytes;
        }

        public void AddPartialContent(long downloadedBytes)
        {
            _totalBytes += downloadedBytes;
            _receivedBytes = downloadedBytes;
        }

        public void SetContentLength(long contentLength)
        {
            _totalBytes = contentLength;
        }

        public double Progress => ReceivedBytes / (double) TotalBytes * 100;

        public long ReceivedBytes => _receivedBytes;
        public long TotalBytes => _totalBytes;

        public double Speed => _downloadSpeed;
    }
}