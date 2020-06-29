namespace GAIA2020.Design
{
    using GAIA2020.Manager;
    using GAIA2020.Models;
    using GAIA2020.Utilities;
    using GaiaDB;
    using GaiaDB.Enums;
    using HMFrameWork.Ancestor;
    using LogStyle;
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Defines the <see cref="BoringView" />.
    /// </summary>
    public partial class BoringView : AUserControl, ILoadComplete, ILogStyleChanged
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BoringView"/> class.
        /// </summary>
        public BoringView()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The BeginInit.
        /// </summary>
        public override void BeginInit()
        {
        }

        /// <summary>
        /// The EndInit.
        /// </summary>
        public override void EndInit()
        {
            App.GetViewManager().AddValue(typeof(BoringView), this);
        }

        /// <summary>
        /// The iLoadComplete.
        /// </summary>
        public void iLoadComplete()
        {
            LoadStyle(DBDoc.Get_CurrDoc().Get_ActiveStyle());
        }

        /// <summary>
        /// The iLogStyleChange.
        /// </summary>
        public void iLogStyleChange()
        {
            // 로그스타일 변경으로 인한 작업
            LoadStyle();
        }

        /// <summary>
        /// The LoadStyle.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool LoadStyle()
        {
            return LoadStyle(DBDoc.Get_CurrDoc().Get_ActiveStyle());
        }

        /// <summary>
        /// The LoadStyle.
        /// </summary>
        /// <param name="e">The e<see cref="eDepartment"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool LoadStyle(eDepartment e)
        {
            // 로그스타일 도면 찾기
            LogStyleManager styleManager = App.GetLogStyleManager();
            string styleName = styleManager.GetLogStyleFileName(e);
            if (File.Exists(styleName))
            {
                // 기존 파일 닫기
                logview.UnLoad();
                // 작업파일(임시작업 파일)이름 찾기
                string fileName = styleManager.GetLogFileNameTemp(styleName);
                // 스타일읽고 임시파일로 작업파일 생성
                LogStyleBase logstyle = styleManager.GetLogStyle(e, styleName, fileName);
                // 사용할 로그 스타일지정과 작업파일열기
                logview.Load(logstyle, fileName);
                // 스타일
                // Check!!! ((BoringViewModel)this.DataContext).logStyle = e;

                this.DrillPipeNumTextBlock_ButtonClicked(null, null);

                return true;
            }
            return false;
        }

        /// <summary>
        /// The addDrillLog_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private void addDrillLog_Click(object sender, RoutedEventArgs e)
        {
            switch (DBDoc.Get_CurrDoc().Get_ActiveStyle(true))
            {
                case eDepartment.Fill:
                    this.drillPipeNumTextBlock.FixedText = "SB-";
                    break;
                case eDepartment.Bridge:
                    this.drillPipeNumTextBlock.FixedText = "BB-";
                    break;
                case eDepartment.Cut:
                    this.drillPipeNumTextBlock.FixedText = "CB-";
                    break;
                case eDepartment.Tunnel:
                    this.drillPipeNumTextBlock.FixedText = "TB-";
                    break;
                case eDepartment.BorrowPit:
                    break;
                case eDepartment.TrialPit:
                    break;
                case eDepartment.HandAuger:
                    break;
            }

            if (this.drillPipeNumTextBlock.Visibility.Equals(Visibility.Collapsed))
            {
                if (this.drillbox.Items.Count.Equals(0))
                {
                    this.drillPipeNumTextBlock.Data = "1";
                }
                else
                {
                    this.drillPipeNumTextBlock.Data = (this.drillbox.Items.Cast<DrillProperty>().Select(x => StringUtil.GetFirstIntFromString(x.Title.Split('-')[1])).Max() + 1).ToString();
                }
                BoringViewModel vm = this.DataContext as BoringViewModel;
                vm.AddDrillLogVisibility = Visibility.Visible;
                vm.AddButtonContent = "<";
            }
            else
            {
                BoringViewModel vm = this.DataContext as BoringViewModel;
                vm.AddDrillLogVisibility = Visibility.Collapsed;
                vm.AddButtonContent = "+";
            }
        }

        /// <summary>
        /// The AUserControl_Loaded.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="System.Windows.RoutedEventArgs"/>.</param>
        private void AUserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!aloadflag)
            {
                aloadflag = true; // 한번만 호출하자
                ((BoringViewModel)DataContext).Set_ViewCtrl(logview);
            }
        }

        /// <summary>
        /// The BtnAfterClick.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private void BtnAfterClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ComboBox targetComboBox = button.Tag.Equals("page") ? this.pageBox : this.drillbox;

            int targetIndex = targetComboBox.SelectedIndex + 1;
            if (targetIndex < targetComboBox.Items.Count)
            {
                targetComboBox.SelectedIndex = targetIndex;
            }
        }

        /// <summary>
        /// The BtnBeforeClick.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private void BtnBeforeClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ComboBox targetComboBox = button.Tag.Equals("page") ? this.pageBox : this.drillbox;

            int targetIndex = targetComboBox.SelectedIndex - 1;
            if (targetIndex >= 0)
            {
                targetComboBox.SelectedIndex = targetIndex;
            }
        }

        /// <summary>
        /// The DrillPipeNumTextBlock_ButtonClicked.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void DrillPipeNumTextBlock_ButtonClicked(object sender, EventArgs e)
        {
            BoringViewModel vm = this.DataContext as BoringViewModel;
            vm.AddDrillLogVisibility = Visibility.Collapsed;
            vm.AddButtonContent = "+";
        }

        /// <summary>
        /// The Fit_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private void Fit_Click(object sender, RoutedEventArgs e)
        {
            this.logview.dlog.canvas.ZoomExtents();
            this.logview.dlog.canvas.Regenerate();
        }

        #endregion
    }
}
