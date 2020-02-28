using Common;

namespace MusicStoreKeeper.Vmv.ViewModel
{
}

public abstract class TreeViewItemWrap<T> : NotifyPropertyChangedBase
{
    public abstract T Value { get; set; }

    public abstract string Icon { get; }

    private bool _isChecked;

    public virtual bool IsChecked
    {
        get => _isChecked;
        set
        {
            _isChecked = value;
            OnPropertyChanged();
        }
    }
}