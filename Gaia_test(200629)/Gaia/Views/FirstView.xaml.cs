namespace Gaia.Views
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;
    using System.Windows.Media;

    public partial class FirstView : Grid
    {
        #region Constructors

        //public FirstView(Action<Tuple<string, string>> setA1, Action<List<Tuple<string, Brush>>> setA2, Action<string> setA3)
        public FirstView(FirstViewModel test)
        {
            InitializeComponent();
            //this.soilColumn.SetAction(setA1);
            //this.polygonColorPicker.SetAction(setA2);
            //this.sampleForm.SetAction(setA3);
            this.soilColumn.SetProp(test);
            this.polygonColorPicker.SetProp(test);
            this.sampleForm.SetProp(test);
        }

        #endregion
    }
}
