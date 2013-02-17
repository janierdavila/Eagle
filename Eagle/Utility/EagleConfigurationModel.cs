using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Eagle.Annotations;

namespace Eagle.Utility
{
    public class EagleConfigurationModel : INotifyPropertyChanged
    {
        static EagleConfigurationModel()
        {
            Xml = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml");
        }

        public EagleConfigurationModel()
        {
            Directories = new ObservableCollection<string>();
            Emails = new ObservableCollection<string>();
            Exts = new ObservableCollection<string>();
        }

        private static readonly string Xml;

        private ObservableCollection<string> _directories;
        private ObservableCollection<string>  _exts;
        private ObservableCollection<string> _emails;

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


        public ObservableCollection<string> Directories
        {
            get { return _directories; }
            set
            {
                if (Equals(value, _directories)) return;
                _directories = value;

                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Exts
        {
            get { return _exts; }
            set
            {
                if (value == _exts) return;
                _exts = value;

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

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
