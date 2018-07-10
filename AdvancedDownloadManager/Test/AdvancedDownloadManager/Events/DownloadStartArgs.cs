using System;

namespace Test.AdvancedDownloadManager.Events
{
    public class DownloadStartArgs : EventArgs
    {
        public File File { get; }

        public DownloadStartArgs(File file)
        {
            File = file;
        }
    }
}