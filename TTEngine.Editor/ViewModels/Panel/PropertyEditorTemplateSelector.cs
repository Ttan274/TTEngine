using System.Windows;
using System.Windows.Controls;

namespace TTEngine.Editor.ViewModels.Panel
{
    public class PropertyEditorTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BoolTemplate { get; set; }
        public DataTemplate EnumTemplate { get; set; }
        public DataTemplate StringTemplate { get; set; }
        public DataTemplate NumberTemplate { get; set; }
        public DataTemplate DictionaryTemplate {  get; set; }
        
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if(item is not PropertyFieldViewModel field)
                return base.SelectTemplate(item, container);

            if(field.IsBool)
                return BoolTemplate;

            if(field.IsEnum)
                return EnumTemplate;

            if(field.IsNumber)
                return NumberTemplate;

            if(field.IsStringDictionary)
                return DictionaryTemplate;

            return StringTemplate;
        }
    }
}
