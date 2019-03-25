using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;
using ADM.Core;
using JetBrains.Annotations;

// ReSharper disable MemberCanBePrivate.Global UnusedMember.Global
namespace ADM.Interface
{
    public sealed class DownloadEntryViewModel : INotifyPropertyChanged
    {
        public readonly Downloader Downloader; //TODO: Make it private
        private readonly FileProperties _file;

        public DownloadEntryViewModel(FileProperties file)
        {
            _file = file;
            Downloader = new Downloader(file);
            Downloader.PropertyChanged += OnDownloaderPropertyChanged;
            Downloader.Start();
        }

        private void OnDownloaderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Downloader.DownloadSpeed):
                    NotifyChanged(nameof(DownloadInfo));
                    NotifyChanged(nameof(ETA));
                    break;
                case nameof(Downloader.Progress):
                    NotifyChanged(nameof(DownloadInfo));
                    NotifyChanged(nameof(ETA));
                    NotifyChanged(nameof(Progress));
                    break;
                case nameof(Downloader.State):
                    NotifyChanged(nameof(ProgressBarColor));
                    NotifyChanged(nameof(IsCompleted));
                    NotifyChanged(nameof(IsDownloading));
                    NotifyChanged(nameof(IsRemovable));
                    NotifyChanged(nameof(HasFailed));
                    NotifyChanged(nameof(IsPaused));
                    break;
            }
            Console.WriteLine($"Progress: {Progress:0.00}");
            Console.WriteLine($"Speed: {Downloader.DownloadSpeed}");
            Console.WriteLine($"State: {Downloader.State}");
        }

        public string FileName => $"{_file.FileName}.{_file.Extension}";
        public string DownloadUri => _file.DownloadUri;

        public string ETA
        {
            get
            {
                if (Downloader.TotalBytes <= 0)
                    return "Unknown time left";

                var timeLeft = Downloader.TotalBytes / Downloader.DownloadSpeed;
                return $"{GetTime(timeLeft)} remaining.";
            }
        }

        public string DownloadInfo
        {
            get
            {
                StringBuilder builder = new StringBuilder(40);
                builder.Append(GetReadableSpeed(Downloader.DownloadSpeed));
                builder.Append(" - ");
                builder.Append(GetReadableSize(Downloader.ReceivedBytes));
                if (Downloader.TotalBytes > 0)
                {
                    builder.Append(" of ");
                    builder.Append(GetReadableSize(Downloader.TotalBytes));
                }
                return builder.ToString();
            }
        }

        public static string GetTime(double time)
        {
            if (time < 90)
                return $"{time:0} seconds";
            if (time < 3600)
                return $"{time / 60:0} minutes";
            if (time < 86400)
                return $"{time / 3600:0} hours";
            return $"{time / 216000:0} days";
        }

        public static string GetReadableSpeed(double speed)
        {
            if (speed < 1000)
                return $"{speed:0} B/s";
            if (speed < 1572864) // < 1MB
                return $"{speed / 1024f:0} kB/s";
            return $"{speed / 1048576f:0} MB/s";
        }

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
        
        public bool IsCompleted => Downloader.State == DownloadState.Completed;
        public bool IsDownloading => Downloader.State == DownloadState.Running || IsPaused;
        public bool IsPaused => Downloader.State == DownloadState.Paused;
        public bool HasFailed => Downloader.State == DownloadState.Failed;
        public bool IsRemovable => IsCompleted || HasFailed;

        public SolidColorBrush ProgressBarColor => Downloader.State == DownloadState.Paused
            ? new SolidColorBrush(Color.FromArgb(255, 61, 249, 243))
            : new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
        
        public double Progress => Downloader.Progress;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void NotifyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}