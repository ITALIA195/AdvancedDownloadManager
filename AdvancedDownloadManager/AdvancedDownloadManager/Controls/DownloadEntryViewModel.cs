using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using AdvancedDownloadManager.Annotations;
using AdvancedDownloadManager.Events;

// ReSharper disable MemberCanBePrivate.Global UnusedMember.Global
namespace AdvancedDownloadManager.Controls
{
    public sealed class DownloadEntryViewModel : INotifyPropertyChanged
    {
        public readonly Downloader Downloader; //TODO: Make it private
        private readonly FileProperties _file;

        public DownloadEntryViewModel(FileProperties file)
        {
            ConsoleManager.Show();
            _file = file;
            Downloader = new Downloader(file);
            Downloader.PropertyChanged += OnDownloaderPropertyChanged;
            Downloader.Start();
        }

        private void OnDownloaderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Downloader.Progress):
                    NotifyChanged(nameof(Progress));
                    Console.WriteLine($"Progress: {Downloader.Progress}");
                    break;
                case nameof(Downloader.State):
                    NotifyChanged(nameof(IsDownloading));
                    NotifyChanged(nameof(IsPaused));
                    Console.WriteLine($"State: {Downloader.State}: Pause: {IsPaused}; Downloading: {IsDownloading}");
                    break;
            }
        }

        public string FileName => _file.FileName;
        public string DownloadUri => _file.DownloadUri;

        public bool IsDownloading => Downloader.State == DownloadState.Running;
        public bool IsPaused => Downloader.State == DownloadState.Paused;
        public double Progress => Downloader.Progress;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void NotifyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}