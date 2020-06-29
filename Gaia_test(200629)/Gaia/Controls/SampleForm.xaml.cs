namespace Gaia.Controls
{
    using Gaia.Views;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public partial class SampleForm : Grid
    {
        #region Fields

        private List<RadioButton> radioButtons = new List<RadioButton>();
        private Canvas oldCanvas;

        private Action<string> setA3;
        private FirstViewModel test;
        private FirstViewModel prop;
        #endregion

        #region Constructors

        public SampleForm()
        {
            InitializeComponent();
            this.radioButtons.Add(this.sample1);
            this.radioButtons.Add(this.sample2);
            this.radioButtons.Add(this.sample3);
            this.radioButtons.Add(this.sample4);
        }

        public void SetAction(Action<string> setA3, FirstViewModel test)
        {
            this.setA3 = setA3;
            this.test = test;
        }
        public void SetProp(FirstViewModel prop)
        {
            this.prop = prop;
        }

        #endregion

        #region Methods

        public string GetSelectedSampleForm()
        {
            switch (this.radioButtons.IndexOf(this.radioButtons.Find(x => x.IsChecked.Equals(true))))
            {
                case 0:
                    return "UD";
                case 1:
                    return "DS";
                case 2:
                    return "NS";
                case 3:
                    return "CS";
                default:
                    return string.Empty;
            }
        }

        public void SetSelectedSampleForm(string code)
        {
            Canvas canvas;
            switch (code.ToUpper())
            {
                case "UD":
                    canvas = this.radioButtons[0].Content as Canvas;
                    break;
                case "DS":
                    canvas = this.radioButtons[1].Content as Canvas;
                    break;
                case "NS":
                    canvas = this.radioButtons[2].Content as Canvas;
                    break;
                case "CS":
                    canvas = this.radioButtons[3].Content as Canvas;
                    break;
                default:
                    return;
            }
            this.Canvas_MouseDown(canvas, null);
        }

        #endregion

        private void Canvas_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            double width = 0;
            double height = 0;

            foreach (FrameworkElement fe in canvas.Children)
            {
                Rect rect = VisualTreeHelper.GetDescendantBounds(fe);

                if (width < rect.Right)
                {
                    width = rect.Right;
                }
                if (height < rect.Bottom)
                {
                    height = rect.Bottom;
                }
            }

            this.canvas.Width = width;
            this.canvas.Height = height;
        }
        
        private void Canvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Canvas canvas = sender as Canvas;
            if(this.oldCanvas != null && !this.oldCanvas.Equals(canvas))
            {
                this.oldCanvas.Background = new SolidColorBrush(Colors.Transparent);
            }
            this.oldCanvas = canvas;
            canvas.Background = FindResource("Theme.Background") as SolidColorBrush;
            RadioButton radioButton = canvas.Parent as RadioButton;
            radioButton.IsChecked = true;

            //this.setA3(this.GetSelectedSampleForm());
            this.prop.SampleForm = this.GetSelectedSampleForm();
        }

    }
}
