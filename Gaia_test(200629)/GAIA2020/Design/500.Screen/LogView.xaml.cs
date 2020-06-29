using HMFrameWork.Ancestor;
using LogStyle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GAIA2020.Design
{
    /// <summary>
    /// LogView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LogView : AUserControl
    {
        public LogView()
        {
            InitializeComponent();
        }

        #region 로그스타일 인터페이스
        public void iLogStyleChange()
        {
            // 로그스타일 변경으로 인한 작업
            //

            // 자식으로 전달            
            ILogStyleChanged i;            
            int j, jsize = VisualTreeHelper.GetChildrenCount(this);
            for (j = 0; j < jsize; j++)
            {
                var v = VisualTreeHelper.GetChild(this, j);
                i = v as ILogStyleChanged;
                if ( null !=  i)
                {
                    i.iLogStyleChange();
                }
            }
        }
        #endregion

        #region DependencyProperty
        public static readonly DependencyProperty MapSpaceProperty = DependencyProperty.Register("LogStyle", typeof(LogStyleBase), typeof(LogView), new PropertyMetadata(OnLogStylePropertyChanged));
        private static void OnLogStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var x = BindingOperations.GetBinding(d, MapSpaceProperty);
            var control = d as LogView;
            if (d is LogView view)
            {
                LogStyleBase style = e.NewValue as LogStyleBase;
            }
        }
        #endregion

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

        private void _MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.Cursor != Cursors.Wait)
            {
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

    }
}
