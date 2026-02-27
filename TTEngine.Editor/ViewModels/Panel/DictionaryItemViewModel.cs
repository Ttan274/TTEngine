using TTEngine.Editor.Models.Editor;

namespace TTEngine.Editor.ViewModels.Panel
{
    public class DictionaryItemViewModel : ObservableObject
    {
        private readonly Dictionary<string, string> _source;

        private string _key;
        public string Key
        {
            get => _key;
            set
            {
                if (_key == value) return;

                var val = _source[_key];
                _source.Remove(_key);
                _source[value] = val;

                _key = value;
                OnPropertyChanged();
            }
        }

        private string _value;
        public string Value
        {
            get => _value;
            set
            {
                if (_value == value) return;

                _value = value;
                _source[_key] = value;
                OnPropertyChanged();
            }
        }

        public DictionaryItemViewModel(Dictionary<string, string> source, string key, string value)
        {
            _source = source;
            _key = key;
            _value = value;
        }
    }
}
