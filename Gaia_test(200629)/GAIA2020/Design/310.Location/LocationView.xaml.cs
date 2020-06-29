namespace GAIA2020.Design
{
    using GaiaDB;
    using HmDataDocument;
    using HmDraw;
    using HmDraw.Entities;
    using HMFrameWork.Ancestor;
    using HmGeometry;
    using HMViewHmDraw;
    using Microsoft.Win32;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Windows;
    using Teigha.GraphicsSystem;

    /// <summary>
    /// Defines the <see cref="LocationView" />.
    /// </summary>
    public partial class LocationView : AUserControl
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationView"/> class.
        /// </summary>
        public LocationView()
        {
            InitializeComponent();
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
            App.GetViewManager().AddValue(typeof(LocationView), this);
        }

        /// <summary>
        /// The AUserControl_Loaded.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private void AUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window.Closing += (object s, CancelEventArgs ce) =>
            {
                //if (!canvas.GetDocument().GetDatabase().IsDisposed)
                //{
                //    canvas.GetDocument().GetDatabase().Dispose();
                //}
            };
        }
        
        
        /// <summary>
        /// The Button_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private void LoadDwgButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "현황도 불러오기";
            openFileDialog.Filter = "CAD파일(*.DWG, *.DXF)|*.dwg;*.dxf;|모든파일(*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                DBDataPOSI posi = new DBDataPOSI();
                posi.bExternalFile = true;
                posi.strFileName = openFileDialog.FileName;

                string strEpsgFileName = HmGeometry.TextUtil.Get_FilePathName(posi.strFileName) + ".epsg";
                if(System.IO.File.Exists(strEpsgFileName))
                {
                    using (StreamReader sr = new StreamReader(strEpsgFileName))
                    {
                        string strbuf = sr.ReadLine();
                        int epsg = 0;
                        int.TryParse(strbuf, out epsg);
                        posi.geoInfo.Set_EPSGCode(epsg);
                    }
                }

                DBDoc.Get_CurrDoc().posi.Add_TR(posi);
            }
        }

        #endregion

        private void ModelViewCtrl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!aloadflag)
            {
                aloadflag = true; // 한번만 호출하자
                ((LocationViewModel)DataContext).Set_ViewCtrl(modelViewCtrl);
            }
        }

        private void Fit_Click(object sender, RoutedEventArgs e)
        {
            this.modelViewCtrl.ZoomFitView();
        }
    }
}
