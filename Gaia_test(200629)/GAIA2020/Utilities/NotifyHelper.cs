namespace GAIA2020.Utilities
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using ToastNotifications;
    using ToastNotifications.Lifetime;
    using ToastNotifications.Messages;
    using ToastNotifications.Position;

    /// <summary>
    /// Defines the <see cref="NotifyHelper" />.
    /// </summary>
    public class NotifyHelper
    {
        #region Constants

        /// <summary>
        /// Defines the LOG_EXTENSION.
        /// </summary>
        private const string LOG_EXTENSION = ".log";

        #endregion

        #region Fields

        /// <summary>
        /// Defines the lazy.
        /// </summary>
        private static readonly Lazy<NotifyHelper> lazy =
            new Lazy<NotifyHelper>(() => new NotifyHelper());

        /// <summary>
        /// Defines the duration.
        /// </summary>
        private double duration = 3;

        /// <summary>
        /// Defines the logFileDirectory.
        /// </summary>
        private string logFileDirectory = string.Empty;

        /// <summary>
        /// Defines the notifier.
        /// </summary>
        private Notifier notifier;

        /// <summary>
        /// Defines the window.
        /// </summary>
        private Window window;

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="NotifyHelper"/> class from being created.
        /// </summary>
        private NotifyHelper()
        {
            this.window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: this.window,
                    corner: Corner.BottomRight,
                    offsetX: 10,
                    offsetY: 10);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(this.duration),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(30));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
        }

        #endregion

        #region Enums

        /// <summary>
        /// Defines the NotiType.
        /// </summary>
        public enum NotiType
        {
            /// <summary>
            /// Defines the Error.
            /// </summary>
            Error,
            /// <summary>
            /// Defines the Information.
            /// </summary>
            Information,
            /// <summary>
            /// Defines the Success.
            /// </summary>
            Success,
            /// <summary>
            /// Defines the Warning.
            /// </summary>
            Warning,
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Instance.
        /// </summary>
        public static NotifyHelper Instance
        {
            get { return lazy.Value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The SetWindow.
        /// </summary>
        /// <param name="duration">The duration<see cref="double"/>.</param>
        public void SetDuration(double duration)
        {
            this.duration = duration;
            notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: this.window,
                    corner: Corner.BottomRight,
                    offsetX: 10,
                    offsetY: 10);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(this.duration),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(30));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
        }

        /// <summary>
        /// The SetLogFilePath.
        /// </summary>
        /// <param name="logFileDirectory">The logFilePath<see cref="string"/>.</param>
        public void SetLogFileDirectory(string logFileDirectory)
        {
            this.logFileDirectory = logFileDirectory;
        }

        /// <summary>
        /// The Show.
        /// </summary>
        /// <param name="type">노티피케이션의 타입<see cref="NotiType"/>.</param>
        /// <param name="message">사용자에게 노출될 메시지<see cref="string"/>.</param>
        /// <param name="logMessge">로그파일에 남겨질 메시지 (default "")<see cref="string"/>.</param>
        /// <param name="writeLog">로그파일에 남길지 유무 (default true)<see cref="bool"/>.</param>
        public void Show(NotiType type, string message, string logMessge = "", bool writeLog = true)
        {
            Window currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            if (this.window == null && currentWindow == null) return;
            if (this.window != currentWindow)
            {
                this.SetWindow(currentWindow);
            }

            switch (type)
            {
                case NotiType.Error:
                    notifier.ShowError(message);
                    break;
                case NotiType.Information:
                    notifier.ShowInformation(message);
                    break;
                case NotiType.Success:
                    notifier.ShowSuccess(message);
                    break;
                case NotiType.Warning:
                    notifier.ShowWarning(message);
                    break;
                default:
                    break;
            }

            if (writeLog && !this.logFileDirectory.Equals(string.Empty))
            {
                if (!Directory.Exists(this.logFileDirectory))
                {
                    Directory.CreateDirectory(this.logFileDirectory);
                }
                string logFilePath = Path.Combine(this.logFileDirectory, DateTime.Now.ToString("yyMMdd") + LOG_EXTENSION);

                if (!File.Exists(logFilePath))
                {
                    File.Create(logFilePath).Dispose();
                }
                StringBuilder sb = new StringBuilder();
                sb.Append(DateTime.Now.ToString("HH:mm:ss"));
                sb.Append(" (");
                sb.Append(type.ToString());
                sb.Append(") :\t");
                if (!logMessge.Equals(string.Empty))
                {
                    sb.Append("#");
                    sb.Append(logMessge);
                    sb.Append("#\t");
                }
                sb.AppendLine(message);
                File.AppendAllText(logFilePath, sb.ToString());
            }
        }

        /// <summary>
        /// The SetWindow.
        /// </summary>
        /// <param name="parent">The parent<see cref="Window"/>.</param>
        private void SetWindow(Window parent)
        {
            this.window = parent;
            notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: this.window,
                    corner: Corner.BottomRight,
                    offsetX: 10,
                    offsetY: 10);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(this.duration),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(30));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
        }

        #endregion
    }
}
