using System.Globalization;
using System.Windows.Data;
using TTEngine.Editor.Models.Asset;

namespace TTEngine.Editor.Converter
{
    public class AssetIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not AssetType type)
                return "📄";

            return type switch
            {
                AssetType.Folder => "📁",
                AssetType.Json => "📄",
                AssetType.Texture => "🖼",
                AssetType.Font => "🔤",
                _ => "📦"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
