namespace WpfBase.Bases
{
    using System.ComponentModel;
    using System.Windows.Controls;
    using WpfBase.Interface;

    public abstract class ViewBase : UserControl, IView, ISupportInitialize
    {
        public string Title;
        public override abstract void BeginInit();
        public override abstract void EndInit();
    }

}
