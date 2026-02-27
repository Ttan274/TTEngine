using System.Collections.ObjectModel;
using System.Reflection;
using TTEngine.Editor.Models.Editor;

namespace TTEngine.Editor.ViewModels.Panel
{
    public class PropertyFieldViewModel : ObservableObject
    {
        public object TargetObject { get; }
        public PropertyInfo Property { get; }
        public string DisplayName => Property.Name;
        public Type PropertyType => Property.PropertyType;
        public string StringValue
        {
            get => Property.GetValue(TargetObject)?.ToString();
            set
            {
                if(TryConvert(value, out var converted))
                {
                    Property.SetValue(TargetObject, converted);
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<DictionaryItemViewModel> DictionaryItems { get; }

        public PropertyFieldViewModel(object targetObject, PropertyInfo property)
        {
            TargetObject = targetObject;
            Property = property;

            if(IsStringDictionary)
            {
                var dict = (Dictionary<string, string>)Property.GetValue(targetObject);

                DictionaryItems = new ObservableCollection<DictionaryItemViewModel>(
                    dict.Select(kv => new DictionaryItemViewModel(dict, kv.Key, kv.Value)));
            }
        }

        //Helpers to determine property type for editor purposes
        public bool IsBool => PropertyType == typeof(bool);
        public bool IsEnum => PropertyType.IsEnum;
        public bool IsString => PropertyType == typeof(string);
        public bool IsNumber => 
            PropertyType == typeof(int)     ||
            PropertyType == typeof(double)  ||
            PropertyType == typeof(float);
        public bool IsStringDictionary =>
            PropertyType == typeof(Dictionary<string, string>);
         
        public Array EnumValues => IsEnum ? Enum.GetValues(PropertyType) : null;

        private bool TryConvert(string value, out object result)
        {
            result = null;

            try
            {
                if (PropertyType == typeof(int))
                {
                    if (int.TryParse(value, out var i))
                    {
                        result = i;
                        return true;
                    }
                    return false;
                }

                if (PropertyType == typeof(float))
                {
                    if (float.TryParse(value,
                        System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var f))
                    {
                        result = f;
                        return true;
                    }
                    return false;
                }

                if (PropertyType == typeof(double))
                {
                    if (double.TryParse(value,
                        System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var d))
                    {
                        result = d;
                        return true;
                    }
                    return false;
                }

                if (PropertyType == typeof(bool))
                {
                    if (bool.TryParse(value, out var b))
                    {
                        result = b;
                        return true;
                    }
                    return false;
                }

                if (PropertyType.IsEnum)
                {
                    if(Enum.TryParse(PropertyType, value, true, out var enumVal))
                    {
                        result = enumVal;
                        return true;
                    }
                    return false;
                }

                result = value;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
