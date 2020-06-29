namespace GAIA2020.Manager
{
    using GAIA2020.Design;
    using GaiaDB.Enums;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="GlobalVar" />.
    /// </summary>
    public class GlobalVar
    {
        #region Fields

        // 현재 스타일에서 보고 있는 주상도 키값
        //
        /// <summary>
        /// Defines the Key4LogStyle.
        /// </summary>
        private Dictionary<eDepartment, DrillProperty> key4LogStyle = null;

        // 현재 사용중인 로그스타일
        /// <summary>
        /// Defines the _logKind.
        /// </summary>
        private eDepartment _logKind;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalVar"/> class.
        /// </summary>
        public GlobalVar()
        {
            Init();// 편집중인 디비키 초기화
            logKind = eDepartment.Fill;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the logKind.
        /// </summary>
        public eDepartment logKind
        {
            get { return _logKind; }
            set { _logKind = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 해당 주상도 스타일에서 마지막으로 편집했던 주상도 키를 얻는다.
        /// </summary>
        /// <param name="e">주상도스타일.</param>
        /// <returns>.</returns>
        public DrillProperty GetLogKey(eDepartment e)
        {
            DrillProperty pageData = null;
            // 해당스타일에서 마지막으로 보고 있던 주상도 키
            if (key4LogStyle.ContainsKey(e))
                pageData = key4LogStyle[e];
            //
            return pageData;
        }
        //public DrillProperty GetLogKey()
        //{
        //    return GetLogKey(logKind);
        //}
        public uint GetLogKey()
        {
            uint ukey = 0;
            DrillProperty dp = GetLogKey(logKind);
            if (null != dp)
                ukey = dp.Ukey;

            return ukey;
        }

        /// <summary>
        /// 로그스타일에서 편집중인 디비키를 모두 무효화한다.
        /// </summary>
        public void Init()
        {
            key4LogStyle = new Dictionary<eDepartment, DrillProperty>();
        }

        /// <summary>
        /// The SetLogKey.
        /// </summary>
        /// <param name="drillProperty">The drillProperty<see cref="DrillProperty"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool SetLogKey(DrillProperty drillProperty)
        {
            return SetLogKey(logKind, drillProperty);
        }

        /// <summary>
        /// 해당스타일에서 편집하고 있는 주상도 키를 저장한다.
        /// </summary>
        /// <param name="e">로그스타일.</param>
        /// <param name="drillProperty">디비키.</param>
        /// <returns>.</returns>
        public bool SetLogKey(eDepartment e, DrillProperty drillProperty)
        {
            if (key4LogStyle.ContainsKey(e))
                key4LogStyle[e] = drillProperty;
            else
                key4LogStyle.Add(e, drillProperty);
            //
            return true;
        }

        #endregion
    }
}
