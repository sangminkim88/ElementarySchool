namespace Settings.ViewModels
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using WpfBase.Bases;
    using WpfBase.Common;
    using WpfBase.Managers;

    public class SettingsMainVM : ViewModelBase
    {
        #region Constructors

        public SettingsMainVM()
        {
            ShowView = new RelayCommand(ExecuteShowView);
        }

        #endregion

        #region Properties

        public ICommand ShowView { get; private set; }

        #endregion

        #region Methods

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            ViewModelManager.AddValue(typeof(SettingsMainVM), this);
        }

        private void ExecuteShowView(object o)
        {
            var values = (object[])o;
            Grid grid = (Grid)values[0];
            string name = (string)values[1];
            bool isExist = false;
            foreach (Grid item in grid.Children)
            {
                Grid child = item as Grid;
                if (item.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    child.Visibility = Visibility.Visible;
                    isExist = true;
                }
                else
                {
                    child.Visibility = Visibility.Collapsed;
                }
            }

            if (!isExist)
            {
                Grid view = Activator.CreateInstance(Type.GetType("Settings.Views." + name)) as Grid;
                grid.Children.Add(view);
            }
        }

        #endregion
    }
}
