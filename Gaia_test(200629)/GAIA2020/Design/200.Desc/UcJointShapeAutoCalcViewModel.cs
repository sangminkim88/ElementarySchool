using GaiaDB;
using HMFrameWork.Ancestor;
using HmGeometry;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GAIA2020.Design
{
    public class UcJointShapeAutoCalcViewModel : AViewModel, INotifyPropertyChanged
    {
        public UcJointShapeAutoCalcViewModel()
        {
            nCannyThres1 = 100;
            nCannyThres2 = 200;
            nHoughLineThres = 50;
            nHoughLineMinLenThres = 50;
            nHoughLineMaxGapThres = 20;
        }

        #region 속성
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

        private int nhoughLineThres;
        public int nHoughLineThres
        {
            get { return nhoughLineThres; }
            set
            { SetValue(ref nhoughLineThres, value); }
        }

        private int nhoughLineMinLenThres;
        public int nHoughLineMinLenThres
        {
            get { return nhoughLineMinLenThres; }
            set
            { SetValue(ref nhoughLineMinLenThres, value); }
        }

        private int nhoughLineMaxGapThres;
        public int nHoughLineMaxGapThres
        {
            get { return nhoughLineMaxGapThres; }
            set
            { SetValue(ref nhoughLineMaxGapThres, value); }
        }
        #endregion

        /// <summary>
        /// 절리형상 DB update 해줍니다.
        /// </summary>
        public void Update()
        {
            DBDoc doc = DBDoc.Get_CurrDoc();

            //절리형상 데이터
            DBDataJSHP jshpD = null;
            if (doc.jshp.Get_Data(1, ref jshpD))
            {
                HmBitmap image = new HmBitmap() { img = (System.Drawing.Bitmap)jshpD.img.Image, };

                List<HmLine2D> lines = GetHoughLines(image, nCannyThres1, nCannyThres2, nHoughLineThres, nHoughLineMinLenThres, nHoughLineMaxGapThres);

                //절리형상 lines
                jshpD.lines.Clear();
                jshpD.lines.AddRange(lines);

                //좌측창 어느 엔티티를 click했는지 따라서 심도와 시료현태 두께 값을 넣는다, 기본적으로 두께값이 3미터이다.

                //좌측창 어느 엔티티를 click했는지 따라서 straKey를 넣는다. 

                doc.jshp.Modify_TR(1, jshpD);
            }
        }

        /// <summary>
        /// 이미지에서 라인 추출 해줍니다.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="nCannyThres1">canny threshold 1</param>
        /// <param name="nCannyThres2">canny threshold 1</param>
        /// <param name="nHoughLineThres">minimum number of votes (intersections in Hough grid cell)</param>
        /// <param name="nHoughLineMinLenThres">minimum number of pixels making up a line</param>
        /// <param name="nHoughLineMaxGapThres">maximum gap in pixels between connectable line segments</param>
        /// <returns></returns>
        public static List<HmLine2D> GetHoughLines(HmBitmap image, int nCannyThres1 = 100, int nCannyThres2 = 200, int nHoughLineThres = 50, int nHoughLineMinLenThres = 50, int nHoughLineMaxGapThres = 20)
        {
            // 원본 이미지
            var cvSourceMat = OpenCvSharp.Extensions.BitmapConverter.ToMat(image.img);
            // grayscale 이미지
            var cvGrayScaleMat = new Mat(cvSourceMat.Size(), MatType.CV_8UC1);
            Cv2.CvtColor(cvSourceMat, cvGrayScaleMat, ColorConversionCodes.BGRA2GRAY);
            // gaussian blurring 이미지
            var cvGaussianBlurMat = new Mat(cvGrayScaleMat.Size(), MatType.CV_8UC1);
            int kerSize = 3; // gaussian kernel size - 입력받아야함
            int sigmaX = 1; // gaussian sigma - 입력안받아도 될것같다, default는 1이다.
            Cv2.GaussianBlur(cvGrayScaleMat, cvGaussianBlurMat, new OpenCvSharp.Size(kerSize, kerSize), sigmaX);
            // canny 이미지
            //int nCannyThres1 = 100; // canny edge detection threshold 값 - 입력받아야함
            //int nCannyThres2 = 200;
            var cvCannyMat = new Mat(cvGaussianBlurMat.Size(), MatType.CV_8UC1);
            Cv2.Canny(cvGaussianBlurMat, cvCannyMat, nCannyThres1, nCannyThres2);
            // hough line 이미지
            var cvHoughLineMat = new Mat(cvCannyMat.Size(), MatType.CV_8UC1);
            double rho = 1;  //# distance resolution in pixels of the Hough grid
            double theta = Math.PI / 180;  //# angular resolution in radians of the Hough grid
            //int nHoughLineThres = 50;  //# minimum number of votes (intersections in Hough grid cell)
            //int nHoughLineMinLenThres = 50;  //# minimum number of pixels making up a line
            //int nHoughLineMaxGapThres = 20;  //# maximum gap in pixels between connectable line segments
            LineSegmentPoint[] lines = Cv2.HoughLinesP(cvCannyMat, rho, theta, nHoughLineThres, nHoughLineMinLenThres, nHoughLineMaxGapThres);
            //for (int i = 0; i < lines.Length; i++)
            //{ Cv2.Line(cvHoughLineMat, lines[i].P1.X, lines[i].P1.Y, lines[i].P2.X, lines[i].P2.Y, Scalar.Red, 3); }
            // output 이미지
            //for (int i = 0; i < lines.Length; i++)
            //{ Cv2.Line(cvSourceMat, lines[i].P1.X, lines[i].P1.Y, lines[i].P2.X, lines[i].P2.Y, Scalar.Red, 3); }

            //image.img = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(cvSourceMat);

            List<HmLine2D> line2Ds = new List<HmLine2D>();
            for (int i = 0; i < lines.Length; i++)
            { line2Ds.Add(new HmLine2D(new HmPoint2D(lines[i].P1.X, lines[i].P1.Y), new HmPoint2D(lines[i].P2.X, lines[i].P2.Y))); }
            return line2Ds;
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
    }
}
