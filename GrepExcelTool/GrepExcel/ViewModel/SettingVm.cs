using System;

namespace GrepExcel.ViewModel
{
    public class SettingArgs : EventArgs
    {
        public SettingArgs()
        {

        }
        public int NumberRecent { get; set; }
    }

    public class SettingVm
    {
        private static SettingVm _instance = null;

        public event EventHandler SettingChanged;

        public SettingVm()
        {

        }

        static public SettingVm Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SettingVm();
                }
                return _instance;
            }
        }

        public void Notify(SettingArgs e)
        {
            OnSettingChanged(e);
        }

        protected virtual void OnSettingChanged(SettingArgs e)
        {
            SettingChanged?.Invoke(this, e);
        }


    }
}
