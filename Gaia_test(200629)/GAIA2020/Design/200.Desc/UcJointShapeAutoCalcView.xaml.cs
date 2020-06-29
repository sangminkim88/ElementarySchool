using HMFrameWork.Ancestor;
using System;
using System.Windows;

namespace GAIA2020.Design
{
    /// <summary>
    /// Interaction logic for UcJointShapeAutoCalcDialog.xaml
    /// </summary>
    public partial class UcJointShapeAutoCalcView : AWindow
    {
        public UcJointShapeAutoCalcView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The Window_Close.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        protected override void Window_Close(object sender, RoutedEventArgs e)
        {
        }

        private void AWindow_ContentRendered(object sender, EventArgs e)
        {
            this.btnOk.Focus();
        }

        private void AWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((UcJointShapeAutoCalcViewModel)DataContext).Update();            
        }

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
            {
            }
        }
    }
}
