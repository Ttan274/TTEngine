using System.ComponentModel;

namespace TTEngine.Editor.Models.Level
{
    public class MapDefinition: INotifyPropertyChanged
    {
        private string _id;
        public string Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string p)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }
}
