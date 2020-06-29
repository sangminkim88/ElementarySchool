namespace GAIA2020.Design
{
    using GaiaDB;
    using HmDraw;
    using HmDraw.Entities;
    using HmDraw.View.Wpf;
    using HmGeometry;
    using LogStyle;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Teigha.Geometry;
    using Key = System.Windows.Input.Key;

    public partial class UDrillLog : UserControl
    {
        #region Fields

        private IDrillLog iLog;
        private ILoadComplete iloadComplete;

        private System.Windows.Point mouseDownPoint;

        #endregion

        #region Constructors

        public UDrillLog()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        public ViewControl Canvas
        {
            get { return canvas; }
        }
        #endregion

        #region Methods

        public void SetInterface(IDrillLog iLog)
        {
            this.iLog = iLog;
        }
        public void SetInterface(ILoadComplete iloadComplete)
        {
            this.iloadComplete = iloadComplete;
        }
        /// <summary>
        /// entity add/modify
        /// </summary>
        /// <param name="label">label</param>
        /// <param name="value">변경될 값</param>
        /// <param name="v">현재수정영역의 logtag</param>
        /// <param name="CalcTag">전체영역의 logtag</param>
        public void EntityUpsert(string label, string value, LogTag v, LogTag Allv, LogTag NVALUEv, double BegY)
        {
            // DB정보
            string keystr = CmdParmParser.GetValuefromKey(label, "db", '&');
            // SPTG polyline의 index
            string indexstr = CmdParmParser.GetValuefromKey(label, "index", '&');

            // update
            if (v.update)
            {                
                double py = 0.0;
                if (double.TryParse(value, out double y))
                {
                    py = Allv.box.leftY + (BegY - y) * (Allv.box.height / 20);
                }

                foreach (var t in GetEntityByLayer<HmText>(v.strlyr, label))
                {                    
                    HmBounds2D textBound = t.Geometry.Get_Bounds2D();                                        
                    switch (keystr)
                    {
                        // 상단 프로젝트정보
                        case "PROJ":
                            {
                                RedrawHmText(t, new HmPoint3D(textBound.UpperLeft.X, textBound.UpperLeft.Y, 0), value, label);
                                break;
                            }
                        // 심도,표고,두께
                        case "STRA":
                            {
                                if(string.Compare(t.Label, label) == 0)                               
                                {
                                    // 심도                                             
                                    string valstr = CmdParmParser.GetValuefromKey(label, "currtext", '&');
                                    label = label.Replace(valstr, value); // Label Update
                                    RedrawHmText(t, new HmPoint3D(textBound.LowerLeft.X, py + 2.8, 0.0), value, label);

                                    // 표고
                                    string lb = v.xdatastr + "&type=표고";
                                    if (canvas.Entities.GetEntitiesByLabel(lb).FirstOrDefault() is HmText eleEntity)
                                    {
                                        // 심도변경에따른 표고 재계산

                                        lb = label + "&type=표고"; // Label Update
                                        RedrawHmText(eleEntity, new HmPoint3D(textBound.LowerLeft.X + 10, py + 2.8, 0.0), "10.00", lb);
                                    }

                                    // 두께
                                    lb = v.xdatastr + "&type=두께";
                                    if (canvas.Entities.GetEntitiesByLabel(lb).FirstOrDefault() is HmText thickEntity)
                                    {
                                        // 심도변경에따른 두께 재계산

                                        lb = label + "&type=두께"; // Label Update
                                        RedrawHmText(thickEntity, new HmPoint3D(textBound.LowerLeft.X + 21, py + 2.8, 0.0), "10.00", lb);
                                    }
                                }
                                break;
                            }
                        // 타격회수/관입량/N-VALUE
                        case "SPTG":
                            {
                                if (string.Compare(t.Label, label) == 0)                                
                                {                                    
                                    RedrawHmText(t, new HmPoint3D(textBound.UpperLeft.X, textBound.UpperLeft.Y, 0), value, label); // text                              
                                    EntitySPTUpsert(v, NVALUEv, label, value, indexstr); // spt 그래프
                                    break;
                                }
                                break;
                            }
                    }
                }
            }
            // add
            else
            {
                EntityTextInsert(v, label, value);
                if(!string.IsNullOrEmpty(indexstr))
                {
                    EntitySPTUpsert(v, NVALUEv, label, value, indexstr);
                }                
            }                        
        }
        /// <summary>
        /// polyline entity add/modify
        /// </summary>
        /// <param name="label">label</param>
        /// <param name="value">변경될 값</param>
        /// <param name="v">현재수정영역의 logtsg</param>
        /// <param name="CalcTag">전체영역의 logtag</param>
        public void EntityPolylineUpsert(string label, string value, LogTag v, LogTag Allv, double BegY)
        {                        
            string keystr = CmdParmParser.GetValuefromKey(label, "db", '&');
            string varstr = CmdParmParser.GetValuefromKey(label, "var", '&');

            // polyline 수정이 필요없는 작업은 패스한다.
            if (varstr.Contains("타격회수") || keystr.Equals("PROJ")) return;

            if (v.update)
            {            
                // y값을 캐드상의 좌표로 계산
                double py = 0.0;
                if (double.TryParse(value, out double y))
                {
                    py = Allv.box.leftY + (BegY - y) * (Allv.box.height / 20);
                }

                foreach (var p in GetEntityByLayer<HmPolyline>(v.strlyr, label))
                {                    
                    HmBounds2D lineBound = p.Geometry.Get_Bounds2D();

                    switch (keystr)
                    {
                        case "STRA":
                            {
                                if(string.Compare(label, p.Label) == 0)
                                {
                                    if (p.Vertices.Count == 4)
                                    {
                                        p.Vertices[1].Point = new HmPoint2D(v.box.leftX, py);
                                        p.Vertices[2].Point = new HmPoint2D(v.box.leftX + v.box.width, py);
                                        p.Layer = v.strlyr;

                                        // Label Update                        
                                        string valstr = CmdParmParser.GetValuefromKey(label, "currtext", '&');
                                        label = label.Replace(valstr, value);
                                        p.Label = label;

                                        canvas.Entities.Update(p);
                                        break;
                                    }
                                    else
                                    {
                                        p.Vertices[0].Point = new HmPoint2D(v.box.leftX, py);
                                        p.Vertices[1].Point = new HmPoint2D(v.box.leftX + v.box.width, py);
                                        p.Layer = v.strlyr;

                                        // Label Update                        
                                        string valstr = CmdParmParser.GetValuefromKey(label, "currtext", '&');
                                        label = label.Replace(valstr, value);
                                        p.Label = label;

                                        canvas.Entities.Update(p);
                                        break;
                                    }
                                }

                                break;
                            }                        
                    }
                }
            }
            else
            {
                EntityPolylineInsert(v, label);
            }
        }

        public List<T> GetEntityByLayer<T>(string layerName, string labelName = "") where T : class
        {
#if true
            IEnumerable<HmEntity> hmEntities = canvas.Entities.Cast<HmEntity>().Where(x => x.Layer == layerName && x is T);
#else
            //IEnumerable<HmEntity> hmEntities = canvas.Entities.Cast<HmEntity>().Where(x => x.Layer == layerName && x.GetType().Equals(typeof(T)));
#endif
            if (!string.IsNullOrEmpty(labelName))
            {
                return hmEntities.Where(x => x.Label.Contains(labelName)).Cast<T>().ToList();
            }
            return hmEntities.Cast<T>().ToList();            
        }
        public HmPolyline GetEntityByLayerPoint(string layerName, double px, double py)
        {            
            IEnumerable<HmEntity> hmEntities = canvas.Entities.Cast<HmEntity>().Where(x => x.Layer == layerName);            
            foreach (var v in hmEntities)
            {
                if(v is HmPolyline line)
                {
                    if(line.Geometry.IsInsideEntity(new HmPoint2D(px, py)) >= 0)
                    {
                        return line;
                    }                    
                }
            }
            return null;
        }

        /// <summary>
        /// 레이어명과 포인트를 이용해 HmPolyline을 구한 후 그 위에 포함된 유일한 HmText를 구함
        /// </summary>
        /// <param name="layer">레이어명</param>
        /// <param name="px">x포인트</param>
        /// <param name="py">y포인트</param>
        /// <returns></returns>
        public HmText GetHmTextByLayerPoint(string layer, double px, double py)
        {
            if (layer != null)
            {
                var hmPolyline = canvas.Entities.ToList().FindAll(x => x is HmPolyline && x.Layer != null && x.Layer.Equals(layer) && x.Geometry.IsInsideEntity(new HmPoint2D(px, py)) >= 0);

                if (hmPolyline.Count().Equals(1))
                {
                    HmEntity tmp = this.canvas.Entities.ToList().FindAll(x => x is HmText && x.Layer != null && 
                    x.Layer.Equals(layer) && hmPolyline.First().Geometry.IsInsideEntity
                    (new HmPoint2D(x.Geometry.Get_Bounds2D().UpperLeft.X, x.Geometry.Get_Bounds2D().UpperLeft.Y)) >= 0).FirstOrDefault();

                    if(tmp != null)
                    {
                        return (HmText)tmp;
                    }
                }
            }
            return null;
        }

        public HmPoint3D World2Screen(double px, double py)
        {
            Point3d p3d = Aux.WorldToEye(canvas.GetViewPanel().Graphics.HelperDevice, px, py);
            return new HmPoint3D(p3d.X, p3d.Y, p3d.Z);
        }

        private void Canvas_LoadComplete()
        {
            canvas.GetViewPanel().Preset = new ViewPanelPreset()
            {
                IsDrawCursor = false,
                IsDrawSelectionRect = false,
                BackgroundColor = System.Drawing.Color.White,
                LogicalPalette = Teigha.GraphicsSystem.Device.LightPalette,
            };
            //
            iloadComplete?.iLoadComplete();
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ViewPanel v = canvas.GetViewPanel();
            this.mouseDownPoint = Mouse.GetPosition(v);

            if (null == v.Graphics) return;

            Point3d p3d = Aux.EyeToWorld(v.Graphics.HelperDevice, mouseDownPoint.X, mouseDownPoint.Y);
            iLog?.iMouseDown(mouseDownPoint.X, mouseDownPoint.Y, p3d.X, p3d.Y, p3d.Z);
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ViewPanel v = canvas.GetViewPanel();
            System.Windows.Point p = Mouse.GetPosition(v);

            if (null == v.Graphics) return;

            if (mouseDownPoint.Equals(p))
            {
                Point3d p3d = Aux.EyeToWorld(v.Graphics.HelperDevice, p.X, p.Y);
                iLog?.iMouseUp(p.X, p.Y, p3d.X, p3d.Y, p3d.Z);
            }
        }

        private void Canvas_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Escape))
            {
                iLog?.iEditorHide();
            }
        }

        /// <summary>
        /// 신규 text 추가
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="label"></param>
        /// <param name="value"></param>
        /// <param name="logTag"></param>
        private void EntityTextInsert(LogTag v, string label, string value)
        {
            layerUpsert(v.strlyr);
            HmPoint3D pt = new HmPoint3D();
            Color color = Color.Empty;
            if (label.Contains("심도표고두께"))
            {
                pt.Set(new HmPoint2D(v.box.leftX + 2.8, v.box.leftY + 0.8));
                color = Color.Red;
            }
            else if(label.Contains("타격회수"))
            {
                pt.Set(new HmPoint2D(v.box.leftX + 2.0, v.box.leftY));
                color = Color.DarkGray;
            }
            else
            {
                pt.Set(new HmPoint2D(v.box.leftX, v.box.leftY - v.box.height / 2));
                color = Color.Magenta;
            }
            HmText textEntity = new HmText(pt, 2.0, value)
            {
                Label = label,
                Justify = v.justify,
                Layer = v.strlyr,
                Color = color
            };

            canvas.Entities.Add(textEntity);
            RemoveBound(textEntity.Geometry.Get_Bounds2D(), "#SPT"); // SPT 양식바운더리제거
        }
        /// <summary>
        /// 신규 polyline 추가
        /// </summary>
        /// <param name="layer">layer</param>
        /// <param name="label">label</param>
        /// <param name="logTag">logtag</param>
        private void EntityPolylineInsert(LogTag v, string label)
        {
            layerUpsert(v.strlyr);
            HmPolyline2D poly = new HmPolyline2D();
            poly.AddVertex(new HmPoint2D(v.box.leftX, v.box.leftY));
            poly.AddVertex(new HmPoint2D(v.box.leftX + v.box.width, v.box.leftY));            

            HmPolyline polyEntity = new HmPolyline(poly)
            {
                Label = label,
                Layer = v.strlyr,
                Color = Color.Red
            };

            canvas.Entities.Add(polyEntity);
        }
        /// <summary>
        /// 심도 재계산시 Text 변경
        /// </summary>
        /// <param name="exisText">기존 Textentity</param>
        /// <param name="pt">다시 그릴 위치</param>
        /// <param name="value">다시 그릴 값</param>
        /// <param name="label">entity label</param>
        private void RedrawHmText(HmText exisText, HmPoint3D pt, string value, string label)
        {
            // 기존 Text제거
            canvas.Entities.Remove(exisText);

            HmText vv = new HmText(pt, 2.0, value)
            {
                Label = label,
                Layer = exisText.Layer,
                Color = exisText.Color
            };
            canvas.Entities.Add(vv);
        }

        /// <summary>
        /// SPT point/polyline 추가/수정함수
        /// </summary>
        /// <param name="v">logtag</param>
        /// <param name="label">entity label</param>
        /// <param name="value">값</param>
        /// <param name="indexStr">spt 그래프 polyline vertices인덱스</param>
        /// <
        private void EntitySPTUpsert(LogTag v, LogTag NVALUEv, string label, string value, string indexStr = "")
        {            
            // 위치 계산
            double sptValue = Convert.ToDouble(value.Split(GaiaConstants.FIRST_DELIMITER)[0]);
            double px = NVALUEv.box.leftX + sptValue * NVALUEv.box.width / 50;

            HmCircle sptEntity = null;
            HmPolyline lineEntity = null;

            // point 검색을위한 label셋팅
            label = label.Replace("타격회수", "NVALUE");

            // 수정시
            if (v.update) 
            {
                // label로 entity검색후 point 변경
                sptEntity = canvas.Entities.GetEntitiesByLabel(label).FirstOrDefault() as HmCircle;
                sptEntity.Center = new HmPoint3D(px, sptEntity.Center.Y, 0);
                canvas.Entities.Update(sptEntity);

                // polyline 검색을위한 label셋팅
                label = "&db=SPTG&var=NVALUE";

                // polyline 변경
                if (string.IsNullOrEmpty(indexStr)) return;

                int idx = Convert.ToInt32(indexStr); // polyline index                
                lineEntity = canvas.Entities.GetEntitiesByLabel(label).FirstOrDefault() as HmPolyline;
                lineEntity.Vertices[idx].Point = new HmPoint2D(px, lineEntity.Vertices[idx].Point.Y);
                canvas.Entities.Update(lineEntity);
            }
            // 추가시
            else
            {
                // point 추가
                sptEntity = new HmCircle(new HmPoint3D(px, v.box.leftY, 0), 0.3)
                {
                    Layer = v.strlyr,
                    Label = label,
                    Color = Color.DarkGray
                };
                canvas.Entities.Add(sptEntity);

                // polyline 검색을위한 label셋팅
                label = "&db=SPTG&var=NVALUE";

                // polyline 추가                
                lineEntity = canvas.Entities.GetEntitiesByLabel(label).FirstOrDefault() as HmPolyline;
                // 없으면 생성
                if (lineEntity == null)
                {
                    lineEntity = new HmPolyline
                    {
                        Label = label,
                        Color = Color.DarkGray
                    };
                    lineEntity.Vertices.Add(new HmVertex2D(px, v.box.leftY));
                    canvas.Entities.Add(lineEntity);
                }
                // 있으면 절점 추가
                else
                {
                    lineEntity.Vertices.Add(new HmVertex2D(px, v.box.leftY));
                    canvas.Entities.Update(lineEntity);                    
                }                                               
            }
        }
        private void layerUpsert(string strlyr)
        {
            HmLayer hmLayer = new HmLayer(strlyr);
            hmLayer.Label = strlyr;
            canvas.Layers.Add(hmLayer);
        }
        /// <summary>
        /// 시료형태와 타격횟수에 entity가 추가되면 양식바운더리를 제거하는 함수
        /// </summary>
        /// <param name="entityBound">추가한 엔티티의 바운드</param>
        /// <param name="layer"></param>
        private void RemoveBound(HmBounds2D entityBound, string layer)
        {
            if (entityBound == null) return;

            IEnumerable<HmEntity> hmEntities = from e in canvas.Entities.Cast<HmEntity>()
                                               where e.Layer == layer
                                               select e;

            foreach (var v in hmEntities)
            {
                if (v is HmPolyline)
                {
                    HmPolyline bound = v as HmPolyline;
                    if (bound.GetBounds().Include(entityBound) == 2)
                    {
                        canvas.Entities.Remove(v);
                        break;
                    }
                }
            }
        }
#endregion

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
#if false
            // 언로드후에도 파일을 잡고 있는 문제가 발생하여 해결하고자 여기에서 Dispose() 호출
            if (!canvas.GetDocument().GetDatabase().IsDisposed)
            {
                canvas.GetDocument().GetDatabase().Dispose();
            }
#endif
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/477e7e74-ccbf-4498-8ab9-ca2f3b836597/how-to-know-when-a-wpf-usercontrol-is-closing?forum=wpf
            var tmpWindow = Window.GetWindow(this);
            if (tmpWindow != null)
            {
                Window window = tmpWindow;
                window.Closing += W_Closing;
            }
        }

        private void W_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //throw new System.NotImplementedException();
            // 언로드후에도 파일을 잡고 있는 문제가 발생하여 해결하고자 여기에서 Dispose() 호출
            if (!canvas.GetDocument().GetDatabase().IsDisposed)
            {
                canvas.GetDocument().GetDatabase().Dispose();
            }
        }
    }
}
