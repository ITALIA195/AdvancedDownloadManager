using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using AdvancedDownloadManager.Controls;

namespace AdvancedDownloadManager
{
    public partial class MainWindow
    {
        private StackPanel _downloadList;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            _downloadList = FindName("DownloadList") as StackPanel;
            Debug.Assert(_downloadList != null);
            AddDownloadEntry();
        }

        private void AddDownloadEntry()
        {
            var entry = new DownloadEntry(
                new FileProperties
                {
                    FileName = "5MB.zip",
                    DownloadUri =
                        "http://ipv4.download.thinkbroadband.com/5MB.zip",
                    Path = "D:/"
                });
//            entry.OpenFile += OnOpenFile;
//            entry.Pause += OnPause;
            _downloadList.Children.Add(entry);
        }

//        private void OnOpenFile(object sender, RoutedEventArgs e)
//        {
//            if (sender is DownloadEntry entry)
//            {
//                var file = Path.Combine(entry.File.Path, entry.File.FileName);
//                if (!File.Exists(file))
//                    return;
//
//                Process.Start(file); //TODO: Handle I/O exceptions 
//                e.Handled = true;
//            }
//        }
//
//        private void OnPause(object sender, RoutedEventArgs e)
//        {
//            if (sender is DownloadEntry entry)
//            {
//                
//                
//                e.Handled = true;
//            }
//        }
    }
}