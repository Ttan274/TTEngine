using System.Diagnostics;
using System.IO;

namespace TTEngine.Editor.EditorServices.EngineLauncher
{
    public class EngineLauncher
    {
        private Process _process;
        public event Action EngineExited;

        public bool IsRunning => _process != null && !_process.HasExited;

        public void Start(string exePath, Action<string> onLogReceived)
        {
            if (IsRunning)
                return;

            var info = new ProcessStartInfo
            {
                FileName = exePath,
                WorkingDirectory = Path.GetDirectoryName(exePath),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            //Setup Process
            _process = new Process();
            _process.StartInfo = info;
            _process.EnableRaisingEvents = true;

            //Data & Error Received Actions
            _process.OutputDataReceived += (s, e) =>
            {
                if(!string.IsNullOrEmpty(e.Data))
                    onLogReceived?.Invoke(e.Data);
            };

            _process.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    onLogReceived?.Invoke("[ERROR]" + e.Data);
            };

            //Engine Exit Action
            _process.Exited += (s, e) =>
            {
                onLogReceived?.Invoke("Engine Exited");
                EngineExited?.Invoke();
            };

            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
        }

        public void Stop()
        {
            if(IsRunning)
                _process.Kill();
        }
    }
}
