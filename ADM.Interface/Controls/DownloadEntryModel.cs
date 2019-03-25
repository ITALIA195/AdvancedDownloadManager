namespace AdvancedDownloadManager.Controls
{
    public static class DownloadEntryModel
    {
        public static string GetReadableSize(long size)
        {
            if (size < 2048) // < 2 kB
                return $"{size} B";
            if (size < 2097152) // < 2 MB
                return $"{size / 1024f:0} kB";
            if (size < 1073741824) // < 1 GB
                return $"{size / 1048576f:0.0} MB";
            return $"{size / 1073741824f:0.0} GB";
        } 
    }
}