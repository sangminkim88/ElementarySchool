namespace GAIA2020.Utilities
{
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Defines the <see cref="DatagridUtil" />.
    /// </summary>
    public static class DatagridUtil
    {
        #region Methods

        /// <summary>
        /// The GetCell.
        /// </summary>
        /// <param name="dataGrid">The dataGrid<see cref="DataGrid"/>.</param>
        /// <param name="rowContainer">The rowContainer<see cref="DataGridRow"/>.</param>
        /// <param name="column">The column<see cref="int"/>.</param>
        /// <returns>The <see cref="DataGridCell"/>.</returns>
        public static DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
        {
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = VTHelper.FindVisualChildren<DataGridCellsPresenter>(rowContainer).ToList().FirstOrDefault();
                if (presenter == null)
                {
                    rowContainer.ApplyTemplate();
                    presenter = VTHelper.FindVisualChildren<DataGridCellsPresenter>(rowContainer).ToList().FirstOrDefault();
                }
                if (presenter != null)
                {
                    DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    if (cell == null)
                    {
                        dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                        cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    }
                    return cell;
                }
            }
            return null;
        }

        #endregion
    }
}
