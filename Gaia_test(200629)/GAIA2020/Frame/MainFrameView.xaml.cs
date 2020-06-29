namespace GAIA2020.Frame
{
    using GAIA2020.Design;
    using GAIA2020.Utilities;
    using GaiaDB;
    using GaiaDB.Enums;
    using HMFrameWork.Ancestor;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Point = System.Windows.Point;

    /// <summary>
    /// Defines the <see cref="MainFrameView" />.
    /// </summary>
    public partial class MainFrameView : AWindow, IMainFrame
    {
        #region Fields

        /// <summary>
        /// Defines the maximizeByClick.
        /// </summary>
        private bool maximizeByClick;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainFrameView"/> class.
        /// </summary>
        public MainFrameView()
        {
            InitializeComponent();

            this.init();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The Get_PreViewImage.
        /// </summary>
        /// <returns>The <see cref="System.Drawing.Bitmap"/>.</returns>
        public Bitmap Get_PreViewImage()
        {
            //return null;
            var mngr = App.GetViewModelManager();
            LocationViewModel vm = mngr.GetValue(typeof(LocationViewModel), false) as LocationViewModel;
            if (vm == null) return null;

            return vm.RenderToBitmap(900, 600);
        }

        /// <summary>
        /// The iLogStyleChange.
        /// </summary>
        public void iLogStyleChange()
        {
            // 로그스타일 변경으로 인한 작업
            //

            // 자식으로 전달
            ILogStyleChanged i;
            foreach (var v in VTHelper.FindVisualChildren<AUserControl>(this))
            {
                i = v as ILogStyleChanged;
                if (null != i)
                {
                    i.iLogStyleChange();
                }
            }
        }

        /// <summary>
        /// The Window_Close.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        protected override void Window_Close(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// The _MouseEnter.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="MouseEventArgs"/>.</param>
        private void _MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.Cursor != Cursors.Wait)
            {
                if (sender is Grid)
                {
                    Mouse.OverrideCursor = Cursors.Hand;
                }
                else
                {
                    Mouse.OverrideCursor = Cursors.SizeWE;
                }
            }
        }

        /// <summary>
        /// The _MouseLeave.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="MouseEventArgs"/>.</param>
        private void _MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.Cursor != Cursors.Wait)
            {
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        /// <summary>
        /// The AWindow_Closing.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="System.ComponentModel.CancelEventArgs"/>.</param>
        private void AWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DBDoc doc = DBDoc.Get_CurrDoc();
            bool bCloseWindow = doc.New_Document(false);
            //bool bCloseWindow = doc.ChangeDB_SaveDoc(false);
            if (bCloseWindow)
            {
                MainFrame_Imp.Set_MainFrame(null);
                doc.Set_ActiveStyle(eDepartment.None);
                App.GetViewManager().BoringView.logview.UnLoad();
            }
            else
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// The AWindow_Closed.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="System.EventArgs"/>.</param>
        private void AWindow_Closed(object sender, EventArgs e)
        {

            // 매니저닫기
            var vmmanager = App.GetViewModelManager();
#if true
            List<Type> viewModels = new List<Type>();
            //상민(200625)
            //TextPolygon에서 Loaded 함수 내 VisualTreeHelper.GetDescendantBounds 오류로 추가함
            //디버그 모드에서는 발생하지 않지만 릴리즈모드에서는 반환값이 의도와는 다른 값으로 나와 다운됨
            //비주얼트리를 구성하기 위해 생성을 미리 함(MainWindow에서)
            viewModels.Add(typeof(PrjListViewModel));
            viewModels.Add(typeof(APartDescViewModel));

            vmmanager?.CleanupExcept(viewModels);
#else
            vmmanager?.Cleanup();
#endif
            var vwmanager = App.GetViewManager();
#if true
            List<Type> views = new List<Type>();
            views.Add(typeof(PrjListView));
            views.Add(typeof(APartDescView));
            vwmanager?.CleanupExcept(views);
#else
            vwmanager?.Cleanup();
#endif

            // 업데이트 등록을 모두 삭제
            HmDataDocument.TransactionCtrl.DelAll_DBUpdateWndCtrl();
            // 프로젝트 리스트 ViewModel만 새로 추가(수정된 주상도 내용을 그리드에 업데이트 하기위해서)
            var vm = vmmanager.GetValue(typeof(PrjListViewModel)) as PrjListViewModel;
            HmDataDocument.TransactionCtrl.Add_DBUpdateWndCtrl(vm as HmDataDocument.IDBUpdate);
            APartDescViewModel aPartDescViewModel = vmmanager.GetValue(typeof(APartDescViewModel)) as APartDescViewModel;
            HmDataDocument.TransactionCtrl.Add_DBUpdateWndCtrl(aPartDescViewModel);

            GC.Collect();
        }

        /// <summary>
        /// The AWindow_ContentRendered.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void AWindow_ContentRendered(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowStyle = WindowStyle.None;
            }
        }

        /// <summary>
        /// The AWindow_SizeChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="SizeChangedEventArgs"/>.</param>
        private void AWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowStyle = WindowStyle.None;
                this.Topmost = true;
                this.Topmost = false;
            }
        }

        /// <summary>
        /// The AWindow_StateChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void AWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowStyle = WindowStyle.None;
            }
        }

        /// <summary>
        /// The Grid_MouseLeftButtonDown.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="MouseButtonEventArgs"/>.</param>
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            if (e.ClickCount == 2)
            {
                if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                }
                else if (WindowState != WindowState.Maximized)
                {
                    WindowState = WindowState.Maximized;
                    maximizeByClick = true;
                }
            }
            else
            {
                if (WindowState is WindowState.Normal)
                {
                    DragMove();
                }
            }
        }

        /// <summary>
        /// The Grid_MouseMove.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="MouseEventArgs"/>.</param>
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (maximizeByClick)
            {
                maximizeByClick = false;
                return;
            }
            if (e.LeftButton.Equals(MouseButtonState.Pressed) &&
                    WindowState.Equals(WindowState.Maximized))
            {
                Point point = Mouse.GetPosition(this);
                double percent = point.X / (this.Width / 100);
                WindowState = WindowState.Normal;

                this.Left = point.X - ((this.Width / 100) * percent);
                this.Top = -point.Y;

                DragMove();
            }
        }

        /// <summary>
        /// The init.
        /// </summary>
        private void init()
        {
            //App.GetGlobalVar().Init();
            DBDoc.Get_CurrDoc().New_Document();
            MainFrame_Imp.Set_MainFrame(this);
        }

#endregion
    }
}
