namespace Eagle.Model
{
    public class PathInfo : ModelBase
    {
        protected bool Equals(PathInfo other)
        {
            return string.Equals(_fileName, other._fileName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PathInfo) obj);
        }

        public override int GetHashCode()
        {
            return _fileName.GetHashCode();
        }

        private string _fileName;
        private string _filter;

        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (value == _fileName) return;
                _fileName = value;
                OnPropertyChanged();
            }
        }

        public string Filter
        {
            get { return _filter; }
            set
            {
                if (value == _filter) return;
                _filter = value;
                OnPropertyChanged();
            }
        }

        
    }
}