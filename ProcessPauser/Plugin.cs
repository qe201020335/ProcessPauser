using System.Collections.Generic;
using System.Diagnostics;
using IPA;
using IPALogger = IPA.Logging.Logger;

namespace ProcessPauser
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        private Stack<Process> _processes = new Stack<Process>();

        [Init]
        public void Init(IPALogger logger)
        {
            Instance = this;
            Log = logger;
            Log.Info("ProcessPauser initialized.");
        }

        #region BSIPA Config

        //Uncomment to use BSIPA's config
        /*
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
        }
        */

        #endregion

        [OnStart]
        public void OnApplicationStart()
        {
            Log.Debug("OnApplicationStart");
            PauseProcess();
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Log.Debug("OnApplicationQuit");
            UnPauseProcess();
        }

        private void PauseProcess()
        {
            var p = Process.GetProcessesByName("wallpaper64.exe");
            foreach (var process in p)
            {
                Log.Info($"Pause {process.ProcessName}, pid {process.Id}");
                process.Suspend();
                _processes.Push(process);
            }
        }

        private void UnPauseProcess()
        {
            while (_processes.Count > 0)
            {
                var process = _processes.Pop();
                Log.Info($"Unpause {process.ProcessName}, pid {process.Id}");
                process.Resume();
            }
        }
    }
}