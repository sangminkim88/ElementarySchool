namespace GAIA2020.Design
{
    using GAIA2020.Utilities;
    using GaiaDB;
    using GaiaDB.Enums;
    using HmDataDocument;
    using HMFrameWork.Ancestor;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;

    /// <summary>
    /// Defines the <see cref="PrintViewModel" />.
    /// </summary>
    public class PrintViewModel : AViewModel
    {
        #region Fields

        /// <summary>
        /// Defines the selectedValue.
        /// </summary>
        private string selectedValue;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintViewModel"/> class.
        /// </summary>
        public PrintViewModel()
        {
            //CommandTest = new RelayCommand(Test);

            loadTreeItems();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the SelectedValue.
        /// </summary>
        public string SelectedValue
        {
            get { return selectedValue; }
            set
            {
                SetValue(ref selectedValue, value);
                showPreview();
            }
        }

        /// <summary>
        /// Gets or sets the TreeViewData.
        /// </summary>
        public ObservableCollection<TreeViewModel> TreeViewData { get; set; } = new ObservableCollection<TreeViewModel>();

        #endregion

        #region Methods

        /// <summary>
        /// The BeginInit.
        /// </summary>
        public override void BeginInit()
        {
        }

        /// <summary>
        /// The EndInit.
        /// </summary>
        public override void EndInit()
        {
            App.GetViewModelManager().AddValue(typeof(PrintViewModel), this);
        }

        /// <summary>
        /// The loadTreeItems.
        /// </summary>
        private void loadTreeItems()
        {
            DBDoc doc = DBDoc.Get_CurrDoc();
            DBDataHINFO dbDataHINFO = null;
            doc.hinfo.Get_Data(ref dbDataHINFO);

            List<int> counts = new List<int>();
            counts.Add(dbDataHINFO.dicInt[eDepartment.Fill.ToString()]);
            counts.Add(dbDataHINFO.dicInt[eDepartment.Bridge.ToString()]);
            counts.Add(dbDataHINFO.dicInt[eDepartment.Cut.ToString()]);
            counts.Add(dbDataHINFO.dicInt[eDepartment.Tunnel.ToString()]);
            counts.Add(dbDataHINFO.dicInt[eDepartment.TrialPit.ToString()]);
            counts.Add(dbDataHINFO.dicInt[eDepartment.HandAuger.ToString()]);

            TreeViewModel root = new TreeViewModel("All", null);
            TreeViewModel location = new TreeViewModel("위치도(?)", root);
            TreeViewModel plot = new TreeViewModel("단면도(?)", root, Visibility.Visible);
            TreeViewModel drillLog = new TreeViewModel("주상도", root);
            TreeViewModel fill = new TreeViewModel("쌓기부(" + counts[0] + ")", drillLog);
            TreeViewModel bridge = new TreeViewModel("교량부(" + counts[1] + ")", drillLog);
            TreeViewModel cut = new TreeViewModel("깍기부(" + counts[2] + ")", drillLog);
            TreeViewModel tunnel = new TreeViewModel("터널부(" + counts[3] + ")", drillLog);
            TreeViewModel trialpit = new TreeViewModel("시험굴(" + counts[4] + ")", drillLog);
            TreeViewModel handauger = new TreeViewModel("핸드오거(" + counts[5] + ")", drillLog);

            root.Children.Add(location);
            root.Children.Add(plot);
            root.Children.Add(drillLog);
            drillLog.Children.Add(fill);
            drillLog.Children.Add(bridge);
            drillLog.Children.Add(cut);
            drillLog.Children.Add(tunnel);
            drillLog.Children.Add(trialpit);
            drillLog.Children.Add(handauger);

            List<Tuple<TreeViewModel, int>> tmp = new List<Tuple<TreeViewModel, int>>();
            tmp.Add(new Tuple<TreeViewModel, int>(fill, counts[0]));
            tmp.Add(new Tuple<TreeViewModel, int>(bridge, counts[1]));
            tmp.Add(new Tuple<TreeViewModel, int>(cut, counts[2]));
            tmp.Add(new Tuple<TreeViewModel, int>(tunnel, counts[3]));
            tmp.Add(new Tuple<TreeViewModel, int>(trialpit, counts[4]));
            tmp.Add(new Tuple<TreeViewModel, int>(handauger, counts[5]));

            foreach (var item in tmp)
            {
                for (int i = 0; i < item.Item2; i++)
                {
                    item.Item1.Children.Add(new TreeViewModel(item.Item1.Name.Split('(')[0] + i, item.Item1));
                }
            }

            TreeViewData.Add(root);
        }

        /// <summary>
        /// The showPreview.
        /// </summary>
        private void showPreview()
        {
        }

        #endregion
    }
}
