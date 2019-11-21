namespace Common.Controls
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;

    public partial class NumericUpDown : TextBox, INotifyPropertyChanged
    {
        #region Fields

        private string content;

        #endregion

        #region Constructors

        public NumericUpDown()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Content = "1";
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        public string Content
        {
            get { return content; }
            set { content = value;
                OnPropertyChanged("Content"); }
        }

        #endregion

        #region Methods

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged; if (handler != null) { handler(this, new PropertyChangedEventArgs(name)); }
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = (int.Parse(this.Content) + 1).ToString();
        }
        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            this.Content = (int.Parse(this.Content) - 1).ToString();
        }

        #endregion
    }
}
