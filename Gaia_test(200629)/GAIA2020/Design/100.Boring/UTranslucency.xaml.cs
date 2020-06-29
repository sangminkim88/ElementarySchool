namespace GAIA2020.Design
{
    using GAIA2020.Utilities;
    using LogStyle;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Defines the <see cref="UTranslucency" />.
    /// </summary>
    public partial class UTranslucency : UserControl
    {
        #region Fields

        /// <summary>
        /// Defines the currentLogTag.
        /// </summary>
        private LogTag currentLogTag;

        /// <summary>
        /// Defines the isNumeric.
        /// </summary>
        private bool isNumeric;

        /// <summary>
        /// Defines the okAction.
        /// </summary>
        private Action<LogTag, object, bool> okAction;

        /// <summary>
        /// Defines the oldData.
        /// </summary>
        private string oldData;
        private bool isTab;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UTranslucency"/> class.
        /// </summary>
        public UTranslucency()
        {
            InitializeComponent();
            myInit();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The HideControl.
        /// </summary>
        public void HideControl()
        {
            this.myInit();
        }

        /// <summary>
        /// The SetOkAction.
        /// </summary>
        /// <param name="okAction">The okAction<see cref="Action{LogTag, object}"/>.</param>
        public void SetOkAction(Action<LogTag, object, bool> okAction)
        {
            this.okAction = okAction;
        }

        /// <summary>
        /// The ShowControl.
        /// </summary>
        /// <param name="logTag">The logTag<see cref="LogTag"/>.</param>
        /// <param name="value">The value<see cref="string"/>.</param>
        /// <param name="leftx">The leftx<see cref="double"/>.</param>
        /// <param name="lefty">The lefty<see cref="double"/>.</param>
        /// <param name="w">The w<see cref="double"/>.</param>
        /// <param name="h">The h<see cref="double"/>.</param>
        /// <param name="isNumeric">The isNumeric<see cref="bool"/>.</param>
        public void ShowControl(LogTag logTag, string value, double leftx, double lefty, double w, double h, bool isNumeric = false)
        {
            if (logTag.type == null)
            {
                return;
            }

            this.isTab = false;
            this.isNumeric = isNumeric;

            this.currentLogTag = logTag;
            if (currentLogTag.valkey.Equals("시추표고") || currentLogTag.valkey.Equals("시추심도") ||
                currentLogTag.valkey.Equals("케이싱심도") || currentLogTag.valkey.Equals("지하수위"))
            {
                value = string.Empty;
                this.isNumeric = true;
            }
            else if (currentLogTag.valkey.Equals("시추공경"))
            {
                value = string.Empty;
            }


            this.panel.Visibility = Visibility.Visible;
            if (logTag.type.Equals(typeof(DateTime)))
            {
                textBox.Visibility = Visibility.Collapsed;
                multilineTextBox.Visibility = Visibility.Collapsed;
                calendar.Visibility = Visibility.Visible;
#if true
                SetCalendarFromTo(value as string);
#else
                DateTime dt;
                if (!DateTime.TryParse(value as string, out dt))
                    dt = DateTime.Now;
                calendar.SelectedDate = dt;
#endif
                this.okButton.Focus();
            }
            else if (logTag.type.Equals(typeof(decimal)))
            {
                textBox.Visibility = Visibility.Collapsed;
                multilineTextBox.Visibility = Visibility.Visible;
                calendar.Visibility = Visibility.Collapsed;
                multilineTextBox.Text = value;
                multilineTextBox.Focus();
                Keyboard.Focus(multilineTextBox);
            }
            else
            {
                textBox.Visibility = Visibility.Visible;
                multilineTextBox.Visibility = Visibility.Collapsed;
                calendar.Visibility = Visibility.Collapsed;
                textBox.Text = value;
                textBox.Focus();
                Keyboard.Focus(textBox);
            }

            this.Margin = new Thickness(leftx, lefty, 0, 0);
        }

        /// <summary>
        /// The Calendar_KeyUp.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="KeyEventArgs"/>.</param>
        private void Calendar_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                this.Ok_Click(null, null);
            }
        }

        /// <summary>
        /// The Calendar_MouseDoubleClick.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="MouseButtonEventArgs"/>.</param>
        private void Calendar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Ok_Click(null, null);
        }

        /// <summary>
        /// The Cancel_Clicked.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private void Cancel_Clicked(object sender, RoutedEventArgs e)
        {
            this.myInit();
        }

        /// <summary>
        /// The GetCalendarFromTo.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        private string GetCalendarFromTo()
        {
            string valstr = string.Empty, begdate = DateTime.Now.ToShortDateString(), findate = begdate;
            //
            int isize = calendar.SelectedDates.Count;
            if (isize > 0)
            {
                // 연속된 날짜는 드래그로 선택할 수 있음
                begdate = calendar.SelectedDates[0].ToShortDateString();
                findate = calendar.SelectedDates[isize - 1].ToShortDateString();
            }
            else
            {
                begdate = findate = calendar.SelectedDate?.ToShortDateString();
            }
            //
            if (begdate.Equals(findate))
                valstr = string.Format("{0}", begdate);
            else
                valstr = string.Format("{0} ~ {1}", begdate, findate);
            return valstr;
        }

        /// <summary>
        /// The myInit.
        /// </summary>
        private void myInit()
        {
            this.panel.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// The Ok_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (!this.isTab)
            {
                this.myInit();
            }
            this.okAction(currentLogTag,
                    textBox.Visibility.Equals(Visibility.Visible) ? textBox.Text :
                    multilineTextBox.Visibility.Equals(Visibility.Visible) ? multilineTextBox.Text :
                    //calendar.SelectedDate?.ToShortDateString());
                    GetCalendarFromTo(), this.isTab);
        }

        /// <summary>
        /// The SetCalendarFromTo.
        /// </summary>
        /// <param name="valstr">The valstr<see cref="string"/>.</param>
        private void SetCalendarFromTo(string valstr)
        {
            DateTime begdate = DateTime.Now, findate = DateTime.Now;

            string[] datestr = valstr.Split('~');
            if (datestr.Length > 0)
            {
                DateTime.TryParse(datestr[0].Trim(), out begdate);
                DateTime.TryParse(datestr[datestr.Length - 1].Trim(), out findate);
            }

            //
            calendar.SelectedDates.Clear();
            calendar.SelectedDates.AddRange(begdate, findate);
        }

        /// <summary>
        /// The TextBox_PreviewTextInput.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="TextCompositionEventArgs"/>.</param>
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            this.oldData = (sender as TextBox).Text;
        }

        /// <summary>
        /// The TextBox_TextChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="TextChangedEventArgs"/>.</param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string data = textBox.Text;
            if (this.isNumeric)
            {
                if (!StringUtil.IsNumeric(data))
                {
                    NotifyHelper.Instance.Show(NotifyHelper.NotiType.Error, "숫자형 데이터만 입력 가능합니다.");
                    textBox.Text = this.oldData;
                    textBox.SelectionStart = this.oldData.Length;
                }
            }          

            // 시추표고, 시추심도, 케이싱심도, 지하수위는 소수점 두째자리까지 표현되어야 함
            if (currentLogTag.valkey.Equals("시추표고") || currentLogTag.valkey.Equals("시추심도") ||
                currentLogTag.valkey.Equals("케이싱심도") || currentLogTag.valkey.Equals("지하수위"))
            {
                if (!StringUtil.CheckDecimalPlace(2, data))
                {
                    NotifyHelper.Instance.Show(NotifyHelper.NotiType.Error, "숫자형 데이터(소수점 두째자리까지)만 입력 가능합니다.");
                    textBox.Text = this.oldData;
                    textBox.SelectionStart = this.oldData.Length;
                }
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Tab))
            {
                //상민 : 다음 엔티티 선택하는 로직 들어가야함
                this.isTab = true;
                e.Handled = true;
                this.Ok_Click(null, null);
            }
 
        }

        #endregion
    }
}
