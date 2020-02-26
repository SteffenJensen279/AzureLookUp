using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CheckAzureColumns.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {

        #region PropertyChangedStuff
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public bool SetAndInvoke<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
                return false;
            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
        #endregion PropertyChangedStuff
    }
}
