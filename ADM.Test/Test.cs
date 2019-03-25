using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using ADM.Core;

namespace ADM.Test
{
    public class Test
    {
        private readonly Queue<Downloader> _downloadQueue = new Queue<Downloader>();
        
        public Test()
        {
            FillQueue();
            Download();
        }

        private void Download()
        {
            if (_downloadQueue.Count <= 0) return;
            var downloader = _downloadQueue.Dequeue();
            downloader.PropertyChanged += OnPropertyChanged;          
            downloader.Start();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var downloader = sender as Downloader;
            Debug.Assert(downloader != null, $"{nameof(downloader)} != null");
//            Console.Clear();
            Console.CursorVisible = false;    
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"Downloading: {downloader.FileProperties.FileName}            ");
            Console.WriteLine($"Download speed: {GetReadableSpeed(downloader.DownloadSpeed)}       ");
            Console.WriteLine($"Progress: {downloader.Progress:00.00}%. {GetReadableSize(downloader.ReceivedBytes)} / {GetReadableSize(downloader.TotalBytes)}              ");
            Console.WriteLine($"State: {downloader.State}              ");

            if ((downloader.State != DownloadState.Running && downloader.Progress > 90))
                Download();
        }

        private static string GetReadableSpeed(double speed)
        {
            if (speed < 1000)
                return $"{speed:0} B/s";
            if (speed < 1572864) // < 1MB
                return $"{speed / 1024f:0} kB/s";
            return $"{speed / 1048576f:0} MB/s";
        }

        private static string GetReadableSize(long size)
        {
            if (size < 2048) // < 2 kB
                return $"{size} B";
            if (size < 2097152) // < 2 MB
                return $"{size / 1024f:0} kB";
            if (size < 1073741824) // < 1 GB
                return $"{size / 1048576f:0.0} MB";
            return $"{size / 1073741824f:0.0} GB";
        }
        
        private void FillQueue()
        {
            for (int i = 5; i <= 12; i++)
                _downloadQueue.Enqueue(new Downloader(new FileProperties
                {
                    FileName = $"Nisekoi2_Ep_{i:00}_SUB_ITA",
                    DownloadUri = $@"www.lacumpa.org/DDL/ANIME/Nisekoi2/Nisekoi2_Ep_{i:00}_SUB_ITA.mp4",
                    Path = @"D:\Animes\Nisekoi\Second Season"
                }));
        }
    }
}