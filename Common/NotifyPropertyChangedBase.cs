using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Common
{
    /// <summary>
    /// Base class for INotifyPropertyChanged support
    /// </summary>
    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        //Вызывается, если у свойства есть зависимые свойства.
        protected virtual void DependentPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
