using TTEngine.Editor.Converter;
using TTEngine.Editor.Dtos;
using TTEngine.Editor.Models.Level;

namespace TTEngine.Editor.Services
{
    public class LevelService
    {
        private readonly JsonRepository<LevelFileDto> _repository;
        public LevelService()
        {
            _repository = new JsonRepository<LevelFileDto>(EditorPaths.LevelDefs);
        }

        public List<LevelDefinition> Load()
        {
            LevelFileDto dto = new LevelFileDto();    //_repository.Get();
           return LevelMapper.FromDto(dto);
        }

        public void Save(IEnumerable<LevelDefinition> levels)
        {
            var dto = LevelMapper.ToDto(levels);
           // _repository.Save(dto);
        }
    }
}
