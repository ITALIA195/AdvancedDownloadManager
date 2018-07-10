using System;

namespace Test.AdvancedDownloadManager.Events
{
    public class DownloadCompleteArgs : EventArgs
    {
        public File File { get; }
        public bool Successful { get; }

        public DownloadCompleteArgs(File file, bool successful)
        {
            File = file;
            Successful = successful;
        }
    }
}