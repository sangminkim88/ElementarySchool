namespace GAIA2020.Design
{
    using GAIA2020.Enums;
    using GAIA2020.Frame;
    using GAIA2020.Models;
    using GAIA2020.Utilities;
    using GaiaDB;
    using GaiaDB.Enums;
    using HmDataDocument;
    using HMFrameWork.Ancestor;
    using HMFrameWork.Helper;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// Defines the <see cref="PrjListView" />.
    /// </summary>
    public partial class PrjListView : AUserControl
    {
        #region Fields

        /// <summary>
        /// Defines the addCount.
        /// </summary>
        private int addCount = 1;

        /// <summary>
        /// Defines the editing.
        /// </summary>
        private bool editing;

        /// <summary>
        /// Defines the oldValue.
        /// </summary>
        private string oldValue;

        /// <summary>
        /// Defines the radioButtons.
        /// </summary>
        private List<RadioButton> radioButtons = new List<RadioButton>();

        /// <summary>
        /// Defines the selectedDepartment.
        /// </summary>
        private eDepartment selectedDepartment = eDepartment.None;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PrjListView"/> class.
        /// </summary>
        public PrjListView()
        {
            InitializeComponent();
            this.radioButtons.Add(this.checkboardFormat);
            this.radioButtons.Add(this.listFormat);
            this.listFormat.IsChecked = true;
        }

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
        }

        /// <summary>
        /// The AUserControl_Loaded.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private void AUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.pathLable.Content = App.WORKING_DIRECTORY;
        }

        /// <summary>
        /// The ChangeDirectoryMenuItem_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private void ChangeDirectoryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            setWorkingDirectory();
        }

        /// <summary>
        /// The DataControl_KeyDown.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="KeyEventArgs"/>.</param>
        private void DataControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (!editing && e.Key.Equals(Key.Enter))
            {
                ProjectModel projectModel;
                if (sender is DataGrid dataGrid)
                {
                    projectModel = dataGrid.SelectedItem as ProjectModel;
                }
                else
                {
                    projectModel = imageListBox.SelectedItem as ProjectModel;
                }
                if (projectModel.IsNew != true)
                {
                    setProjectModelAndShowMainFrame(projectModel);
                }
            }
            else if (e.Key.Equals(Key.Delete))
            {
                this.deleteButton.Command.Execute(null);
            }
            else if (e.Key.Equals(Key.C) && Keyboard.Modifiers.Equals(ModifierKeys.Control))
            {
                this.copyButton.Command.Execute(this.contentGrid);
            }
        }

        /// <summary>
        /// The DataGrid_BeginningEdit.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="DataGridBeginningEditEventArgs"/>.</param>
        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            this.oldValue = (e.Column.GetCellContent(e.Row) as TextBlock).Text;
            this.editing = true;
        }

        /// <summary>
        /// The DataGrid_CellEditEnding.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="DataGridCellEditEndingEventArgs"/>.</param>
        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            TextBox target = e.Column.GetCellContent(e.Row) as TextBox;
            string newValue = target.Text;
            if (!newValue.Equals(oldValue))
            {
                ProjectModel projectModel = ((sender as DataGrid).SelectedItem as ProjectModel);
                if (projectModel.IsNew == true)
                {
                    if (e.Column.Header.Equals("사업명"))
                    {
                        if (newValue.Equals(string.Empty))
                        {
                            NotifyHelper.Instance.Show(NotifyHelper.NotiType.Error, "처음 생성한 프로젝트는 사업명을 입력해주셔야 합니다.");
                            e.Cancel = true;
                            return;
                        }

                        //복사 명령어를 이용한 새로운 프로젝트
                        if (!projectModel.FilePath.Equals(string.Empty) && projectModel.FilePath.StartsWith("COPIED:"))
                        {
                            string newFilePath = FileUtil.GetCheckedDuplicateFilePath(App.WORKING_DIRECTORY, newValue, GaiaConstants.DATA_FILE_EXTENSION);
                            try
                            {
                                System.IO.File.Copy(projectModel.FilePath.Replace("COPIED:", ""), newFilePath);
                            }
                            catch (Exception)
                            {
                                NotifyHelper.Instance.Show(NotifyHelper.NotiType.Error, "파일을 생성하지 못했습니다.");
                                target.Text = oldValue;
                                e.Cancel = true;
                                return;
                            }
                            projectModel.FilePath = newFilePath;
                        }
                        else
                        {
                            projectModel.FilePath = FileUtil.GetCheckedDuplicateFilePath(App.WORKING_DIRECTORY, newValue, GaiaConstants.DATA_FILE_EXTENSION);
                            projectModel.Thumbnail = "/Resources/Images/noPreview.png";

                            try
                            {
                                DBDoc doc = DBDoc.Get_CurrDoc();
                                doc.New_Document(false, projectModel.FilePath);
                                DBDataPROJ projD = new DBDataPROJ();
                                projD.ProjectName = newValue;
                                projD.CompanyName = projectModel.Order;
                                doc.proj.Add_Data(1, projD); // TR발생시키지 않음
                                doc.Save_Document("");
                            }
                            catch (Exception)
                            {
                                NotifyHelper.Instance.Show(NotifyHelper.NotiType.Error, "파일을 생성하지 못했습니다.");
                                target.Text = oldValue;
                                e.Cancel = true;
                                return;
                            }
                        }

                        projectModel.IsNew = false;
                    }
                    else
                    {
                        NotifyHelper.Instance.Show(NotifyHelper.NotiType.Error, "처음 생성한 프로젝트는 사업명을 먼저 입력해주셔야 합니다.");
                        target.Text = oldValue;
                        e.Cancel = true;
                        return;
                    }
                }

                try
                {
                    DBDoc doc = new DBDoc(false);
                    doc.Open_Document(projectModel.FilePath);

                    DBDataHINFO dbDataHINFO = null;
                    doc.hinfo.Get_Data(ref dbDataHINFO);
                    DBDataPROJ dBDataPROJ = null;
                    doc.proj.Get_Data(ref dBDataPROJ);
                    string header = (sender as DataGrid).CurrentCell.Column.Header.ToString();
                    if (header.Equals("사업명"))
                    {
                        dbDataHINFO.dicStr["PROJECT_NAME"] = newValue;
                        dBDataPROJ.ProjectName = newValue;
                    }
                    else
                    {
                        dbDataHINFO.dicStr["COMPANY_NAME"] = newValue;
                        dBDataPROJ.CompanyName = newValue;
                    }
                    doc.hinfo.Modify_Data(1, dbDataHINFO);
                    doc.proj.Modify_Data(1, dBDataPROJ);
                    doc.Save_Document(string.Empty);
                }
                catch (Exception)
                {

                }
            }
            this.editing = false;
        }

        /// <summary>
        /// The DataGrid_LoadingRow.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="DataGridRowEventArgs"/>.</param>
        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            PrjListViewModel viewModel = this.DataContext as PrjListViewModel;
            DataGrid dataGrid = sender as DataGrid;
            if (viewModel.AddProjectExcuting && dataGrid.Items.Count.Equals(addCount++))
            {
                DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(0) as DataGridRow;
                if (row != null)
                {
                    DataGridCell cell = DatagridUtil.GetCell(dataGrid, row, 0);
                    if (cell != null)
                        cell.Focus();
                }
                dataGrid.SelectedIndex = 0;
                viewModel.AddProjectExcuting = false;
                addCount = 1;
            }
        }

        /// <summary>
        /// The DataGridRow_MouseDoubleClick.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="MouseButtonEventArgs"/>.</param>
        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            setProjectModelAndShowMainFrame((sender as DataGridRow).Item as ProjectModel);
        }

        /// <summary>
        /// The Border_MouseLeftButtonUp.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="MouseButtonEventArgs"/>.</param>
        private void Department_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            selectedDepartment = (eDepartment)Enum.Parse(typeof(eDepartment), (sender as Border).Tag.ToString());
        }

        /// <summary>
        /// The DirectoryLable_MouseUp.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="MouseButtonEventArgs"/>.</param>
        private void DirectoryLable_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.setWorkingDirectory();
        }

        /// <summary>
        /// The ImageListBox_MouseDoubleClick.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="MouseButtonEventArgs"/>.</param>
        private void ImageListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            setProjectModelAndShowMainFrame((sender as ListBox).SelectedItem as ProjectModel);
        }

        /// <summary>
        /// The RadioButton_Checked.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton checkedRadioButton = sender as RadioButton;

            foreach (RadioButton radioButton in this.radioButtons)
            {
                SolidColorBrush brush = (radioButton.Equals(checkedRadioButton) ? FindResource("Theme.Color1") : FindResource("Theme.Color4")) as SolidColorBrush;
                foreach (var item in ((Grid)radioButton.Content).Children)
                {
                    if (item is Rectangle)
                    {
                        ((Rectangle)item).Fill = brush;
                    }
                }
            }

            if (checkedRadioButton.Name.Equals("listFormat"))
            {
                this.imageListBox.Tag = 1;
            }
            else
            {
                this.imageListBox.Tag = 2;
            }
        }

        /// <summary>
        /// The setProjectModelAndShowMainFrame.
        /// </summary>
        /// <param name="projectModel">The projectModel<see cref="ProjectModel"/>.</param>
        private void setProjectModelAndShowMainFrame(ProjectModel projectModel)
        {
            if (projectModel.IsNew == true)
            {
                NotifyHelper.Instance.Show(NotifyHelper.NotiType.Information, "처음 생성한 프로젝트는 사업명을 입력해주셔야 합니다.");
                return;
            }
            MainFrameView mainFrameView = WindowHelper.GetTypedWindow(typeof(MainFrameView)) as MainFrameView;
            if (null == mainFrameView)
            {
                mainFrameView = new MainFrameView();
            }

            if (this.selectedDepartment.Equals(eDepartment.None))
            {
                this.selectedDepartment = eDepartment.Fill;
            }

            mainFrameView.Show();

            DBDoc.Get_CurrDoc().Open_Document(projectModel.FilePath);

            DBDoc.Get_CurrDoc().Set_ActiveStyle(this.selectedDepartment);

            this.selectedDepartment = eDepartment.None;
        }

        /// <summary>
        /// The setWorkingDirectory.
        /// </summary>
        private void setWorkingDirectory()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = App.WORKING_DIRECTORY;
                if (dialog.ShowDialog().Equals(System.Windows.Forms.DialogResult.OK))
                {
                    App.WORKING_DIRECTORY = dialog.SelectedPath;
                    ConfigHelper.WriteProfileString(eConfigSection.User, eConfigKey.Directory, dialog.SelectedPath);
                    this.pathLable.Content = dialog.SelectedPath;
                }
            }
        }

#endregion

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem.Header.Equals("추가"))
            {
                this.addButton.Command.Execute(this.contentGrid);
            }
            else if (menuItem.Header.Equals("복사"))
            {
                this.copyButton.Command.Execute(this.contentGrid);
            }
            else
            {
                this.deleteButton.Command.Execute(null);
            }
        }
    }
}
