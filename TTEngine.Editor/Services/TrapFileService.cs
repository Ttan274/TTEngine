using System.IO;
using System.Text.Json;
using TTEngine.Editor.Dtos;
using TTEngine.Editor.Models.Trap;

namespace TTEngine.Editor.Services
{
    public static class TrapFileService
    {
        private static string FILE_NAME = "TrapDef.json";

        public static void Save(IEnumerable<TrapDefinition> traps)
        {
            var json = JsonSerializer.Serialize(traps, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(GetPath(), json);
        }

        public static List<TrapDefinition> Load()
        {
            var path = GetPath();
            if (!File.Exists(path))
                return new List<TrapDefinition>();

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<TrapDefinition>>(json);
        }

        private static string GetPath()
        {
            var path = EditorPaths.GetDataFolder();
            return Path.Combine(path, FILE_NAME);
        }

        //public static TrapDto ToDto(TrapDefinition trap)
        //{
        //    if(trap is FireTrapDefinition fire)
        //    {
        //        return new TrapDto
        //        {
        //            Id = fire.Id,
        //            ImagePath = fire.ImagePath,
        //            X = fire.X,
        //            Y = fire.Y,
        //            ActiveDuration = fire.ActiveDuration,
        //            InactiveDuration = fire.InactiveDuration,
        //            DamagePerSecond = fire.DamagePerSecond
        //        };
        //    }

        //    if(trap is SawTrapDefinition saw)
        //    {
        //        return new TrapDto
        //        {
        //            Id = saw.Id,
        //            ImagePath = saw.ImagePath,
        //            X = saw.X,
        //            Y = saw.Y,
        //            Speed = saw.Speed,
        //            Damage = saw.Damage,
        //            DamageCooldown = saw.DamageCooldown
        //        };
        //    }

        //    throw new InvalidOperationException("Unknown Trap Type");
        //}

        //public static TrapDefinition FromDto(TrapDto dto)
        //{
        //    if(dto.Id == "Fire")
        //    {
        //        return new FireTrapDefinition
        //        {
        //            Id = dto.Id,
        //            ImagePath = dto.ImagePath,
        //            X = dto.X,
        //            Y = dto.Y,
        //            ActiveDuration = dto.ActiveDuration ?? 1.0f,
        //            InactiveDuration = dto.InactiveDuration ?? 1.0f,
        //            DamagePerSecond = dto.DamagePerSecond ?? 10.0f
        //        };
        //    }

        //    if(dto.Id == "Saw")
        //    {
        //        return new SawTrapDefinition
        //        {
        //            Id = dto.Id,
        //            ImagePath = dto.ImagePath,
        //            X = dto.X,
        //            Y = dto.Y,
        //            Speed = dto.Speed ?? 60.0f,
        //            Damage = dto.Damage ?? 20.0f,
        //            DamageCooldown = dto.DamageCooldown ?? 0.5f
        //        };
        //    }

        //    throw new InvalidOperationException("Unknown trap dto type");
        //}
    }
}
