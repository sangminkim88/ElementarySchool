namespace GAIA2020.Design
{
    using GAIA2020.Controls;
    using GAIA2020.Models;
    using GAIA2020.Utilities;
    using GaiaDB;
    using HmDataDocument;
    using HMFrameWork.Ancestor;
    using HMFrameWork.Command;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Threading;

    /// <summary>
    /// Defines the <see cref="PrjListViewModel" />.
    /// </summary>
    public partial class PrjListViewModel : AViewModel
    {
        #region Fields

        /// <summary>
        /// Defines the projects.
        /// </summary>
        private List<ProjectModel> projects = new List<ProjectModel>();

        /// <summary>
        /// Defines the selectedData.
        /// </summary>
        private ProjectModel selectedData;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PrjListViewModel"/> class.
        /// </summary>
        public PrjListViewModel()
        {
            CommandViewLoaded = new RelayCommand(ViewLoaded);
            CommandListBoxSelectionChanged = new RelayCommand(ListBoxSelectionChanged);
            CommandSearchTextBoxTextChanged = new RelayCommand(SearchTextBoxTextChanged);
            CommandDataGridSelectionChanged = new RelayCommand(DataGridSelectionChanged);
            AddProject = new RelayCommand(addProject);
            CopyProject = new RelayCommand(copyProject);
            DeleteProject = new RelayCommand(deleteProject);
            Refresh = new RelayCommand(refresh);
        }

        private void refresh(object obj)
        {
            this.loadProjectInfo();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the AddProject.
        /// </summary>
        public ICommand AddProject { get; private set; }

        /// <summary>
        /// Gets the CommandDataGridSelectionChanged.
        /// </summary>
        public ICommand CommandDataGridSelectionChanged { get; private set; }

        /// <summary>
        /// Gets the CommandListBoxSelectionChanged.
        /// </summary>
        public ICommand CommandListBoxSelectionChanged { get; private set; }

        /// <summary>
        /// Gets the CommandSearchTextBoxTextChanged.
        /// </summary>
        public ICommand CommandSearchTextBoxTextChanged { get; private set; }

        /// <summary>
        /// Gets the CommandViewLoaded.
        /// </summary>
        public ICommand CommandViewLoaded { get; private set; }

        /// <summary>
        /// Gets the CopyProject.
        /// </summary>
        public ICommand CopyProject { get; private set; }

        /// <summary>
        /// Gets the DeleteProject.
        /// </summary>
        public ICommand DeleteProject { get; private set; }
        public ICommand Refresh { get; private set; }

        /// <summary>
        /// Gets the FilteredData
        /// Gets or sets the FilteredData..
        /// </summary>
        public ObservableCollection<ProjectModel> FilteredData { get; private set; } = new ObservableCollection<ProjectModel>();

        /// <summary>
        /// Gets or sets the SelectedData.
        /// </summary>
        public ProjectModel SelectedData
        {
            get { return selectedData; }
            set { SetValue(ref selectedData, value); }
        }

        public bool AddProjectExcuting { get; set; }

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
            App.GetViewModelManager().AddValue(typeof(PrjListViewModel), this);
            TransactionCtrl.Add_DBUpdateWndCtrl(this); // IDBUpdate를 상속받은 경우 필히 연결시켜줍니다.
        }

        /// <summary>
        /// The addProject.
        /// </summary>
        /// <param name="obj">The obj<see cref="object"/>.</param>
        private void addProject(object obj)
        {
            ProjectModel projectModel = new ProjectModel();
            projectModel.IsNew = true;
            this.projects.Insert(0, projectModel);

            Grid contentGrid = obj as Grid;
            WatermarkTextbox watermarkTextbox = contentGrid.FindName("searchTextBox") as WatermarkTextbox;
            watermarkTextbox.Text = string.Empty;
            this.SearchTextBoxTextChanged(watermarkTextbox);
            this.AddProjectExcuting = true;
        }

        /// <summary>
        /// The copyProject.
        /// </summary>
        /// <param name="obj">The obj<see cref="object"/>.</param>
        private void copyProject(object obj)
        {
            if (this.SelectedData != null && this.SelectedData.IsNew != true)
            {
                ProjectModel newData = new ProjectModel();
                newData.BorrowPit = this.SelectedData.BorrowPit;
                newData.Bridge = this.SelectedData.Bridge;
                newData.Cut = this.SelectedData.Cut;
                newData.FilePath = "COPIED:" + this.SelectedData.FilePath;
                newData.Fill = this.SelectedData.Fill;
                newData.HandAuger = this.SelectedData.HandAuger;
                newData.Order = this.SelectedData.Order;
                newData.Thumbnail = this.SelectedData.Thumbnail;
                newData.TrialPit = this.SelectedData.TrialPit;
                newData.Tunnel = this.SelectedData.Tunnel;
                newData.IsNew = true;

                this.projects.Insert(0, newData);

                Grid contentGrid = obj as Grid;
                WatermarkTextbox watermarkTextbox = contentGrid.FindName("searchTextBox") as WatermarkTextbox;
                watermarkTextbox.Text = string.Empty;
                this.SearchTextBoxTextChanged(watermarkTextbox);
                this.AddProjectExcuting = true;
            }
        }

        /// <summary>
        /// The createProjectModel.
        /// </summary>
        /// <param name="hinfoD">.</param>
        /// <returns>.</returns>
        private ProjectModel createProjectModel(DBDataHINFO hinfoD)
        {
            ProjectModel returnData = new ProjectModel();

            if (hinfoD.dicImage.Keys.Contains("PREVIEW"))
            {
                returnData.Thumbnail = ImageConverter.BitMapToBitmapImage(hinfoD.dicImage["PREVIEW"].img, System.Drawing.Imaging.ImageFormat.Png);
            }
            else
            {
                returnData.Thumbnail = "/Resources/Images/noPreview.png";
            }
            returnData.Name = hinfoD.dicStr["PROJECT_NAME"];
            returnData.Order = hinfoD.dicStr["COMPANY_NAME"];

            foreach (var item in hinfoD.dicInt)
            {
                string department = item.Key;
                switch (department)
                {
                    case "Fill":
                        returnData.Fill = item.Value;
                        break;
                    case "Bridge":
                        returnData.Bridge = item.Value;
                        break;
                    case "Cut":
                        returnData.Cut = item.Value;
                        break;
                    case "Tunnel":
                        returnData.Tunnel = item.Value;
                        break;
                    case "BorrowPit":
                        returnData.BorrowPit = item.Value;
                        break;
                    case "TrialPit":
                        returnData.TrialPit = item.Value;
                        break;
                    case "HandAuger":
                        returnData.HandAuger = item.Value;
                        break;
                }
            }

            return returnData;
        }

        /// <summary>
        /// The createProjectModel.
        /// </summary>
        /// <param name="filePath">The filePath<see cref="string"/>.</param>
        /// <returns>The <see cref="ProjectModel"/>.</returns>
        private ProjectModel createProjectModel(string filePath)
        {
            if (File.Exists(filePath))
            {
                DBDataHINFO hinfoD = new DBDataHINFO();
                ProjectModel returnData = new ProjectModel();
                Image image = new Image();
                if (!HmBaseDoc.Open_InfoData(filePath, ref hinfoD))
                {                    
                    returnData.Thumbnail = "/Resources/Images/noPreview.png";
                }
                else
                {
                    returnData = createProjectModel(hinfoD);
                    returnData.FilePath = filePath;
                }
                return returnData;
            }

            return null;
        }

        /// <summary>
        /// The DataGridSelectionChanged.
        /// </summary>
        /// <param name="obj">The obj<see cref="object"/>.</param>
        private void DataGridSelectionChanged(object obj)
        {
            Grid contentGrid = obj as Grid;
            DataGrid dataGrid = contentGrid.FindName("dataGrid") as DataGrid;
            if (dataGrid.Items.Count > 0 && dataGrid.SelectedCells.Count > 0)
            {
                DataGridCellInfo dataGridCell = dataGrid.CurrentCell;
                int rowIndex = dataGrid.Items.Count - 1;
                if (!this.FilteredData.IndexOf(dataGridCell.Item as ProjectModel).Equals(rowIndex))
                {
                    (contentGrid.FindName("imageListBox") as ListBox).ScrollIntoView(this.SelectedData);
                }
            }
        }

        /// <summary>
        /// The deleteProject.
        /// </summary>
        /// <param name="obj">The obj<see cref="object"/>.</param>
        private void deleteProject(object obj)
        {
            if (this.SelectedData != null)
            {
                if (MessageBox.Show(this.SelectedData.Name + "\n삭제 시 복구할 수 없습니다. 삭제하시겠습니까?", "Warning", MessageBoxButton.YesNo).Equals(MessageBoxResult.Yes))
                {
                    File.Delete(this.SelectedData.FilePath);
                    this.projects.Remove(this.SelectedData);
                    if (this.FilteredData.Contains(this.SelectedData))
                    {
                        this.FilteredData.Remove(this.SelectedData);
                    }
                }
            }
        }

        /// <summary>
        /// The ListBoxSelectionChanged.
        /// </summary>
        /// <param name="obj">The obj<see cref="object"/>.</param>
        private void ListBoxSelectionChanged(object obj)
        {
            if (this.SelectedData != null)
            {
                ((obj as Grid).FindName("dataGrid") as DataGrid).ScrollIntoView(this.SelectedData);
            }
        }

        /// <summary>
        /// The loadProjectInfo.
        /// </summary>
        private void loadProjectInfo()
        {
            this.projects.Clear();
            this.FilteredData.Clear();
            try
            {
                foreach (string f in Directory.GetFiles(App.WORKING_DIRECTORY).OrderByDescending(x => new FileInfo(x).LastWriteTime))
                {
                    if (Path.GetExtension(f).Equals(GaiaConstants.DATA_FILE_EXTENSION))
                    {
                        ProjectModel item = createProjectModel(f);
                        if (null != item)
                        {
                            this.projects.Add(item);
                            this.FilteredData.Add(item);
                        }
                    }
                }
                SearchTextBoxTextChanged(null);
            }
            catch (Exception e)
            {
                NotifyHelper.Instance.Show(NotifyHelper.NotiType.Error, e.ToString());
            }
        }

        /// <summary>
        /// The SearchTextBoxTextChanged.
        /// </summary>
        /// <param name="obj">The obj<see cref="object"/>.</param>
        private void SearchTextBoxTextChanged(object obj)
        {
            if (null != obj)
            {
                WatermarkTextbox watermarkTextbox = obj as WatermarkTextbox;
                this.FilteredData.Clear();
                this.projects.FindAll(x => x.Name != null && x.Name.Contains(watermarkTextbox.Text)).ForEach(x => this.FilteredData.Add(x));
            }
        }

        /// <summary>
        /// The ViewLoaded.
        /// </summary>
        /// <param name="obj">The obj<see cref="object"/>.</param>
        private void ViewLoaded(object obj)
        {
            this.loadProjectInfo();

            DBUpdate_All();
        }

        #endregion
    }
}
