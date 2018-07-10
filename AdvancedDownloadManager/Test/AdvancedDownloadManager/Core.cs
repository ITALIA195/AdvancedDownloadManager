using System;
using System.Runtime.InteropServices;
using Test.AdvancedDownloadManager.Events;

namespace Test.AdvancedDownloadManager
{
    public class Core
    {
        public Core()
        {
            var downloader = new Downloader(new File
            {
                FileName = "DarlingInTheFranXX_Ep_24_SUB_ITA.mp4",
                DownloadUri = @"http://www.unluckypeople.org/DDL/ANIME/DarlingInTheFranXX/DarlingInTheFranXX_Ep_24_SUB_ITA.mp4",
                Path = @"D:\"
            });

            downloader.DownloadStart += OnDownloadStarted;
            downloader.DownloadResume += OnDownloadResumed;
            downloader.DownloadPause += OnDownloadPaused;
            downloader.DownloadProgressChanged += OnDownloadProgressChanged;
            downloader.DownloadFinish += OnDownloadFinish;

            downloader.Start();
            
            
            while (true)
            {
                var line = Console.ReadLine();
                if (line == "pause")
                    downloader.Pause();
                else
                {
                    downloader.Resume();
                }
               
            }
        }

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangeArgs e)
        {
            if (e.HasTotalBytes)
            {
                Console.WriteLine($"Progress of {e.File.FileName}: {Math.Floor(e.ReceivedBytes/(float)e.TotalBytes*100)}% ({e.ReceivedBytes}/{e.TotalBytes})");
            }
            else
            {
                Console.WriteLine($"Received {e.ReceivedBytes} of file {e.File.FileName}");
            }
        }
        
        private void OnDownloadStarted(object sender, DownloadStartArgs e)
        {
            Console.WriteLine($"Started downloading {e.File.FileName}");
        }

        private void OnDownloadPaused(object sender, DownloadPauseArgs e)
        {
            Console.WriteLine($"Download of {e.File.FileName} paused succesfully");
        }

        private void OnDownloadResumed(object sender, DownloadResumeArgs e)
        {
            Console.WriteLine($"Download of {e.File.FileName} resumed succesfully");
        }
        
        private void OnDownloadFinish(object sender, DownloadCompleteArgs e)
        {
            Console.WriteLine($"Download of {e.File.FileName} finished {(e.Successful ? "succesfully" : "unsuccesfully")}");
        }
    }
}