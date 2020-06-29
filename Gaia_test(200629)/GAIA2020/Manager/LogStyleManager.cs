using GAIA2020.Utilities;
using GaiaDB.Enums;
using HmDraw;
using LogStyle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;

namespace GAIA2020.Manager
{
    public class LogStyleManager
    {
        // 로그스타일 목록 파일 경로
        private string stylefname = string.Empty;

        // 로그목록
        private Dictionary<Tuple<eDepartment, bool>, Tuple<string, double>> styleList = null;

        public LogStyleManager()
        {
            styleList = new Dictionary<Tuple<eDepartment, bool>, Tuple<string, double>>();
            this.stylefname = "";
            LoadStyle(stylefname);
        }
        public LogStyleManager(string strfname)
        {
            styleList = new Dictionary<Tuple<eDepartment, bool>, Tuple<string, double>>();
            this.stylefname = strfname;
            LoadStyle(stylefname);
        }

        /// <summary>
        /// 스타일목록읽기
        /// </summary>
        private void LoadStyle(string strfname)
        {
            try
            {
                if(!File.Exists(strfname))
                    strfname = ConfigHelper.GetProgramFilePath(Path.ChangeExtension(ConfigHelper.GetExeuteFileName(), ".style"));
                string path = strfname;
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
                if (!File.Exists(path))
                {
                    using (FileStream fs = File.Create(path))
                    {
                        // 다음 사용을 위해 파일닫기
                        fs.Dispose();
                    }
                }

                string[] flines = File.ReadAllLines(path);
                foreach (string f in flines)
                {
                    eDepartment e;
                    string strkey = Utilities.CmdParmParser.GetKeyfromString(f);

                    if (Enum.TryParse<eDepartment>(strkey, out e))
                    {
                        string valstr = Utilities.CmdParmParser.GetValuefromKey(f, strkey);
                        if (File.Exists(valstr))
                        {
                            styleList.Add(new Tuple<eDepartment, bool>(e, false), new Tuple<string, double>(valstr, 20));
                        }                       
                    }
                }

                foreach (eDepartment item in Enum.GetValues(typeof(eDepartment)))
                {
                    // 일반주상도
                    if (styleList.ContainsKey(new Tuple<eDepartment, bool>(item, false)))
                    { }
                    else
                    {
                        switch (item)
                        {
                            case eDepartment.Fill:
                                styleList.Add(new Tuple<eDepartment, bool>(item, false), new Tuple<string, double>(@".\Resources\Dwg\SB.dwg", 20));
                                break;
                            case eDepartment.Bridge:
                                styleList.Add(new Tuple<eDepartment, bool>(item, false), new Tuple<string, double>(@".\Resources\Dwg\BB.dwg", 20));
                                break;
                            case eDepartment.Cut:
                                styleList.Add(new Tuple<eDepartment, bool>(item, false), new Tuple<string, double>(@".\Resources\Dwg\CB.dwg", 20));
                                break;
                            case eDepartment.Tunnel:
                                styleList.Add(new Tuple<eDepartment, bool>(item, false), new Tuple<string, double>(@".\Resources\Dwg\TB.dwg", 20));
                                break;
                            case eDepartment.BorrowPit:
                                styleList.Add(new Tuple<eDepartment, bool>(item, false), new Tuple<string, double>(@".\Resources\Dwg\BB.dwg", 20));
                                break;
                            case eDepartment.TrialPit:
                                styleList.Add(new Tuple<eDepartment, bool>(item, false), new Tuple<string, double>(@".\Resources\Dwg\BB.dwg", 20));
                                break;
                            case eDepartment.HandAuger:
                                styleList.Add(new Tuple<eDepartment, bool>(item, false), new Tuple<string, double>(@".\Resources\Dwg\BB.dwg", 20));
                                break;
                        }
                    }
                    //범례주상도
                    if (styleList.ContainsKey(new Tuple<eDepartment, bool>(item, true)))
                    { }
                    else
                    {
                        switch (item)
                        {
                            case eDepartment.Fill:
                                styleList.Add(new Tuple<eDepartment, bool>(item, true), new Tuple<string, double>(@".\Resources\Dwg\SB_범례.dwg", 15));
                                break;
                            case eDepartment.Bridge:
                                styleList.Add(new Tuple<eDepartment, bool>(item, true), new Tuple<string, double>(@".\Resources\Dwg\BB_범례.dwg", 15));
                                break;
                            case eDepartment.Cut:
                                styleList.Add(new Tuple<eDepartment, bool>(item, true), new Tuple<string, double>(@".\Resources\Dwg\CB_범례.dwg", 15));
                                break;
                            case eDepartment.Tunnel:
                                styleList.Add(new Tuple<eDepartment, bool>(item, true), new Tuple<string, double>(@".\Resources\Dwg\TB_범례.dwg", 15));
                                break;
                            case eDepartment.BorrowPit:
                                styleList.Add(new Tuple<eDepartment, bool>(item, true), new Tuple<string, double>(@".\Resources\Dwg\BB_범례.dwg", 15));
                                break;
                            case eDepartment.TrialPit:
                                styleList.Add(new Tuple<eDepartment, bool>(item, true), new Tuple<string, double>(@".\Resources\Dwg\BB_범례.dwg", 15));
                                break;
                            case eDepartment.HandAuger:
                                styleList.Add(new Tuple<eDepartment, bool>(item, true), new Tuple<string, double>(@".\Resources\Dwg\BB_범례.dwg", 15));
                                break;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                NotifyHelper.Instance.Show(NotifyHelper.NotiType.Error, e.ToString());
            }
        }

        /// <summary>
        /// 주어진 스타일의 도면이름 가져오기
        /// </summary>
        /// <param name="e">스타일이름</param>
        /// <param name="legend">범례유무</param>
        /// <returns></returns>
        public string GetLogStyleFileName(eDepartment e, bool legend = false)
        {
            string fileName = string.Empty;
#if true
            Tuple<string, double> v = GetLogStylefr(e, legend);
            if (null != v)
                fileName = v.Item1;
#else
            Tuple<eDepartment, bool> tp = new Tuple<eDepartment, bool>(e, legend);
            if (styleList.ContainsKey(tp))
            {
                fileName = styleList[tp].Item1;
            }
#endif
            return fileName;
        }

        /// <summary>
        /// 주어진 스타일 가져오기
        /// </summary>
        /// <param name="e">스타일이름</param>
        /// <param name="legend">범례유무</param>
        /// <returns></returns>
        public Tuple<string, double> GetLogStylefr(eDepartment e, bool legend)
        {
            Tuple<string, double> v = null;
            //
            Tuple<eDepartment, bool> tp = new Tuple<eDepartment, bool>(e, legend);
            if (styleList.ContainsKey(tp))
            {
                v = styleList[tp];
            }
            //
            return v;
        }

        /// <summary>
        /// 주어진 스타일의 도면을 임시폴더에 저장하고 저장위치를 돌려준다
        /// </summary>
        /// <param name="e">스타일이름</param>
        /// <param name="legend">범례유무</param>
        /// <returns></returns>
        public string GetLogStyle(eDepartment e, bool legend = false)
        {
            string fileName = string.Empty;
            //
            string styleName = GetLogStyleFileName(e, legend);
            if (File.Exists(styleName))
            {
                // 임시파일이름
#if true
                string tempName = ConfigHelper.GetProgramFilePath(Path.ChangeExtension(ConfigHelper.GetExeuteFileName(), ".dwg"));
#else
                string tempName = System.IO.Path.GetTempFileName();
#endif
                // 임시폴더를 찾는다
                string folderName = System.IO.Path.GetTempPath(); //ConfigHelper.GetMyDocumentPath(Path.ChangeExtension(ConfigHelper.GetExeuteFileName(), ".style"));
                if(File.Exists(tempName))
                    File.Delete(tempName);
                // 임시폴더에 스타일도면을 복사하고 파일명을 리턴한다
                File.Copy(styleName, tempName);

                fileName = Path.Combine(Path.GetDirectoryName(tempName), Path.GetFileName(tempName));
            }

            return fileName;
        }

        /// <summary>
        /// 작업파일(임시파일)이름을 구한다
        /// </summary>
        /// <param name="styleName"></param>
        /// <returns></returns>
        public string GetLogFileNameTemp(string styleName)
        {
            string fileName = string.Empty;
            //
            if (File.Exists(styleName))
            {
                // 임시파일이름
#if true
                string tempName = ConfigHelper.GetProgramFilePath(Path.ChangeExtension(ConfigHelper.GetExeuteFileName(), ".dwg"));
#else
                string tempName = System.IO.Path.GetTempFileName();
#endif
                fileName = Path.Combine(Path.GetDirectoryName(tempName), Path.GetFileName(tempName));
            }

            return fileName;
        }

        public LogStyleBase GetLogStyle(eDepartment e, string styleName, string fileName)
        {
            LogStyleBase logstyle = null;
            HmDocument document = new HmDocument();
            document.Open(true, styleName);
            switch (e)
            {
                case eDepartment.Fill:
                    logstyle = new LogSBStyle();
                    logstyle.ReadStyle(document);
                    break;
                case eDepartment.Bridge:
                    logstyle = new LogBBStyle();
                    logstyle.ReadStyle(document);
                    break;
                case eDepartment.Cut:
                    logstyle = new LogCBStyle();
                    logstyle.ReadStyle(document);
                    break;
                case eDepartment.Tunnel:
                    logstyle = new LogTBStyle();
                    logstyle.ReadStyle(document);
                    break;
                case eDepartment.BorrowPit:
                    logstyle = new LogBBStyle();
                    logstyle.ReadStyle(document);
                    break;
                case eDepartment.TrialPit:
                    logstyle = new LogBBStyle();
                    logstyle.ReadStyle(document);
                    break;
                case eDepartment.HandAuger:
                    logstyle = new LogBBStyle();
                    logstyle.ReadStyle(document);
                    break;
            }
            // 스타일 읽고 저장
            document.SaveAs(fileName);
            //
            document.StartNew();
            return logstyle;
        }

        #region 로그스타일 주상도 길이
        /// <summary>
        /// 해당스타일의 주상도를 가져온다
        /// </summary>
        /// <param name="e">스타일</param>
        /// <param name="legend">범례유무</param>
        /// <returns>Tuple(파일이름, 주상도길이)</returns>
        private Tuple<string, double> LoadStyle(eDepartment e, bool legend)
        {
            Tuple<string, double> v = null;
            // 로그스타일 도면 찾기
            //LogStyleManager styleManager = App.GetLogStyleManager();
            v = GetLogStylefr(e, legend);
            //
            return v;
        }

        /// <summary>
        /// 해당스타일의 일반과 범례주상도를 가져온다
        /// </summary>
        /// <param name="e">스타일</param>
        /// <returns>Tuple(파일이름, 주상도길이)</returns>
        private List<Tuple<string, double>> LoadStyle(eDepartment e)
        {
            List<Tuple<string, double>> vlist = new List<Tuple<string, double>>();
            // 로그스타일 도면 찾기
            Tuple<string, double> v = null;
            //LogStyleManager styleManager = App.GetLogStyleManager();
            // 일반주상도
            v = GetLogStylefr(e, false);
            if (null != v)
                vlist.Add(v);
            // 범례주상도
            v = GetLogStylefr(e, true);
            if (null != v)
                vlist.Add(v);
            //
            return vlist;
        }

        /// <summary>
        /// 해당스타일의 일반과 범례주상도 높이를 가져온다
        /// </summary>
        /// <param name="e">스타일</param>
        /// <returns>Tuple(일반주상도길이, 범례주상도길이)</returns>
        public Tuple<double, double> GetStyleHeight(eDepartment e)
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
        #endregion
    }
}
