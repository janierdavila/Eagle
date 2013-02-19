using System;
using System.Collections.ObjectModel;
using System.IO;

using Eagle.Utility;

namespace Eagle.Model
{
    public class EagleConfigurationModel : ModelBase
    {
        static EagleConfigurationModel()
        {
            Xml = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml");
        }

        public EagleConfigurationModel()
        {
            Directories = new ObservableCollection<PathInfo>();
            Emails = new ObservableCollection<string>();
            SmtpInfo = new SmtpInfo();
        }

        private static readonly string Xml;

        private ObservableCollection<PathInfo> _directories;
        private ObservableCollection<string> _emails;
        private SmtpInfo _smtpInfo;
        private int _notificationDelay = 10;

        public int NotificationDelay
        {
            get { return _notificationDelay; }
            set
            {
                if (value == _notificationDelay) return;
                _notificationDelay = value;
                OnPropertyChanged();
            }
        }

        public SmtpInfo SmtpInfo
        {
            get { return _smtpInfo; }
            set
            {
                if (Equals(value, _smtpInfo)) return;
                _smtpInfo = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Emails
        {
            get { return _emails; }
            set
            {
                if (value == _emails) return;
                _emails = value;

                OnPropertyChanged();
            }
        }

        public ObservableCollection<PathInfo> Directories
        {
            get { return _directories; }
            set
            {
                if (Equals(value, _directories)) return;
                _directories = value;

                OnPropertyChanged();
            }
        }
        
        public static EagleConfigurationModel Current
        {
            get
            {
                if (!File.Exists(Xml))
                {
                    return new EagleConfigurationModel();
                }

                return SerializerHelper.XmlDeserialize<EagleConfigurationModel>(Xml);
            }
        }

        public async void Save()
        {
            await SerializerHelper.XmlSerialize(Xml, this);
        }
    }
}
