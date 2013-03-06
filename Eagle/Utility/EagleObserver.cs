using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using Eagle.Model;

namespace Eagle.Utility
{
    public class EagleObserver
    {
        private EagleConfigurationModel _model;
        private readonly IList<FileSystemWatcher> _watchers;

        private readonly IList<string> _messages;
        private DateTime _lastTimeEmailWasSent;

        private DispatcherTimer _timer;

        //Used to sync a timer
        //since this class is called from the UI thread
        private readonly Dispatcher _dispatcher;

        public EagleObserver(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;

            _model = EagleConfigurationModel.Current;
            _watchers = new List<FileSystemWatcher>();
            _messages = new List<string>();
            _lastTimeEmailWasSent = DateTime.MinValue;
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
                try
                {
                    var w = new FileSystemWatcher(dir.FileName);
                    w.Filter = dir.Filter;

                    w.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName |
                                     NotifyFilters.DirectoryName;
                    w.IncludeSubdirectories = true;

                    EnableFileWatcherEvents(w);

                    _watchers.Add(w);
                }
                catch (ArgumentException)
                { 
                    /*ignore*/
                }
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
            watcher.EnableRaisingEvents = false;

            watcher.Changed -= OnChanged;
            watcher.Created -= OnChanged;
            watcher.Deleted -= OnChanged;
            watcher.Renamed -= OnRenamed;
        }

        private void StartTimer()
        {
            if (_timer == null)
            {
                _timer = new DispatcherTimer(DispatcherPriority.Normal, _dispatcher);
                _timer.Tick += TimerOnTick;
                _timer.Interval = TimeSpan.FromSeconds(_model.NotificationDelay);
            }

            //Only start the timer if it is not already running
            if (!_timer.IsEnabled)
            {
                _timer.Start();
            }
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            _timer.Stop();

            //Send summary
            SendEmails(string.Empty);
        }

        private void SendEmails(string body)
        {
            //Dont do anything if there are no emails setup
            if (!_model.Emails.Any()) return;

            //Only cache summary if it is not the first time
            if (_lastTimeEmailWasSent != DateTime.MinValue)
            {
                var timespan = DateTime.Now - _lastTimeEmailWasSent;

                if (timespan.TotalSeconds < _model.NotificationDelay)
                {
                    //Schedule next email.
                    //Everything that happens while we hold on, save it for summary
                    //Also only allow one notification per file
                    if (!_messages.Contains(body))
                    {
                        _messages.Add(body);
                    }

                    //This is harmless even when it is called several times
                    //it will only start once!
                    StartTimer();

                    return;
                }
            }

            //Add this last message and get the summary
            _messages.Add(body);
            string summary = GetSummary();

            //This can be done at the end too...but I really wanted to set it ASAP
            //so this method doesnt get very chatty
            _lastTimeEmailWasSent = DateTime.Now;

            foreach (var email in _model.Emails)
            {
                Utilities.SendEmail(email, "These files you were monitoring was/were updated.", summary, _model.SmtpInfo);
            }

            //Since these messages were already sent...
            _messages.Clear();
        }

        private string GetSummary()
        {
            if (_messages.Count <= 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            foreach (var message in _messages)
            {
                sb.AppendFormat("{0}<br />", message);
            }

            return sb.ToString();
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            string message = string.Format("File {0} was {1} at {2}", e.FullPath, e.ChangeType, DateTime.Now);
            SendEmails(message);
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            string message = string.Format("File {0} was {1} at {2}", e.FullPath, e.ChangeType, DateTime.Now);
            SendEmails(message);
        }

    }
}