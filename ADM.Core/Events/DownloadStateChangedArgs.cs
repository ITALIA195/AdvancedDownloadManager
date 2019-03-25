using System;

namespace AdvancedDownloadManager.Events
{
    public class DownloadStateChangedArgs : EventArgs
    {
        public DownloadState State { get; }

        public DownloadStateChangedArgs(DownloadState state)
        {
            State = state;
        }
    }
}