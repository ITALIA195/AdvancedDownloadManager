using System.Text.RegularExpressions;

namespace ADM.Core
{
    public struct FileProperties
    {
        private string _filePath;
        
        public string FileName { get; set; }
        public string Extension => Regex.Match(DownloadUri, @"[^?]+\.(\w+)", RegexOptions.None).Groups[1].Value;
        public string DownloadUri { get; set; }

        public string Path
        {
            get => _filePath;
            set
            {
                _filePath = value;
                if (!_filePath.EndsWith("/") && !_filePath.EndsWith("\\"))
                    _filePath += "/";
            }
        }
    }
}
