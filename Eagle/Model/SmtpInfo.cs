namespace Eagle.Model
{
    public class SmtpInfo : ModelBase
    {
        private string _host = "localhost";
        private int _port = 25;
        private string _userName;
        private string _password;
        private bool _enableSsl;
        
        public bool EnableSsl
        {
            get { return _enableSsl; }
            set
            {
                if (value.Equals(_enableSsl)) return;
                _enableSsl = value;
                OnPropertyChanged();
            }
        }

        public string Host
        {
            get { return _host; }
            set
            {
                if (value == _host) return;
                _host = value;
                OnPropertyChanged();
            }
        }

        public int Port
        {
            get { return _port; }
            set
            {
                if (value == _port) return;
                _port = value;
                OnPropertyChanged();
            }
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                if (value == _userName) return;
                _userName = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (value == _password) return;
                _password = value;
                OnPropertyChanged();
            }
        }
    }
}
