namespace Gaia.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public partial class ConditionalScrollViewer : ScrollViewer
    {
        #region Fields

        private bool useHorizontalScroll;

        private bool useVerticalScroll;

        #endregion

        #region Properties

        public bool UseHorizontalScroll
        {
            get { return useHorizontalScroll; }
            set { useHorizontalScroll = value; }
        }

        public bool UseVerticalScroll
        {
            get { return useVerticalScroll; }
            set { useVerticalScroll = value; }
        }

        #endregion

        #region Methods

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.SizeChanged += this._SizeChanged;
        }

        private void _SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.UseHorizontalScroll.Equals(true))
            {
                Viewbox viewbox = (Viewbox)this.Content;
                if (this.ActualWidth < viewbox.MinWidth)
                {
                    this.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                    viewbox.Width = viewbox.MinWidth;
                }
                else
                {
                    this.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    viewbox.Width = double.NaN;
                    viewbox.Stretch = Stretch.Uniform;
                }
            }

            if (this.UseVerticalScroll.Equals(true))
            {
                Viewbox viewbox = (Viewbox)this.Content;
                if (this.ActualHeight < viewbox.MinHeight)
                {
                    this.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                    viewbox.Height = viewbox.MinHeight;
                }
                else
                {
                    this.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    viewbox.Height = double.NaN;
                    viewbox.Stretch = Stretch.Uniform;
                }
            }
        }

        #endregion
    }
}
