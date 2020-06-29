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

namespace Gaia.Controls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class WatermarkTextbox : TextBox
    {
        public WatermarkTextbox()
        {
            InitializeComponent();
        }

        #region Dependency Properties
        public static readonly DependencyProperty WaterMarkProperty;
        #endregion

        #region Properties
        public string WaterMark
        {
            get { return (string)GetValue(WaterMarkProperty); }
            set { SetValue(WaterMarkProperty, value); }
        }
        #endregion //Properties

        static WatermarkTextbox()
        {
            Type ownerType = typeof(WatermarkTextbox);
            WaterMarkProperty = DependencyProperty.Register(nameof(WaterMark), typeof(string), ownerType, new PropertyMetadata(null));
        }
    }
}
