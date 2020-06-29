namespace GAIA2020
{
    using GAIA2020.Enums;
    using GAIA2020.Utilities;
    using Manager;
    using System;
    using System.Threading;
    using System.Windows;

    /// <summary>
    /// App.xaml에 대한 상호 작용 논리.
    /// </summary>
    public partial class App : Application
    {
        #region Fields

        /// <summary>
        /// Defines the DEFAULT_PATH.
        /// </summary>
        public static string WORKING_DIRECTORY;

        /// <summary>
        /// Defines the mutex.
        /// </summary>
        private Mutex mutex = null;

        #endregion

        #region Methods

        /// <summary>
        /// The GetLogStyleManager.
        /// </summary>
        /// <returns>The <see cref="LogStyleManager"/>.</returns>
        public static LogStyleManager GetLogStyleManager()
        {
            string keyName = "Logs";

            if (Current.Resources.Contains(keyName))
            {
                return Current.FindResource(keyName) as LogStyleManager;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// The GetViewManager.
        /// </summary>
        /// <returns>The <see cref="ViewManager"/>.</returns>
        public static ViewManager GetViewManager()
        {
            string keyName = "Views";

            if (Current.Resources.Contains(keyName))
            {
                return Current.FindResource(keyName) as ViewManager;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// The GetViewModelManager.
        /// </summary>
        /// <returns>The <see cref="ViewModelManager"/>.</returns>
        public static ViewModelManager GetViewModelManager()
        {
            string keyName = "Locator";

            if (Current.Resources.Contains(keyName))
            {
                return Current.FindResource(keyName) as ViewModelManager;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// The Application_Startup.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="StartupEventArgs"/>.</param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string mutexName = "GAIA";
            bool isCreatedNew = false;
            try
            {
                mutex = new Mutex(true, mutexName, out isCreatedNew);
                if (!isCreatedNew)
                {
                    NotifyHelper.Instance.Show(NotifyHelper.NotiType.Error, "Application already started.");
                    Shutdown();
                }
            }
            catch (Exception ex)
            {
                NotifyHelper.Instance.Show(NotifyHelper.NotiType.Error, ex.Message + "\n\n" + ex.StackTrace + "\n\n" + "Application Existing...");
                Shutdown();
            }

            ConfigHelper.MakeConfig();

            WORKING_DIRECTORY = ConfigHelper.ReadProfileString(eConfigSection.User, eConfigKey.Directory, ConfigHelper.GetDefaultDataFilePath());
        }

        #endregion
    }
}
