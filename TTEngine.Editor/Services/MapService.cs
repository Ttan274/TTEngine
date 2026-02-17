using System.IO;
using TTEngine.Editor.Converter;
using TTEngine.Editor.Dtos;
using TTEngine.Editor.Models.Tile;

namespace TTEngine.Editor.Services
{
    public class MapService
    {
        public TileMapModel Load(string mapId)
        {
            var path = EditorPaths.GetMapPath(mapId);
            var repo = new JsonRepository<TileMapData>(path);

            var dto = repo.Get();
            if (dto == null)
                return null;

            return MapMapper.FromDto(dto);
        }

        public void Save(string mapId, TileMapModel model)
        {
            var path = EditorPaths.GetMapPath(mapId);
            var repo = new JsonRepository<TileMapData>(path);

            var dto = MapMapper.ToDto(model);
            repo.Save(dto);
        }

        public void Delete(string mapId)
        {
            var path = EditorPaths.GetMapPath(mapId);
            if(File.Exists(path))
                File.Delete(path);
        }

        public bool Exists(string mapId)
        {
            var path = EditorPaths.GetMapPath(mapId);
            return File.Exists(path);
        }
    }
}
