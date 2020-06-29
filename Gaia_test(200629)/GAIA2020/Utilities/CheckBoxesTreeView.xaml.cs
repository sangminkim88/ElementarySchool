namespace GAIA2020.Utilities
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Defines the <see cref="CheckBoxesTreeView" />.
    /// </summary>
    public partial class CheckBoxesTreeView : UserControl
    {
        #region Fields

        /// <summary>
        /// Defines the SelectedValueProperty.
        /// </summary>
        public static readonly DependencyProperty SelectedValueProperty =
           DependencyProperty.Register("SelectedValue", typeof(string), typeof(CheckBoxesTreeView), new UIPropertyMetadata(default(string)));

        /// <summary>
        /// Defines the TreeViewDataProperty.
        /// </summary>
        public static readonly DependencyProperty TreeViewDataProperty =
           DependencyProperty.Register("TreeViewData", typeof(ObservableCollection<TreeViewModel>), typeof(CheckBoxesTreeView),
               new UIPropertyMetadata(default(ObservableCollection<TreeViewModel>)));

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckBoxesTreeView"/> class.
        /// </summary>
        public CheckBoxesTreeView()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the SelectedValue.
        /// </summary>
        public string SelectedValue
        {
            get { return (string)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the TreeViewData.
        /// </summary>
        public ObservableCollection<TreeViewModel> TreeViewData
        {
            get { return (ObservableCollection<TreeViewModel>)GetValue(TreeViewDataProperty); }
            set { SetValue(TreeViewDataProperty, value); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The Button_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            TreeViewModel treeViewModel = button.DataContext as TreeViewModel;
            Console.WriteLine(treeViewModel.Name);
        }

        /// <summary>
        /// The TreeView_SelectedItemChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedPropertyChangedEventArgs{object}"/>.</param>
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView treeView = sender as TreeView;
            TreeViewModel treeViewModel = treeView.SelectedValue as TreeViewModel;
            if (treeViewModel != null)
            {
                this.SelectedValue = treeViewModel.Name;
            }
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="TreeViewModel" />.
    /// </summary>
    public class TreeViewModel : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Defines the isChecked.
        /// </summary>
        private bool? isChecked = false;

        /// <summary>
        /// Gets the Name......
        /// </summary>
        private string name;

        /// <summary>
        /// Defines the parent.
        /// </summary>
        private TreeViewModel parent;

        /// <summary>
        /// Defines the showAddButton.
        /// </summary>
        private Visibility showAddButton = Visibility.Collapsed;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewModel"/> class.
        /// </summary>
        /// <param name="name">The name<see cref="string"/>.</param>
        /// <param name="parent">The parent<see cref="TreeViewModel"/>.</param>
        /// <param name="showAddButton">The showAddButton<see cref="Visibility"/>.</param>
        public TreeViewModel(string name, TreeViewModel parent, Visibility showAddButton = Visibility.Collapsed)
        {
            this.Name = name;
            this.parent = parent;
            this.showAddButton = showAddButton;
        }

        #endregion

        #region Events

        /// <summary>
        /// Defines the PropertyChanged.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Children.
        /// </summary>
        public ObservableCollection<TreeViewModel> Children { get; private set; } = new ObservableCollection<TreeViewModel>();

        /// <summary>
        /// Gets or sets the IsChecked.
        /// </summary>
        public bool? IsChecked
        {
            get { return isChecked; }
            set { this.SetIsChecked(value, true, true); }
        }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                this.OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Gets or sets the Parent.
        /// </summary>
        public TreeViewModel Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                this.OnPropertyChanged("Parent");
            }
        }

        /// <summary>
        /// Gets or sets the ShowAddButton.
        /// </summary>
        public Visibility ShowAddButton
        {
            get { return showAddButton; }
            set { showAddButton = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The OnPropertyChanged.
        /// </summary>
        /// <param name="prop">The prop<see cref="string"/>.</param>
        internal void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// The SetIsChecked.
        /// </summary>
        /// <param name="value">The value<see cref="bool?"/>.</param>
        /// <param name="updateChildren">The updateChildren<see cref="bool"/>.</param>
        /// <param name="updateParent">The updateParent<see cref="bool"/>.</param>
        private void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == isChecked)
                return;

            isChecked = value;

            if (updateChildren && isChecked.HasValue)
            {
                foreach (var item in this.Children)
                {
                    item.SetIsChecked(isChecked, true, false);
                }
            }

            if (updateParent && parent != null)
                parent.VerifyCheckState();

            this.OnPropertyChanged("IsChecked");
        }

        /// <summary>
        /// The VerifyCheckState.
        /// </summary>
        private void VerifyCheckState()
        {
            bool? state = null;
            for (int i = 0; i < this.Children.Count; ++i)
            {
                bool? current = this.Children[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }
            this.SetIsChecked(state, false, true);
        }

        #endregion
    }
}
