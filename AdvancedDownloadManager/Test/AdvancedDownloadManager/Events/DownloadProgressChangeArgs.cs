using System;

namespace Test.AdvancedDownloadManager.Events
{
    public class DownloadProgressChangeArgs : EventArgs
    {
        public File File { get; }
        public bool HasTotalBytes => TotalBytes != -1;
        public long ReceivedBytes { get; }
        public long TotalBytes { get; }
        
        public DownloadProgressChangeArgs(File file, long receivedBytes, long totalBytes)
        {
            File = file;
            ReceivedBytes = receivedBytes;
            TotalBytes = totalBytes;
        }
    }
}