using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GrepExcel.View
{
    public class BaseModel : INotifyPropertyChanged
    {
        public event EventHandler Close;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected virtual void OnClose(EventArgs e)
        {
            Close?.Invoke(this, e);
        }

    }
}

