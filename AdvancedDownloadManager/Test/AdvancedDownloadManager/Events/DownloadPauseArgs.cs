using System;

namespace Test.AdvancedDownloadManager.Events
{
    public class DownloadPauseArgs : EventArgs
    {
        public File File { get; }
        
        public DownloadPauseArgs(File file)
        {
            File = file;
        }
    }
}