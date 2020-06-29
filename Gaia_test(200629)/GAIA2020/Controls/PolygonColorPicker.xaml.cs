namespace GAIA2020.Controls
{
    using GAIA2020.Utilities;
    using HmGeometry;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Defines the <see cref="PolygonColorPicker" />.
    /// </summary>
    public partial class PolygonColorPicker : UserControl
    {
        #region Fields

        /// <summary>
        /// Defines the SelectedColorIdsProperty.
        /// </summary>
        public static readonly DependencyProperty SelectedColorIdsProperty = DependencyProperty.Register(
            "SelectedColorIds", typeof(ObservableCollection<int>), typeof(PolygonColorPicker),
            new PropertyMetadata(new ObservableCollection<int>(), new PropertyChangedCallback(SelectedColorIdsPropertyChangedCallBack)));

        /// <summary>
        /// Defines the MAXIMUM_SELECT_COUNT.
        /// </summary>
        private static int MAXIMUM_SELECT_COUNT = 2;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonColorPicker"/> class.
        /// </summary>
        public PolygonColorPicker()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the SelectedColorIds.
        /// </summary>
        public ObservableCollection<int> SelectedColorIds
        {
            set
            {
                SetValue(SelectedColorIdsProperty, value);
            }
            get
            {
                return (ObservableCollection<int>)GetValue(SelectedColorIdsProperty);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The SelectedColorIdsPropertyChangedCallBack.
        /// </summary>
        /// <param name="d">The d<see cref="DependencyObject"/>.</param>
        /// <param name="value">The value<see cref="DependencyPropertyChangedEventArgs"/>.</param>
        private static void SelectedColorIdsPropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs value)
        {
            var instance = d as PolygonColorPicker;
            if (instance == null)
            {
                return;
            }

            var newCollection = value.NewValue as ObservableCollection<int>;
            var oldCollection = value.OldValue as ObservableCollection<int>;

            if (newCollection != null)
            {
                newCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(instance.SelectedColorIdsCollectionChanged);
            }

            if (oldCollection != null)
            {
                oldCollection.CollectionChanged -= new NotifyCollectionChangedEventHandler(instance.SelectedColorIdsCollectionChanged);
            }
        }

        /// <summary>
        /// The ClearSelect.
        /// </summary>
        public void ClearSelect()
        {
            foreach (var item in this.grid.Children)
            {
                if (item is TextPolygon) (item as TextPolygon).IsSelected = false;
            }
        }

        /// <summary>
        /// The GetSelectedColorIDs.
        /// </summary>
        /// <returns>The <see cref="List{int}"/>.</returns>
        public List<int> GetSelectedColorIDs()
        {
            return this.SelectedColorIds.OrderBy(x => x).ToList();
        }

        /// <summary>
        /// The SetSelectedColorByIDs.
        /// </summary>
        /// <param name="ids">The ids<see cref="List{int}"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool SetSelectedColorByIDs(List<int> ids)
        {
            bool isGood = true;
            this.ClearSelect();
            for (int i = 0; i < ids.Count; i++)
            {
                if (i >= MAXIMUM_SELECT_COUNT) break;
                isGood &= this.setSelectedColorByID(ids[i]);
            }
            return isGood;
        }

        /// <summary>
        /// The SetSelectedColorByNames.
        /// </summary>
        /// <param name="colorNames">The colorNames<see cref="List{string}"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool SetSelectedColorByNames(List<string> colorNames)
        {
            bool isGood = true;
            this.ClearSelect();
            foreach (var item in colorNames)
            {
                isGood &= this.setSelectedColorByName(item);
            }
            return isGood;
        }

        /// <summary>
        /// The SelectedColorIdsCollectionChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="NotifyCollectionChangedEventArgs"/>.</param>
        private void SelectedColorIdsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.SelectedColorIds.Count > MAXIMUM_SELECT_COUNT)
            {
                this.SelectedColorIds.Remove(int.Parse(e.NewItems[0].ToString()));
            }
            else
            {
                this.SetSelectedColorByIDs(this.SelectedColorIds.ToList());
            }
        }

        /// <summary>
        /// The setSelectedColorByID.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool setSelectedColorByID(int id)
        {
            foreach (var item in this.grid.Children)
            {
                if (item is TextPolygon)
                {
                    TextPolygon textPolygon = item as TextPolygon;
                    if (textPolygon.ID.Equals(id))
                    {
                        if (textPolygon.IsSelectable)
                        {
                            if (textPolygon.IsSelected == true)
                            {
                                if (textPolygon.PolygonText.Equals("흑 색"))
                                {
                                    textPolygon.IsSelected = null;
                                }
                                else
                                {
                                    textPolygon.IsSelected = false;
                                }
                            }
                            else
                            {
                                textPolygon.IsSelected = true;
                            }
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// The setSelectedColorByName.
        /// </summary>
        /// <param name="colorName">The colorName<see cref="string"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool setSelectedColorByName(string colorName)
        {
            foreach (var item in this.grid.Children)
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

        /// <summary>
        /// The TextPolygon_PreviewMouseDown.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="System.Windows.Input.MouseButtonEventArgs"/>.</param>
        private void TextPolygon_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextPolygon textPolygon = sender as TextPolygon;
            if (textPolygon.IsSelected == true)
            {
                this.SelectedColorIds.Remove(textPolygon.ID);
            }
            else
            {
                if (this.SelectedColorIds.Count < PolygonColorPicker.MAXIMUM_SELECT_COUNT)
                {
                    List<int> data = new List<int>(this.SelectedColorIds);
                    data.Add(textPolygon.ID);
                    List<Tuple<string, Brush>> straColor = ColorUtil.GetGaiaColorDataByID(data);
                    if (straColor.Count(x => x.Item1.Equals("흑색")).Equals(1) && straColor.Count(x => x.Item1.Equals("백색")).Equals(1))
                    {
                        NotifyHelper.Instance.Show(NotifyHelper.NotiType.Error, "흑색과 백색을 동시에 선택하실 수 없습니다.");
                        return;
                    }
                    this.SelectedColorIds.Add(textPolygon.ID);
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// The UserControl_IsEnabledChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="DependencyPropertyChangedEventArgs"/>.</param>
        private void UserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                this.border.IsEnabled = true;
                this.border.Opacity = 1;
            }
            else
            {
                this.border.IsEnabled = false;
                this.border.Opacity = .5;
            }
        }

        #endregion
    }
}
