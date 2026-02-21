namespace TTEngine.Editor.Models.Project
{
    public class ProjectInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
