namespace GAIA2020.Design
{
    using GAIA2020.Manager;
    using GaiaDB;
    using GaiaDB.Enums;
    using HmDraw;
    using HmGeometry;
    using LogStyle;
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Defines the <see cref="UDrillLogView" />.
    /// </summary>
    public partial class UDrillLogView : ILoadComplete
    {
        #region Fields

        /// <summary>
        /// Defines the aloading.
        /// </summary>
        private bool aloading = false;

        /// <summary>
        /// Defines the dlogstyle.
        /// </summary>
        private LogStyleBase dlogstyle = null;

        #endregion

        #region Methods

        /// <summary>
        /// The Assign.
        /// </summary>
        public void Assign()
        {
            dlog.Canvas.LoadComplete += Canvas_LoadComplete;
        }

        /// <summary>
        /// The iLoadComplete.
        /// </summary>
        public void iLoadComplete()
        {
            if (aloading)
                return;

            // iLoadComplete  실행되었음을 표시
            aloading = true;

            // 주상도 그리기
            DBDoc doc = DBDoc.Get_CurrDoc();
            HmDBKey dbkey = doc.Get_ActiveDrillLog(true);
            Draw(dbkey);
        }

        /// <summary>
        /// The Load.
        /// </summary>
        /// <param name="logstyle">The logstyle<see cref="LogStyleBase"/>.</param>
        /// <param name="strfname">The strfname<see cref="string"/>.</param>
        /// <param name="minPosi">The minPosi<see cref="HmGeometry.HmPoint3D"/>.</param>
        /// <param name="maxPosi">The maxPosi<see cref="HmGeometry.HmPoint3D"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool Load(LogStyleBase logstyle, string strfname, HmGeometry.HmPoint3D minPosi = null, HmGeometry.HmPoint3D maxPosi = null)
        {
            // 파일열기
            dlog.Canvas.Open(strfname);
            if (minPosi != null && maxPosi != null) dlog.Canvas.ZoomExtents(minPosi, maxPosi, 1.0);

            // 스타일
            dlogstyle = logstyle;

            // XData를 위해 파일을 열고나서 반드시 초기화
            HmRegApp regApp = new HmRegApp("Label");
            dlog.Canvas.RegApps.Add(regApp);

            return true;
        }

        /// <summary>
        /// The UnLoad.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool UnLoad()
        {
            HmDocument document = dlog.Canvas.GetDocument();
            string fileName = Path.Combine(Directory.GetCurrentDirectory() + @"\Resources\Dwg\setting.dwg");
            document?.New(false, fileName);

            return true;
        }

        /// <summary>
        /// The Canvas_LoadComplete.
        /// </summary>
        private void Canvas_LoadComplete()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 로그스타일 로딩전 처리함수
        /// </summary>
        private void BfrLoadStyle()
        {
            HideControl();

            // 기존 파일 닫기
            UnLoad();
        }
        /// <summary>
        /// 로그스타일 로딩후 처리함수
        /// </summary>
        private void AftLoadStyle()
        {

        }

        /// <summary>
        /// The LoadStyle.
        /// </summary>
        /// <param name="e">The e<see cref="eDepartment"/>.</param>
        /// <param name="legend">범례유무</param>
        /// <param name="bZoomExtents">The bZoomExtents<see cref="bool"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool LoadStyle(eDepartment e, bool legend = false, bool bZoomExtents = true)
        {
            // 로그스타일 도면 찾기
            LogStyleManager styleManager = App.GetLogStyleManager();
            string styleName = styleManager.GetLogStyleFileName(e, legend);
            if (File.Exists(styleName))
            {
                HmGeometry.HmPoint3D minPosi = null;
                HmGeometry.HmPoint3D maxPosi = null;
                if (!bZoomExtents)
                {// UnLoad작업 직후에는 HmDraw.Aux.EyeToWorld 사용불가
                    Teigha.Geometry.Point3d pBL = HmDraw.Aux.EyeToWorld(dlog.Canvas.GetGraphics().HelperDevice, 0.0, dlog.Canvas.ActualHeight);
                    Teigha.Geometry.Point3d pTR = HmDraw.Aux.EyeToWorld(dlog.Canvas.GetGraphics().HelperDevice, dlog.Canvas.ActualWidth, 0.0);
                    minPosi = new HmGeometry.HmPoint3D(pBL.X, pBL.Y, pBL.Z);
                    maxPosi = new HmGeometry.HmPoint3D(pTR.X, pTR.Y, pTR.Z);
                }
                // 스타일 로딩 전처리
                BfrLoadStyle();

                // 작업파일(임시작업 파일)이름 찾기
                string fileName = styleManager.GetLogFileNameTemp(styleName);
                // 스타일읽고 임시파일로 작업파일 생성
                LogStyleBase logstyle = styleManager.GetLogStyle(e, styleName, fileName);
                // 사용할 로그 스타일지정과 작업파일열기
                Load(logstyle, fileName, minPosi, maxPosi);

                // 스타일 로딩 후처리
                AftLoadStyle();

                return true;
            }
            return false;
        }

#if false //LogStyleManager 로 이동
        /// <summary>
        /// 해당스타일의 주상도를 가져온다
        /// </summary>
        /// <param name="e">스타일</param>
        /// <param name="legend">범례유무</param>
        /// <returns></returns>
        private Tuple<string, double> LoadStyle(eDepartment e, bool legend)
        {
            Tuple<string, double> v = null;
            // 로그스타일 도면 찾기
            LogStyleManager styleManager = App.GetLogStyleManager();
            v = styleManager.GetLogStylefr(e, legend);
            //
            return v;
        }

        /// <summary>
        /// 해당스타일의 일반과 범례주상도를 가져온다
        /// </summary>
        /// <param name="e">스타일</param>
        /// <returns></returns>
        private List<Tuple<string, double>> LoadStyle(eDepartment e)
        {
            List<Tuple<string, double>> vlist = new List<Tuple<string, double>>();
            // 로그스타일 도면 찾기
            Tuple<string, double> v = null;
            LogStyleManager styleManager = App.GetLogStyleManager();
            // 일반주상도
            v = styleManager.GetLogStylefr(e, false);
            if (null != v)
                vlist.Add(v);
            // 범례주상도
            v = styleManager.GetLogStylefr(e, true);
            if (null != v)
                vlist.Add(v);
            //
            return vlist;
        }

        /// <summary>
        /// 해당스타일의 일반과 범례주상도 높이를 가져온다
        /// </summary>
        /// <param name="e">스타일</param>
        /// <returns></returns>
        private Tuple<double, double> GetStyleHeight(eDepartment e)
        {
            List<Tuple<string, double>> vlist = LoadStyle(e);
            double height1 = 20.0, height2 = 20.0;
            // 일반주상도
            if (vlist.Count > 0)
                height1 = vlist[0].Item2;
            // 범례주상도
            if (vlist.Count > 1)
                height2 = vlist[1].Item2;
            Tuple<double, double> v = new Tuple<double, double>(height1, height2);
            //
            return v;
        }
#endif
        #endregion
    }
}
