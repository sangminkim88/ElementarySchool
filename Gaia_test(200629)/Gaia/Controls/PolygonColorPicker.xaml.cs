namespace Gaia.Controls
{
    using Gaia.Views;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public partial class PolygonColorPicker : Grid
    {
        #region Fields
        private static int MAXIMUM_SELECT_COUNT = 2;
        private List<TextPolygon> selectedTextPolygons = new List<TextPolygon>();

        private Action<List<Tuple<string, Brush>>> SetA2;
        private FirstViewModel prop;

        public void SetAction(Action<List<Tuple<string, Brush>>> setA2)
        {
            this.SetA2 = setA2;
        }
        public void SetProp(FirstViewModel prop)
        {
            this.prop = prop;
        }
        #endregion

        #region Constructors

        public PolygonColorPicker()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        public void ClearSelect()
        {
            foreach (var item in this.canvas.Children)
            {
                if (item is TextPolygon) (item as TextPolygon).IsSelected = false;
            }
        }
        
        public List<Tuple<string, Brush>> GetSelectedColors()
        {            
            return this.selectedTextPolygons.Select(x=> new Tuple<string, Brush>(x.PolygonText ,x.PathFill)).ToList();
        }
        
        public bool SetSelectedColorByNames(List<string> colorNames)
        {
            bool isGood = true;
            this.ClearSelect();
            foreach (var item in colorNames)
            {
                isGood &= this.SetSelectedColorByName(item);
            }
            return isGood;
        }

        private bool SetSelectedColorByName(string colorName)
        {
            foreach (var item in this.canvas.Children)
            {
                if (item is TextPolygon)
                {
                    TextPolygon textPolygon = item as TextPolygon;
                    if (textPolygon.PolygonText.Equals(colorName))
                    {
                        textPolygon.IsSelected = true;
                        return true;
                    }
                }
            }
            return false;
        }

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

        private void TextPolygon_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextPolygon textPolygon = sender as TextPolygon;
            if (textPolygon.IsSelected == true)
            {
                this.selectedTextPolygons.Remove(textPolygon);
            }
            else
            {
                if (this.selectedTextPolygons.Count >= PolygonColorPicker.MAXIMUM_SELECT_COUNT)
                {
                    if (this.selectedTextPolygons[0].PolygonText.Equals("흑 색"))
                    {
                        this.selectedTextPolygons[0].IsSelected = null;
                    }
                    else
                    {
                        this.selectedTextPolygons[0].IsSelected = false;
                    }
                    this.selectedTextPolygons.RemoveAt(0);
                }
                this.selectedTextPolygons.Add(textPolygon);
            }
            this.prop.PolygonColor = this.GetSelectedColors();
            //this.SetA2(this.GetSelectedColors());
        }

        #endregion
    }
}
