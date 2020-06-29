namespace GAIA2020.Menu
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Controls;
    using GaiaDB.Enums;
    using HMFrameWork.Ancestor;

    public partial class MenuView : AUserControl
    {
        public static readonly DependencyProperty DepartmentProperty = DependencyProperty.Register("CurrDepartment", typeof(eDepartment), typeof(MenuView), new PropertyMetadata(OnDepartmentChanged));

        private static void OnDepartmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MenuView;            
            control.SetCurrentMenu((eDepartment)(e.NewValue));
        }


        #region Constructors

        public MenuView()
        {
            InitializeComponent();


            var binding = new Binding("CurrDepartment") { Mode = BindingMode.TwoWay };
            this.SetBinding(DepartmentProperty, binding);
        }

        #endregion



        #region Methods

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            App.GetViewManager().AddValue(typeof(MenuView), this);
        }

        internal void SetCurrentMenu(eDepartment selectedDepartment)
        {
            if (!selectedDepartment.Equals(eDepartment.None))
            {
                string menu = selectedDepartment.ToString();
                RadioButton radioButton = FindName(menu + "Button") as RadioButton;
                if (radioButton.IsChecked != true)
                {
                    radioButton.IsChecked = true;
                    //radioButton.Command.Execute(selectedDepartment.ToString());
                }
            }            
        }

        #endregion
    }
}
