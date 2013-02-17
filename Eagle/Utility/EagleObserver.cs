using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Eagle.Utility
{
    public class EagleObserver
    {
        private EagleConfigurationModel _model;
        private readonly IList<FileSystemWatcher> _watchers;

        public EagleObserver()
        {
            _model = EagleConfigurationModel.Current;
            _watchers = new List<FileSystemWatcher>();
        }

        public void Restart()
        {
            Reset();
            Start();
        }

        public void Start()
        {
            foreach (var dir in _model.Directories)
            {
                var w = new FileSystemWatcher(dir);

                if (_model.Directories.Any() && !string.IsNullOrWhiteSpace(_model.Exts.First()))
                {
                    w.Filter = _model.Exts.First();
                }

                w.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;

                EnableFileWatcherEvents(w);

                _watchers.Add(w);
            }
        }

        /// <summary>
        /// Restart observing dirs.
        /// </summary>
        private void Reset()
        {
            //release memory
            foreach (var watcher in _watchers)
            {
                DisableFileWatcherEvents(watcher);
            }

            _watchers.Clear();

            //Refresh the model getting new configuration
            _model = EagleConfigurationModel.Current;
        }

        private void EnableFileWatcherEvents(FileSystemWatcher watcher)
        {
            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Renamed += OnRenamed;

            watcher.EnableRaisingEvents = true;
        }

        private void DisableFileWatcherEvents(FileSystemWatcher watcher)
        {
            watcher.Changed -= OnChanged;
            watcher.Created -= OnChanged;
            watcher.Deleted -= OnChanged;
            watcher.Renamed -= OnRenamed;

            watcher.EnableRaisingEvents = false;
        }

        private void SendEmails(string body, string path)
        {
            if (_model.Emails.Any())
            {
                foreach (var email in _model.Emails)
                {
                    Utilities.SendEmail(email, path + " was updated.", body);
                }
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            string message = string.Format("File {0} was {1} at {2}", e.FullPath, e.ChangeType, DateTime.Now);
            SendEmails(message, e.FullPath);
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            string message = string.Format("File {0} was {1} at {2}", e.FullPath, e.ChangeType, DateTime.Now);
            SendEmails(message, e.FullPath); 
        }

    }
}