using System;

namespace ADM.Core.Events
{
    public class DownloadProgressChangedArgs : EventArgs
    {
        public FileProperties File { get; }
        public bool HasTotalBytes => TotalBytes != -1;
        public long ReceivedBytes { get; }
        public long TotalBytes { get; }
        
        public DownloadProgressChangedArgs(FileProperties file, long receivedBytes, long totalBytes)
        {
            File = file;
            ReceivedBytes = receivedBytes;
            TotalBytes = totalBytes;
        }
    }
}