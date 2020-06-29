using HMFrameWork.Ancestor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GaiaDB;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using HmGeometry;
using GAIA2020.Utilities;

namespace GAIA2020.Design
{
    public partial class UcJointShapeViewModel : AViewModel, INotifyPropertyChanged
    {
        private HmBitmap m_jshpImage;
        private List<HmLine2D> m_jshpLines;

        public UcJointShapeViewModel()
        {
            m_jshpImage = null;
            m_jshpLines = new List<HmLine2D>();

            nCannyThres1 = 100;
            nCannyThres2 = 200;
        }

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            HmDataDocument.TransactionCtrl.Add_DBUpdateWndCtrl(this); // IDBUpdate를 상속받은 경우 필히 연결시켜줍니다. 
            DBUpdate_All();
        }

        #region 속성

        private HmDBKey hmDBKey;
        public HmDBKey HmDBKey
        {
            get { return hmDBKey; }
            set { SetValue(ref hmDBKey, value); }
        }

        private ImageSource img;
        public ImageSource Img
        {
            get { return img; }
            set { SetValue(ref img, value); }
        }

        private int ncannyThres1;
        public int nCannyThres1
        {
            get { return ncannyThres1; }
            set
            { SetValue(ref ncannyThres1, value); }
        }

        private int ncannyThres2;
        public int nCannyThres2
        {
            get { return ncannyThres2; }
            set
            { SetValue(ref ncannyThres2, value); }
        }
        #endregion

        /// <summary>
        /// 절리형상 사진 update
        /// </summary>
        public void UpdateHoughLines()
        {
            if (null == m_jshpImage)
            { return; }

            m_jshpLines.Clear();
            m_jshpLines = GetHoughLinesP(m_jshpImage, nCannyThres1, nCannyThres2);

            UpdateImage();
        }

        public void UpdateDrawLines(HmLine2D line)
        {
            if (null == m_jshpImage)
            { return; }

            m_jshpLines.Add(line);

            UpdateImage(line);
        }

        public void ClearLines()
        {
            if (null == m_jshpImage)
            { return; }

            m_jshpLines.Clear();

            UpdateImage();
        }

        public void UpdateImage()
        {
            if (null == m_jshpImage)
            { this.Img = null; return; }

            System.Drawing.Bitmap image = m_jshpImage.Clone().img;
            System.Drawing.Pen redPen = new System.Drawing.Pen(System.Drawing.Color.Red, 2);

            foreach (var line in m_jshpLines)
            {
                using (var graphics = System.Drawing.Graphics.FromImage(image))
                {
                    graphics.DrawLine(redPen, (float)line.StartPoint.X, (float)line.StartPoint.Y, (float)line.EndPoint.X, (float)line.EndPoint.Y);
                }
            }

            this.Img = ImageConverter.BitMapToBitmapImage(image, System.Drawing.Imaging.ImageFormat.Png);
        }

        public void UpdateImage(HmLine2D drawline)
        {
            if (null == m_jshpImage)
            { this.Img = null; return; }

            System.Drawing.Bitmap image = m_jshpImage.Clone().img;
            System.Drawing.Pen redPen = new System.Drawing.Pen(System.Drawing.Color.Red, 2);

            foreach (var line in m_jshpLines)
            {
                using (var graphics = System.Drawing.Graphics.FromImage(image))
                {
                    graphics.DrawLine(redPen, (float)line.StartPoint.X, (float)line.StartPoint.Y, (float)line.EndPoint.X, (float)line.EndPoint.Y);
                }
            }

            // drawline
            using (var graphics = System.Drawing.Graphics.FromImage(image))
            {
                graphics.DrawLine(redPen, (float)drawline.StartPoint.X, (float)drawline.StartPoint.Y, (float)drawline.EndPoint.X, (float)drawline.EndPoint.Y);
            }

            this.Img = ImageConverter.BitMapToBitmapImage(image, System.Drawing.Imaging.ImageFormat.Png);
        }

        public void SetImage(ImageSource imgSrc)
        {
            this.Img = imgSrc;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// OnPropertyChanged
        /// </summary>
        /// <param name="info"></param>
        protected void OnPropertyChanged([CallerMemberName]string info = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        /// <summary>
        /// SetValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetValue<T>(ref T backingField, T value, [CallerMemberName]string propertyName = "")
        {
            if (object.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        #endregion

        #region Reset delegate function

        UcJointShapeView.DelegateHandler handlerReset;

        public void SetDelegateHandler(UcJointShapeView.DelegateHandler handlerReset)
        {
            this.handlerReset = handlerReset;
        }

        #endregion
    }
}
