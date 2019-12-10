namespace Settings.ViewModels
{
    using Common;
    using FileManager;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using WpfBase.Bases;
    using WpfBase.Common;
    using WpfBase.Managers;

    public class SettingsMainVM : ViewModelBase
    {
        private Grid grid;
        #region Constructors

        public SettingsMainVM(Grid grid)
        {
            this.grid = grid;

            ShowView = new RelayCommand(ExecuteShowView);

            string viewName = string.Empty;
            viewName = ConfigManager.ReadProfileString(EConfigSection.Settings.ToString(), EConfigKey.LastView.ToString(), viewName);

            if (!viewName.Equals(string.Empty))
            {
                this.ExecuteShowView(viewName);
            }
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
            string name = o as string;
            bool isExist = false;
            foreach (Grid childGrid in this.grid.Children)
            {
                if (childGrid.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    childGrid.Visibility = Visibility.Visible;
                    isExist = true;

                    ConfigManager.WriteProfileString(EConfigSection.Settings.ToString(), EConfigKey.LastView.ToString(), name);
                }
                else
                {
                    childGrid.Visibility = Visibility.Collapsed;
                }
            }

            if (!isExist)
            {
                Grid view = Activator.CreateInstance(Type.GetType("Settings.Views." + name)) as Grid;
                this.grid.Children.Add(view);
                ConfigManager.WriteProfileString(EConfigSection.Settings.ToString(), EConfigKey.LastView.ToString(), name);
            }
        }

        #endregion
    }
}
