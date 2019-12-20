using WpfBase.Interface;

namespace WpfBase.Bases
{
    public class ViewModelBase : NotifyPropertyBase, IViewModel
    {
        public bool IsGoodInit = true;
    }
}
