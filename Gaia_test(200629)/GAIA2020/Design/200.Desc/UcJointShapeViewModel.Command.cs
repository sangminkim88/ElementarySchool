using GAIA2020.Utilities;
using GaiaDB;
using HmDataDocument;
using HMFrameWork.Command;
using HmGeometry;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GAIA2020.Design
{
    using OpenCvSharp;
    using System.Windows;

    public partial class UcJointShapeViewModel
    {
        internal ICommand commandJointShape;
        public ICommand CommandJointShape
        {
            get
            {
                return commandJointShape ?? (commandJointShape = new RelayCommand(JointShapeExecute, CanJointShapeExecute));
            }
        }

        private bool CanJointShapeExecute(object parameter)
        {
            return true;
        }

        private void JointShapeExecute(object parameter)
        {
            try
            {
                string valstr, strkey = parameter.ToString();
                // 명령창 종류
                valstr = CmdParmParser.GetValuefromKey(strkey, "cmd");

                if (valstr == "open")
                {
                    FileOpen();
                }
                else if (valstr == "crop")
                {

                }
                else if (valstr == "draw")
                {
                    UcJointShapeView.isDrawingLine = !UcJointShapeView.isDrawingLine;

                    if (UcJointShapeView.isDrawingLine == true)
                    {
                        handlerReset();
                    }
                }
                else if (valstr == "delete")
                {
                    ClearLines();
                }
                else if (valstr == "reset")
                {
                    handlerReset();
                }
                else if (valstr == "finish")
                {
                    if (null == m_jshpImage)
                    { MessageBox.Show("절리형상 사진 없습니다."); return; }

                    DBDoc doc = DBDoc.Get_CurrDoc();
                    
                    //절리형상 데이터
                    DBDataJSHP jshpD = new DBDataJSHP();

                    jshpD.straKey = m_nStraKey;

                    //절리사진 가져오고
                    jshpD.img.Image = m_jshpImage.img;

                    //절리형상 lines
                    jshpD.lines.AddRange(m_jshpLines);

                    // desc는 depth 정보를 가지고 있으면 이것은 sub desc라고 봅니다.
                    // main desc는 depth 정보를 직접 가지고 있지 않고 stra에서 가져와서 씁니다.
                    // jshp는 우선 desc depth를 사용, 없으면 stra depth를 사용합니다.

                    // desc에서 depth를 추출
                    bool isDescHasDepth = false;
                    double dTop = 0.0, dBot = 0.0;
                    DBDataDESC descD = null;
                    if (doc.desc.Get_Data(m_nKey, ref descD))
                    {
                        if (descD.Get_Depth(ref dTop, ref dBot))
                        {
                            //심도
                            jshpD.depth = dTop;

                            //두께
                            jshpD.thick = dBot - dTop;

                            isDescHasDepth = true;
                        }
                    }

                    // stra에서 depth를 추출
                    if (!isDescHasDepth)
                    {
                        if (doc.stra.Get_Depth(m_nStraKey, ref dTop, ref dBot))
                        {
                            //심도
                            jshpD.depth = dTop;

                            //두께
                            jshpD.thick = dBot - dTop;
                        }
                    }                    

                    //좌측창 어느 엔티티를 click했는지 따라서 심도와 시료현태 두께 값을 넣는다, 기본적으로 두께값이 3미터이다.

                    //좌측창 어느 엔티티를 click했는지 따라서 straKey를 넣는다. 

                    doc.Get_TranCtrl().TransactionOpen("절리형상 추가");
                    doc.jshp.Add_Data(m_nKey, jshpD);
                    doc.Get_TranCtrl().TransactionClose(true, true);

                }
            }
            catch (Exception ex)
            {
                TransactionCtrl tCtrl = DBDoc.Get_CurrDoc().Get_TranCtrl();
                if (tCtrl.Is_TransactionOpen())
                { tCtrl.TransactionClose(false, false); }
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            { }
        }

        public void FileOpen()
        {
            OpenFileDialog openDig = new OpenFileDialog();
            openDig.Multiselect = false;
            openDig.InitialDirectory = "";
            openDig.Title = "이미지 선택";
            openDig.Filter = "이미지파일(*.PNG,*.JPG,*.JPEG,*.BMP,*.GIF,*.TIF,*.TIFF)|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tif;*.tiff;|모든파일(*.*)|*.*";

            if (openDig.ShowDialog() == true)
            {
                if (!string.IsNullOrEmpty(openDig.FileName))
                {
                    FileOpen(openDig.FileName);

                }
            }
        }

        public void FileOpen(string strFilePath)
        {
            //절리사진 가져오고
            HmBitmap image = new HmBitmap() { img = new System.Drawing.Bitmap(strFilePath), };

            m_jshpImage = image.Clone();

            UpdateImage();
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
        public static List<HmLine2D> GetHoughLinesP(HmBitmap image, int nCannyThres1 = 100, int nCannyThres2 = 200, int nHoughLineThres = 50, int nHoughLineMinLenThres = 50, int nHoughLineMaxGapThres = 20)
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
    }
}
