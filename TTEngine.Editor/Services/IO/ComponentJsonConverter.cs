using System.Text.Json;
using System.Text.Json.Serialization;
using TTEngine.Editor.Models.Component;

namespace TTEngine.Editor.Services.IO
{
    public class ComponentJsonConverter
        : JsonConverter<ComponentBase>
    {
        public override ComponentBase Read(ref Utf8JsonReader reader, 
            Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            var root = document.RootElement;

            if (!root.TryGetProperty("Type", out var typeProp))
                throw new Exception("Component missing Type property");

            var type = typeProp.GetString();

            Type targetType = type switch
            {
                "Transform" => typeof(TransformComponent),
                "Animator" => typeof(AnimatorComponent),
                "EntityStats" => typeof(EntityComponent),
                "Interactable" => typeof(InteractableComponent),
                "Trap" => typeof(TrapComponent),
                _ => throw new Exception($"Unknonw component type : {type}") 
            };

            var json = root.GetRawText();
            return (ComponentBase)JsonSerializer.Deserialize(json, targetType, options);
        }

        public override void Write(Utf8JsonWriter writer, 
            ComponentBase value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
        }
    }
}
