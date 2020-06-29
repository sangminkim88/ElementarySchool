using HMFrameWork.Ancestor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GAIA2020.Design
{
    public enum MouseHandlingMode
    {
        /// <summary>
        /// Not in any special mode.
        /// </summary>
        None,

        /// <summary>
        /// The user is left-dragging rectangles with the mouse.
        /// </summary>
        DraggingImage,

        /// <summary>
        /// 영역선택용 사각형
        /// </summary>
        ResizingRectangle,

        /// <summary>
        /// 절리형상 직접 그리기
        /// </summary>
        DrawingLine,
    }

    /// <summary>
    /// Interaction logic for UcJSView.xaml
    /// </summary>
    public partial class UcJSView : AUserControl
    {
        private MouseHandlingMode m_mouseHandlingMode = MouseHandlingMode.None;

        private Point m_mouseDownPoint;


        public UcJSView()
        {
            InitializeComponent();
        }

        private void ZoomOut()
        {
            if (canImgScaleTransfrom.ScaleX > 3 || canImgScaleTransfrom.ScaleY > 3)
                return;

            canImgScaleTransfrom.ScaleX *= 1.1;
            canImgScaleTransfrom.ScaleY *= 1.1;
        }

        private void ZoomIn()
        {
            if (canImgScaleTransfrom.ScaleX < 0.3 || canImgScaleTransfrom.ScaleY < 0.3)
                return;

            canImgScaleTransfrom.ScaleX /= 1.1;
            canImgScaleTransfrom.ScaleY /= 1.1;
        }

        private void Rotate(double angle)
        {
            canImgRotateTransfrom.Angle = angle;
        }

        private void Canvas_Drop(object sender, DragEventArgs e)
        {

        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Resizing rectangle
            if (DragInProgress)
            {
                Canvas_MouseDown(sender, e);
                return;
            }

            canvas.Focus();
            Keyboard.Focus(canvas);

            if (m_mouseHandlingMode == MouseHandlingMode.None)
            {
                m_mouseHandlingMode = MouseHandlingMode.DraggingImage;
                m_mouseDownPoint = e.GetPosition(canvas);

                Image canvasImage = (Image)sender;
                canvasImage.CaptureMouse();
            }

            e.Handled = true;
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            // Resizing rectangle
            if (DragInProgress)
            {
                Canvas_MouseMove(sender, e);
                return;
            }

            if (m_mouseHandlingMode == MouseHandlingMode.DraggingImage)
            {
                Point curCanvasPoint = e.GetPosition(canvas);
                Vector dragVector = curCanvasPoint - m_mouseDownPoint;

                //
                // When in 'dragging rectangles' mode update the position of the rectangle as the user drags it.
                //

                m_mouseDownPoint = curCanvasPoint;

                Image canvasImage = (Image)sender;
                Canvas.SetLeft(canvasImage, Canvas.GetLeft(canvasImage) + dragVector.X);
                Canvas.SetTop(canvasImage, Canvas.GetTop(canvasImage) + dragVector.Y);
            }

            e.Handled = true;
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Resizing rectangle
            if (DragInProgress)
            {
                Canvas_MouseUp(sender, e);
                return;
            }

            if (m_mouseHandlingMode == MouseHandlingMode.DraggingImage)
            {
                m_mouseHandlingMode = MouseHandlingMode.None;

                Image canvasImage = (Image)sender;
                canvasImage.ReleaseMouseCapture();
            }

            e.Handled = true;
        }

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            if (e.Delta > 0)
            {
                ZoomIn();
            }
            else if (e.Delta < 0)
            {
                ZoomOut();
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Rotate(slider.Value);
        }

        private void ButtonPlus_Click(object sender, RoutedEventArgs e)
        {
            slider.Value += 1;
        }

        private void ButtonMinus_Click(object sender, RoutedEventArgs e)
        {
            slider.Value -= 1;
        }


        #region Rectangle functions

        private bool DragInProgress = false;

        private Point LastPoint;

        private enum HitType
        {
            None, Body, UL, UR, LR, LL, L, R, T, B
        };

        HitType MouseHitType = HitType.None;

        private HitType SetHitType(Rectangle rect, Point point)
        {
            double left = Canvas.GetLeft(canvasRect);
            double top = Canvas.GetTop(canvasRect);
            double right = left + canvasRect.Width;
            double bottom = top + canvasRect.Height;
            if (point.X < left) return HitType.None;
            if (point.X > right) return HitType.None;
            if (point.Y < top) return HitType.None;
            if (point.Y > bottom) return HitType.None;

            const double GAP = 10;
            if (point.X - left < GAP)
            {
                // Left edge.
                if (point.Y - top < GAP) return HitType.UL;
                if (bottom - point.Y < GAP) return HitType.LL;
                return HitType.L;
            }
            if (right - point.X < GAP)
            {
                // Right edge.
                if (point.Y - top < GAP) return HitType.UR;
                if (bottom - point.Y < GAP) return HitType.LR;
                return HitType.R;
            }
            if (point.Y - top < GAP) return HitType.T;
            if (bottom - point.Y < GAP) return HitType.B;
            return HitType.Body;
        }

        private void SetMouseCursor()
        {
            // See what cursor we should display.
            Cursor desired_cursor = Cursors.Arrow;
            switch (MouseHitType)
            {
                case HitType.None:
                    desired_cursor = Cursors.Arrow;
                    break;
                case HitType.Body:
                    desired_cursor = Cursors.ScrollAll;
                    break;
                case HitType.UL:
                case HitType.LR:
                    desired_cursor = Cursors.SizeNWSE;
                    break;
                case HitType.LL:
                case HitType.UR:
                    desired_cursor = Cursors.SizeNESW;
                    break;
                case HitType.T:
                case HitType.B:
                    desired_cursor = Cursors.SizeNS;
                    break;
                case HitType.L:
                case HitType.R:
                    desired_cursor = Cursors.SizeWE;
                    break;
            }

            // Display the desired cursor.
            if (Cursor != desired_cursor) Cursor = desired_cursor;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MouseHitType = SetHitType(canvasRect, Mouse.GetPosition(canvas));
            SetMouseCursor();
            if (MouseHitType == HitType.None) return;

            LastPoint = Mouse.GetPosition(canvas);
            DragInProgress = true;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!DragInProgress)
            {
                MouseHitType = SetHitType(canvasRect, Mouse.GetPosition(canvas));
                SetMouseCursor();
            }
            else
            {
                // See how much the mouse has moved.
                Point point = Mouse.GetPosition(canvas);
                double offset_x = point.X - LastPoint.X;
                double offset_y = point.Y - LastPoint.Y;

                // Get the rectangle's current position.
                double new_x = Canvas.GetLeft(canvasRect);
                double new_y = Canvas.GetTop(canvasRect);
                double new_width = canvasRect.Width;
                double new_height = canvasRect.Height;

                // Update the rectangle.
                switch (MouseHitType)
                {
                    case HitType.Body:
                        new_x += offset_x;
                        new_y += offset_y;
                        break;
                    case HitType.UL:
                        new_x += offset_x;
                        new_y += offset_y;
                        new_width -= offset_x;
                        new_height -= offset_y;
                        break;
                    case HitType.UR:
                        new_y += offset_y;
                        new_width += offset_x;
                        new_height -= offset_y;
                        break;
                    case HitType.LR:
                        new_width += offset_x;
                        new_height += offset_y;
                        break;
                    case HitType.LL:
                        new_x += offset_x;
                        new_width -= offset_x;
                        new_height += offset_y;
                        break;
                    case HitType.L:
                        new_x += offset_x;
                        new_width -= offset_x;
                        break;
                    case HitType.R:
                        new_width += offset_x;
                        break;
                    case HitType.B:
                        new_height += offset_y;
                        break;
                    case HitType.T:
                        new_y += offset_y;
                        new_height -= offset_y;
                        break;
                }

                // Don't use negative width or height.
                if ((new_width > 0) && (new_height > 0))
                {
                    // Update the rectangle.
                    Canvas.SetLeft(canvasRect, new_x);
                    Canvas.SetTop(canvasRect, new_y);
                    canvasRect.Width = new_width;
                    canvasRect.Height = new_height;

                    // Save the mouse's new location.
                    LastPoint = point;
                }
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DragInProgress = false;
        }

        #endregion

        #region Crop functions

        private double zoomFactor = 1.0;

        private void BtnCrop_Click(object sender, RoutedEventArgs e)
        {
            double scaleX = canImgScaleTransfrom.ScaleX;
            double scaleY = canImgScaleTransfrom.ScaleY;
            double rAngle = canImgRotateTransfrom.Angle;
            double transX = Canvas.GetLeft(canvasImage);
            double transY = Canvas.GetTop(canvasImage);

            try
            {
                string uri = ((UcJSViewModel)DataContext).GetImageUri();
                // 원본 이미지
                var cvSourceMat = new OpenCvSharp.Mat(uri);

                OpenCvSharp.MatType matType = cvSourceMat.Type();

                // Resize 이미지
                //var cvGrayScaleMat = new OpenCvSharp.Mat(, matType);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message.ToString()); }
            finally { }

            return;

            //0=좌상, 1=좌하, 2=우하, 3=우상
            double[] iox = new double[4]; // image origin x 이동/확대/축수/회전 변환 전
            double[] ioy = new double[4]; // image origin y 이동/확대/축수/회전 변환 전

            iox[0] = 0;                                 ioy[0] = 0;
            iox[1] = iox[0];                            ioy[1] = ioy[0] + canvasImage.ActualHeight;
            iox[2] = iox[0] + canvasImage.ActualWidth;  ioy[2] = ioy[0] + canvasImage.ActualHeight;
            iox[3] = iox[0] + canvasImage.ActualWidth;  ioy[3] = ioy[0];

            double ioxMin = iox[0];
            double ioxMax = iox[3];
            double ioyMin = ioy[0];
            double ioyMax = ioy[1];

            //0=좌상, 1=좌하, 2=우하, 3=우상
            double[] itx = new double[4]; // image target x 이동/확대/축수/회전 변환 후
            double[] ity = new double[4]; // image target y 이동/확대/축수/회전 변환 후

            itx[0] = Canvas.GetLeft(canvasImage);                   ity[0] = Canvas.GetTop(canvasImage);
            itx[1] = itx[0];                                        ity[1] = (ity[0] + canvasImage.ActualHeight) * scaleY;
            itx[2] = (itx[0] + canvasImage.ActualWidth) * scaleX;   ity[2] = (ity[0] + canvasImage.ActualHeight) * scaleY;
            itx[3] = (itx[0] + canvasImage.ActualWidth) * scaleX;   ity[3] = ity[0];

            //0=좌상, 1=좌하, 2=우하, 3=우상
            double[] rtx = new double[4]; // rect target x 이동/확대/축수/회전 변환 후
            double[] rty = new double[4]; // rect target y 이동/확대/축수/회전 변환 후

            rtx[0] = Canvas.GetLeft(canvasRect);        rty[0] = Canvas.GetTop(canvasRect);
            rtx[1] = rtx[0];                            rty[1] = rty[0] + canvasRect.ActualHeight;
            rtx[2] = rtx[0] + canvasRect.ActualWidth;   rty[2] = rty[0] + canvasRect.ActualHeight;
            rtx[3] = rtx[0] + canvasRect.ActualWidth;   rty[3] = rty[0];

            //0=좌상, 1=좌하, 2=우하, 3=우상
            double[] rox = new double[4]; // rect origin x 이동/확대/축수/회전 변환 전
            double[] roy = new double[4]; // rect origin y 이동/확대/축수/회전 변환 전

            rox[0] = rtx[0] - itx[0];                               roy[0] = rty[0] - ity[0];
            rox[1] = rox[0];                                        roy[1] = (roy[0] + canvasRect.ActualHeight) / scaleY;
            rox[2] = (rox[0] + canvasRect.ActualWidth) / scaleX;    roy[2] = (roy[0] + canvasRect.ActualHeight) / scaleY;
            rox[3] = (rox[0] + canvasRect.ActualWidth) / scaleX;    roy[3] = roy[0];

            // rect inside image area 확인
            if ((ioxMin <= rox[0] && rox[0] <= ioxMax) && (ioyMin <= roy[0] && roy[0] <= ioyMax) &&
                (ioxMin <= rox[1] && rox[1] <= ioxMax) && (ioyMin <= roy[1] && roy[1] <= ioyMax) &&
                (ioxMin <= rox[2] && rox[2] <= ioxMax) && (ioyMin <= roy[2] && roy[2] <= ioyMax) &&
                (ioxMin <= rox[3] && rox[3] <= ioxMax) && (ioyMin <= roy[3] && roy[3] <= ioyMax))
            {
                var rect1 = new Rect()
                {
                    X = rox[0],
                    Y = roy[0],
                    Width = rox[3] - rox[1],
                    Height = roy[1] - roy[0],
                };

                // calc scale in PIXEls for CroppedBitmap...
                var img = canvasImage.Source as BitmapSource;
                var scaleWidth = (img.PixelWidth) / (canvasImage.ActualWidth);
                var scaleHeight = (img.PixelHeight) / (canvasImage.ActualHeight);

                var rcFrom = new Int32Rect()
                {
                    X = (int)(rect1.X * scaleWidth),
                    Y = (int)(rect1.Y * scaleHeight),
                    Width = (int)(rect1.Width * scaleWidth),
                    Height = (int)(rect1.Height * scaleHeight)
                };

                var croppedImg = new CroppedBitmap(img, rcFrom);

                ((UcJSViewModel)DataContext).SetImage(croppedImg);

                Canvas.SetLeft(canvasImage, rect1.X);
                Canvas.SetTop(canvasImage, rect1.Y);
            }
            else
            {
                MessageBox.Show("Fixing the error. Sorry. Value does not fall within the expected range. Over!");
            }

            return;

            double imgOffsetX = Canvas.GetLeft(canvasImage);
            double imgOffsetY = Canvas.GetTop(canvasImage);

            try
            {
                if (canvasImage.Source != null)
                {
                    var rect1 = new Rect()
                    {
                        X = Canvas.GetLeft(canvasRect),
                        Y = Canvas.GetTop(canvasRect),
                        Width = canvasRect.Width,
                        Height = canvasRect.Height
                    };

                    rect1.X -= imgOffsetX;
                    rect1.Y -= imgOffsetY;
                    rect1.Width /= scaleX;
                    rect1.Height /= scaleY;

                    if (rect1.X + rect1.Width > canvasImage.ActualWidth || rect1.Y + rect1.Height > canvasImage.ActualHeight)
                    {
                        MessageBox.Show("Over!");
                        return;
                    }

                    // calc scale in PIXEls for CroppedBitmap...
                    var img = canvasImage.Source as BitmapSource;
                    var scaleWidth = (img.PixelWidth) / (canvasImage.ActualWidth);
                    var scaleHeight = (img.PixelHeight) / (canvasImage.ActualHeight);

                    var rcFrom = new Int32Rect()
                    {
                        X = (int)(rect1.X * scaleWidth),
                        Y = (int)(rect1.Y * scaleHeight),
                        Width = (int)(rect1.Width * scaleWidth),
                        Height = (int)(rect1.Height * scaleHeight)
                    };

                    var croppedImg = new CroppedBitmap(img, rcFrom);

                    ((UcJSViewModel)DataContext).SetImage(croppedImg);

                    rect1.X += imgOffsetX;
                    rect1.Y += imgOffsetY;
                    Canvas.SetLeft(canvasImage, rect1.X);
                    Canvas.SetTop(canvasImage, rect1.Y);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message.ToString()); }
            finally { }

            return;

            try
            {
                double dLeft = Canvas.GetLeft(canvasRect);
                double dTop = Canvas.GetTop(canvasRect);

                string strImageUri = ((UcJSViewModel)DataContext).GetImageUri();

                using (System.Drawing.Bitmap source = new System.Drawing.Bitmap(strImageUri))
                {
                    using (System.Drawing.Bitmap target = new System.Drawing.Bitmap((int)canvasRect.Width, (int)canvasRect.Height))
                    {
                        System.Drawing.RectangleF recDest = new System.Drawing.RectangleF(0.0f, 0.0f, (float)target.Width, (float)target.Height);

                        float hd = 1.0f / (target.HorizontalResolution / source.HorizontalResolution);
                        float vd = 1.0f / (target.VerticalResolution / source.VerticalResolution);
                        float hScale = 1.0f / (float)zoomFactor;
                        float vScale = 1.0f / (float)zoomFactor;

                        System.Drawing.RectangleF recSrc = new System.Drawing.RectangleF((hd * (float)dLeft) * hScale, (vd * (float)dTop) * vScale, (hd * (float)canvasRect.Width) * hScale, (vd * (float)canvasRect.Height) * vScale);

                        using (System.Drawing.Graphics gfx = System.Drawing.Graphics.FromImage(target))
                        {
                            gfx.DrawImage(source, recDest, recSrc, System.Drawing.GraphicsUnit.Pixel);
                        }

                        ((UcJSViewModel)DataContext).SetImage(target);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally { }
        }

        #endregion
    }
}
