namespace LogStyle
{
    using HmDraw;
    using HmDraw.Entities;
    using HmGeometry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Defines the <see cref="LogBox" />.
    /// </summary>
    public class LogBox
    {
        #region Fields

        /// <summary>
        /// Defines the leftX, leftY.
        /// </summary>
        public double leftX, leftY;// 좌상단 실제좌표

        /// <summary>
        /// Defines the width, height.
        /// </summary>
        public double width, height;// 폭과 높이

        #endregion

        #region Methods

        /// <summary>
        /// The GetBound.
        /// </summary>
        /// <returns>The <see cref="HmBounds2D"/>.</returns>
        public HmBounds2D GetBound()
        {
            HmBounds2D bound = new HmBounds2D(leftX, leftX + width, leftY - height, leftY);
            return bound;
        }

        /// <summary>
        /// The IsInside.
        /// </summary>
        /// <param name="x">The x<see cref="double"/>.</param>
        /// <param name="y">The y<see cref="double"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool IsInside(double x, double y)
        {
            if (leftX < x && x < leftX + width)
            {
                if (leftY > y && y > leftY - height)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The Set.
        /// </summary>
        /// <param name="lx">The lx<see cref="double"/>.</param>
        /// <param name="ly">The ly<see cref="double"/>.</param>
        /// <param name="w">The w<see cref="double"/>.</param>
        /// <param name="h">The h<see cref="double"/>.</param>
        public void Set(double lx, double ly, double w, double h)
        {
            leftX = lx;
            leftY = ly;
            width = w;
            height = h;
        }

        /// <summary>
        /// The Set.
        /// </summary>
        /// <param name="b">The b<see cref="LogBox"/>.</param>
        public void Set(LogBox b)
        {
            leftX = b.leftX;
            leftY = b.leftY;
            width = b.width;
            height = b.height;
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="LogStyleBase" />.
    /// </summary>
    public abstract partial class LogStyleBase
    {
        #region Fields

        /// <summary>
        /// Defines the baseDepth.
        /// </summary>
        public double baseDepth;// 시작 로깅레벨

        /// <summary>
        /// Defines the currPageNo.
        /// </summary>
        public int currPageNo;// 현재 페이지

        /// <summary>
        /// Defines the intvSpt.
        /// </summary>
        public double intvSpt;// SPT 간격

        /// <summary>
        /// Defines the maxNValue.
        /// </summary>
        public int maxNValue;// 최대 N값

        /// <summary>
        /// Defines the normalDepth.
        /// </summary>
        public double normalDepth;// 일반 주상도 길이

        /// <summary>
        /// Defines the prefix.
        /// </summary>
        public string prefix;// 공번 접두어

        /// <summary>
        /// Defines the thisDepth.
        /// </summary>
        public double thisDepth;// 현재 로깅깊이

        /// <summary>
        /// Defines the totalPages.
        /// </summary>
        public int totalPages;// 전제 페이지

        /// <summary>
        /// Defines the 기타사항.
        /// </summary>
        public LogTag 기타사항;

        /// <summary>
        /// Defines the 발주처.
        /// </summary>
        public LogTag 발주처;

        /// <summary>
        /// Defines the 사업명.
        /// </summary>
        public LogTag 사업명;

        /// <summary>
        /// Defines the 시추각도.
        /// </summary>
        public LogTag 시추각도;

        /// <summary>
        /// Defines the 시추공경.
        /// </summary>
        public LogTag 시추공경;

        /// <summary>
        /// Defines the 시추공번.
        /// </summary>
        public LogTag 시추공번;

        /// <summary>
        /// Defines the 시추방법.
        /// </summary>
        public LogTag 시추방법;

        /// <summary>
        /// Defines the 시추심도.
        /// </summary>
        public LogTag 시추심도;

        /// <summary>
        /// Defines the 시추위치.
        /// </summary>
        public LogTag 시추위치;

        /// <summary>
        /// Defines the 시추자.
        /// </summary>
        public LogTag 시추자;

        /// <summary>
        /// Defines the 시추장비.
        /// </summary>
        public LogTag 시추장비;

        /// <summary>
        /// Defines the 시추표고.
        /// </summary>
        public LogTag 시추표고;

        /// <summary>
        /// Defines the 영역.
        /// </summary>
        public LogTag 영역;

        /// <summary>
        /// Defines the 작성자.
        /// </summary>
        public LogTag 작성자;

        /// <summary>
        /// Defines the 전체페이지.
        /// </summary>
        public LogTag 전체페이지;

        /// <summary>
        /// Defines the 조사일.
        /// </summary>
        public LogTag 조사일;

        /// <summary>
        /// Defines the 좌표.
        /// </summary>
        public LogTag 좌표;

        /// <summary>
        /// Defines the 지하수위.
        /// </summary>
        public LogTag 지하수위;

        /// <summary>
        /// Defines the 케이싱심도.
        /// </summary>
        public LogTag 케이싱심도;

        /// <summary>
        /// Defines the 해머효율.
        /// </summary>
        public LogTag 해머효율;

        /// <summary>
        /// Defines the 현재페이지.
        /// </summary>
        public LogTag 현재페이지;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LogStyleBase"/> class.
        /// </summary>
        public LogStyleBase()
        {
            currPageNo = 0; totalPages = 1;
            baseDepth = 0.0;
            thisDepth = 20.0;
            maxNValue = 50;
            //
            영역 = new LogTag();
            //
            사업명 = new LogTag(0);
            발주처 = new LogTag(30);

            시추공번 = new LogTag(20);
            시추장비 = new LogTag(40);
            시추위치 = new LogTag(50);
            시추표고 = new LogTag(60);
            시추심도 = new LogTag();
            시추방법 = new LogTag(120);
            시추공경 = new LogTag(130);
            시추각도 = new LogTag(160);
            지하수위 = new LogTag(180);
            케이싱심도 = new LogTag(140);
            좌표 = new LogTag(170);
            시추자 = new LogTag(70);
            작성자 = new LogTag(110);
            조사일 = new LogTag(typeof(DateTime), 150);
            해머효율 = new LogTag(80);
            기타사항 = new LogTag(190);

            현재페이지 = new LogTag();
            전체페이지 = new LogTag();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 주어진 위치에 저정된 변수를 검색한다.
        /// </summary>
        /// <param name="x">.</param>
        /// <param name="y">.</param>
        /// <param name="z">.</param>
        /// <returns>.</returns>
        public Tuple<string, LogTag> GetBox(double x, double y, double z = 0.0)
        {
#if true
            FieldInfo[] fs = GetFields();
            var f = fs.Where(a => a.FieldType.Equals(typeof(LogTag)));
            foreach (var v in f)

#else
            foreach (var v in GetFields().Where(a=>a.FieldType.Equals(typeof(LogTag))))
#endif
            {
                object obj = v.GetValue(this);
                if (obj == null)
                    continue;
                LogTag logTag = obj as LogTag;
                if (logTag.category > DrillLogCategory.DLOG_CATEGORY_ROI && logTag.IsInside(x, y))
                {
                    return new Tuple<string, LogTag>(logTag.valkey, logTag);
                }
            }
            return null;
        }

        /// <summary>
        /// 도면에 저장된 변수의 영역을 검색한다(변수는 레이어명으로 구분).
        /// </summary>
        /// <param name="document">.</param>
        /// <param name="layerName">레이어.</param>
        /// <returns>.</returns>
        public LogTag GetBox(HmDocument document, string layerName)
        {
            LogTag logTag = null;

            IEnumerable<HmEntity> hmEntities = from e in document.Entities.Cast<HmEntity>()
                                               where e.Layer == layerName
                                               select e;
            List<HmEntity> vlist = new List<HmEntity>();
            // 찾은경우
            if (hmEntities.Count() > 0)
            {
                logTag = new LogTag();
            }
            // 영역계산
            foreach (var v in hmEntities)
            {
                HmGeometry.HmBounds2D bounds = v.Geometry.Get_Bounds2D();
                logTag.box.leftX = bounds.xMin;
                logTag.box.leftY = bounds.yMax;
                logTag.box.width = bounds.Width;
                logTag.box.height = bounds.Height;
#if false
                if (v.GetType() == typeof(HmPolyline))
#endif
                vlist.Add(v);
            }

            // 엔티티삭제
            document.Entities.Remove(vlist);// hmEntities.ToList());

            return logTag;
        }

        /// <summary>
        /// <summary>
        /// 도면에 저장된 변수의 영역을 주어진 변수에 업데이트한다(변수는 레이어명으로 구분).
        /// </summary>
        /// <param name="document">.</param>
        /// <param name="layerName">레이어.</param>
        /// <param name="logTag">.</param>
        /// <returns>.</returns>
        public bool GetBox(HmDocument document, string layerName, ref LogTag logTag)
        {
            if (logTag == null)
                return false;

            IEnumerable<HmEntity> hmEntities = from e in document.Entities.Cast<HmEntity>()
                                               where e.Layer == layerName
                                               select e;
            List<HmEntity> vlist = new List<HmEntity>();
            // 찾은 경우 영역계산
            foreach (var v in hmEntities)
            {
                HmGeometry.HmBounds2D bounds = v.Geometry.Get_Bounds2D();
                logTag.box.leftX = bounds.xMin;
                logTag.box.leftY = bounds.yMax;
                logTag.box.width = bounds.Width;
                logTag.box.height = bounds.Height;
#if false
                if (v.GetType() == typeof(HmPolyline))
#endif
                vlist.Add(v);
            }

            // 엔티티삭제
            document.Entities.Remove(vlist);// hmEntities.ToList());

            return vlist.Count > 0 ? true : false;
        }

        /// <summary>
        /// 주어진 위치에 저정된 변수를 검색한다.
        /// </summary>
        /// <param name="x">.</param>
        /// <param name="y">.</param>
        /// <param name="z">.</param>
        /// <returns>.</returns>
        public Dictionary<string, LogTag> GetBoxes(double x, double y, double z = 0.0)
        {
            Dictionary<string, LogTag> vlist = new Dictionary<string, LogTag>();
            FieldInfo[] f = GetFields();
            foreach (var v in f)
            {
                if (v.FieldType == typeof(LogTag))
                {
                    object obj = v.GetValue(this);
                    if (obj == null)
                        continue;
                    LogTag logTag = obj as LogTag;
                    if (logTag.category > DrillLogCategory.DLOG_CATEGORY_ROI && logTag.IsInside(x, y))
                    {
                        vlist.Add(logTag.valkey, logTag);
                    }
                }
            }

            return vlist;
        }

        /// <summary>
        /// The GetFields.
        /// </summary>
        /// <param name="flags">The flags<see cref="BindingFlags"/>.</param>
        /// <returns>The <see cref="FieldInfo[]"/>.</returns>
        public virtual FieldInfo[] GetFields(BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            Type type = GetLogType();// typeof(LogBBStyle);

            //FieldInfo[] f = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo[] f = type.GetFields(flags);

            return f;
        }

        /// <summary>
        /// The GetHolePrefix.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public virtual string GetHolePrefix()
        {
            return prefix;
        }

        /// <summary>
        /// The GetLogCategory.
        /// </summary>
        /// <param name="strkey">The strkey<see cref="string"/>.</param>
        /// <returns>The <see cref="(string strlyr, DrillLogCategory category)"/>.</returns>
        public virtual (string strlyr, DrillLogCategory category) GetLogCategory(string strkey)
        {
            return ("", DrillLogCategory.DLOG_CATEGORY_NONE);
        }

        /// <summary>
        /// 해당 카테고리에 포함되는 컨트롤을 찾는다.
        /// </summary>
        /// <param name="categories">카테고리.</param>
        /// <returns>.</returns>
        public virtual List<LogTag> GetLogTag(List<DrillLogCategory> categories)
        {
            List<LogTag> vlist = new List<LogTag>();
            //
            FieldInfo[] fs = GetFields();
            var f = fs.Where(a => a.FieldType.Equals(typeof(LogTag)));
            foreach (var v in f)
            {
                object obj = v.GetValue(this);
                if (obj == null)
                    continue;
                LogTag logTag = obj as LogTag;
                if (categories.Contains(logTag.category) && !logTag.TabIndex.Equals(-1))
                {
                    // 해당 카테고리만
                    vlist.Add(logTag);
                }
            }
            //
            return vlist.OrderBy(x => x.TabIndex).ToList();
        }

        /// <summary>
        /// 주어진 변수의 영역을 검색한다.
        /// </summary>
        /// <param name="varName">변수명.</param>
        /// <returns>.</returns>
        public LogTag GetLogTag(string varName)
        {
            LogTag v = null;
            //
            FieldInfo[] f = GetFields();
            FieldInfo e = f.FirstOrDefault(x => x.Name == varName);
            if (null != e)
            {
                v = e.GetValue(this) as LogTag;
            }

            return v;
        }

        /// <summary>
        /// The GetLogType.
        /// </summary>
        /// <returns>The <see cref="Type"/>.</returns>
        public virtual Type GetLogType()
        {
            return GetType();
        }

        /// <summary>
        /// 시추종료 메시지를 작성해준다.
        /// </summary>
        /// <param name="depth">최종시추깊이.</param>
        /// <returns>.</returns>
        public virtual string GetMessageBoringTerminate(double depth)
        {
            string msgstr = string.Format("*심도 {0:F2}m에서 시추종료", depth);
            return msgstr;
        }

        /// <summary>
        /// 해당 카테고리에 포함되는 항목중에서 주어진(현재) 항목 다음을 찾는다.
        /// </summary>
        /// <param name="categories">카테고리.</param>
        /// <param name="curr">현재항목.</param>
        /// <returns>.</returns>
        public virtual LogTag GetNextLogTag(List<DrillLogCategory> categories, LogTag curr)
        {
            LogTag logTag = GetNextLogTag(categories, curr.valkey);
            return logTag;
        }

        /// <summary>
        /// The GetNextLogTag.
        /// </summary>
        /// <param name="categories">The categories<see cref="List{DrillLogCategory}"/>.</param>
        /// <param name="curr">The curr<see cref="string"/>.</param>
        /// <returns>The <see cref="LogTag"/>.</returns>
        public virtual LogTag GetNextLogTag(List<DrillLogCategory> categories, string curr)
        {
            LogTag logTag = null;
            //
            List<LogTag> vlist = GetLogTag(categories);
            // 현재항목과 같은 항목의 인덱스를 구하고
            int idx = vlist.FindIndex(x => x.valkey == curr);
            if (idx >= 0)
            {
                // 다음 항목으로 이동
                idx++;
                idx = idx % vlist.Count; // 원형으로 탐색
                logTag = vlist[idx];
            }
            //
            return logTag;
        }

        /// <summary>
        /// The GetValue.
        /// </summary>
        /// <param name="xdata">The xdata<see cref="List{string}"/>.</param>
        /// <param name="strkey">The strkey<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetValue(List<string> xdata, string strkey)
        {
            string valstr = CmdParmParser.GetValuefromKey(xdata, strkey);
            return valstr;
        }

        /// <summary>
        /// The GetValue.
        /// </summary>
        /// <param name="xdata">The xdata<see cref="string"/>.</param>
        /// <param name="strkey">The strkey<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetValue(string xdata, string strkey)
        {
            string valstr = CmdParmParser.GetValuefromKey(xdata, strkey);
            return valstr;
        }

        /// <summary>
        /// 주어진 레이어명으로 저장된 문자열을 추출한다.
        /// </summary>
        /// <param name="document">.</param>
        /// <param name="layerName">레이어명.</param>
        /// <returns>.</returns>
        public List<string> GetVar(HmDocument document, string layerName)
        {
            IEnumerable<HmEntity> hmEntities = from e in document.Entities.Cast<HmEntity>()
                                               where e.Layer == layerName
                                               select e;
            List<string> slist = new List<string>();
            List<HmEntity> vlist = new List<HmEntity>();
            // 찾은 경우 영역계산
            foreach (var v in hmEntities)
            {
                if (v.GetType() == typeof(HmText))
                {
                    HmText text = v as HmText;
                    if (null != v)
                    {
                        slist.Add(text.TextString);
                    }
                    vlist.Add(v);
                }
            }

            // 엔티티삭제
            document.Entities.Remove(vlist);// hmEntities.ToList());

            return slist;
        }

        /// <summary>
        /// The ReadStyle.
        /// </summary>
        /// <param name="document">The document<see cref="HmDocument"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public virtual bool ReadStyle(HmDocument document)
        {
            //

            return true;
        }

        /// <summary>
        /// The ReadStyle.
        /// </summary>
        /// <param name="strfname">The strfname<see cref="string"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public virtual bool ReadStyle(string strfname)
        {
            //

            return true;
        }

        /// <summary>
        /// The ReadVar.
        /// </summary>
        /// <param name="document">The document<see cref="HmDocument"/>.</param>
        /// <param name="strlyr">The strlyr<see cref="string"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public virtual bool ReadVar(HmDocument document, string strlyr)
        {
            // 일반주상도 길이
            normalDepth = 20.0;
            // 현재주상도 길이
            thisDepth = 20.0;
            // SPT간격
            intvSpt = 1.0;
            // 최대 SPT N값
            maxNValue = 50;

            return true;
        }

        /// <summary>
        /// The SetCurrPage.
        /// </summary>
        /// <param name="page">The page<see cref="int"/>.</param>
        public void SetCurrPage(int page)
        {
            currPageNo = page;
        }

        #endregion
    }

    /// <summary>
    /// Defines the <see cref="LogTag" />.
    /// </summary>
    public class LogTag
    {
        #region Fields

        /// <summary>
        /// Defines the box.
        /// </summary>
        public LogBox box;// 위치 및 범위

        /// <summary>
        /// Defines the category.
        /// </summary>
        public DrillLogCategory category;// 분류(종류)

        /// <summary>
        /// Defines the justify.
        /// </summary>
        public AttachmentPoint justify;// 정렬방식

        /// <summary>
        /// Defines the strlyr.
        /// </summary>
        public string strlyr;// 이름(레이어)

        /// <summary>
        /// Defines the TabIndex.
        /// </summary>
        public int TabIndex = -1;// 정렬방식

        /// <summary>
        /// Defines the type.
        /// </summary>
        public Type type;

        /// <summary>
        /// Defines the update.
        /// </summary>
        public bool update;// 수정 또는 추가

        /// <summary>
        /// Defines the valkey.
        /// </summary>
        public string valkey;// 키

        /// <summary>
        /// Defines the xdatastr.
        /// </summary>
        public string xdatastr;// XData로 저장할 값

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LogTag"/> class.
        /// </summary>
        public LogTag()
        {
            update = true;
            valkey = string.Empty;
            strlyr = string.Empty;
            xdatastr = string.Empty;
            type = typeof(string);
            box = new LogBox();
            justify = AttachmentPoint.MiddleLeft;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogTag"/> class.
        /// </summary>
        /// <param name="tabIndex">The tabIndex<see cref="int"/>.</param>
        public LogTag(int tabIndex) : this()
        {
            this.TabIndex = tabIndex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogTag"/> class.
        /// </summary>
        /// <param name="type">The type<see cref="Type"/>.</param>
        public LogTag(Type type) : this()
        {
            this.type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogTag"/> class.
        /// </summary>
        /// <param name="type">The type<see cref="Type"/>.</param>
        /// <param name="tabIndex">The tabIndex<see cref="int"/>.</param>
        public LogTag(Type type, int tabIndex) : this()
        {
            this.type = type;
            this.TabIndex = tabIndex;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The After.
        /// </summary>
        public void After()
        {
        }

        /// <summary>
        /// The Clone.
        /// </summary>
        /// <returns>The <see cref="LogTag"/>.</returns>
        public LogTag Clone()
        {
            LogTag o = new LogTag();
            o.type = this.type;
            o.valkey = this.valkey;
            o.xdatastr = this.xdatastr;
            o.strlyr = this.strlyr;
            o.box.Set(this.box);
            o.category = this.category;
            o.justify = this.justify;

            return o;
        }

        /// <summary>
        /// The IsInside.
        /// </summary>
        /// <param name="x">The x<see cref="double"/>.</param>
        /// <param name="y">The y<see cref="double"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool IsInside(double x, double y)
        {
            return box.IsInside(x, y);
        }

        /// <summary>
        /// The Set.
        /// </summary>
        /// <param name="key">The key<see cref="string"/>.</param>
        /// <param name="lyr">The lyr<see cref="string"/>.</param>
        public void Set(string key, string lyr)
        {
            valkey = key;
            strlyr = lyr;
        }

        /// <summary>
        /// The Set.
        /// </summary>
        /// <param name="key">The key<see cref="string"/>.</param>
        /// <param name="lyr">The lyr<see cref="string"/>.</param>
        /// <param name="cat">The cat<see cref="DrillLogCategory"/>.</param>
        public void Set(string key, string lyr, DrillLogCategory cat)
        {
            valkey = key;
            strlyr = lyr;
            category = cat;
        }

        #endregion
    }
}
