using System;

namespace Test.AdvancedDownloadManager.Events
{
    public class DownloadResumeArgs : EventArgs
    {
        public File File { get; }
        
        public DownloadResumeArgs(File file)
        {
            File = file;
        }
    }
}