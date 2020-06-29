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
    /// <summary>
    /// Interaction logic for UcJointShapeView.xaml
    /// </summary>
    public partial class UcJointShapeView : AUserControl
    {
        private bool isDraggingImage = false;

        public static bool isDrawingLine = false;

        private Point m_mouseDownPoint;

        Line m_drawLine;

        public UcJointShapeView()
        {
            InitializeComponent();

            Initialize();
        }

        private void Initialize()
        {
            handlerReset = new DelegateHandler(Reset);
        }

        private void AUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!aloadflag)
            {
                ((UcJointShapeViewModel)DataContext).View_Load();
                aloadflag = true; // 한번만 호출하자

                ((UcJointShapeViewModel)DataContext).SetDelegateHandler(handlerReset);
            }
        }

        #region Rotation

        private void SldrRotate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Rotate(sldrRotate.Value);
        }

        private void Rotate(double angle)
        {
            rotateTransfrom.Angle = angle;
        }

        #endregion

        #region Zoom

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

        private void ZoomOut()
        {
            if (scaleTransfrom.ScaleX > 3 || scaleTransfrom.ScaleY > 3)
                return;

            scaleTransfrom.ScaleX *= 1.1;
            scaleTransfrom.ScaleY *= 1.1;
        }

        private void ZoomIn()
        {
            if (scaleTransfrom.ScaleX < 0.3 || scaleTransfrom.ScaleY < 0.3)
                return;

            scaleTransfrom.ScaleX /= 1.1;
            scaleTransfrom.ScaleY /= 1.1;
        }

        #endregion

        #region Image Dragging

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image canvasImage = (Image)sender;

            // Drawing line
            if (isDrawingLine)
            {
                m_mouseDownPoint = (Point)e.GetPosition(canvas);
                m_drawLine = new Line();
                m_drawLine.X1 = Canvas.GetLeft(canvasImage) + m_mouseDownPoint.X;
                m_drawLine.Y1 = Canvas.GetTop(canvasImage) + m_mouseDownPoint.Y;
                m_drawLine.X2 = Canvas.GetLeft(canvasImage) + m_mouseDownPoint.X + 1;
                m_drawLine.Y2 = Canvas.GetTop(canvasImage) + m_mouseDownPoint.Y + 1;

                return;
            }

            // Dragging rectangle
            if (isDraggingRectangle)
            {
                Canvas_MouseDown(sender, e);
                return;
            }

            // Dragging image
            canvas.Focus();
            Keyboard.Focus(canvas);

            isDraggingImage = true;
            m_mouseDownPoint = e.GetPosition(canvas);

            //Image canvasImage = (Image)sender;
            canvasImage.CaptureMouse();

            e.Handled = true;
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            // Drawing line
            if (isDrawingLine)
            {
                Point currPoint = (Point)e.GetPosition(canvas);
                if (m_drawLine != null & e.LeftButton == MouseButtonState.Pressed)
                {
                    m_drawLine.X2 = Canvas.GetLeft(canvasImage) + currPoint.X;
                    m_drawLine.Y2 = Canvas.GetTop(canvasImage) + currPoint.Y;

                    HmGeometry.HmLine2D line = new HmGeometry.HmLine2D(new HmGeometry.HmPoint2D(m_drawLine.X1, m_drawLine.Y1), new HmGeometry.HmPoint2D(m_drawLine.X2, m_drawLine.Y2));
                    ((UcJointShapeViewModel)DataContext).UpdateImage(line);
                }

                return;
            }

            // Resizing rectangle
            if (isDraggingRectangle)
            {
                Canvas_MouseMove(sender, e);
                return;
            }

            if (isDraggingImage)
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
            // Drawing line
            if (isDrawingLine)
            {
                if (m_drawLine != null)
                {
                    HmGeometry.HmLine2D line = new HmGeometry.HmLine2D(new HmGeometry.HmPoint2D(m_drawLine.X1, m_drawLine.Y1), new HmGeometry.HmPoint2D(m_drawLine.X2, m_drawLine.Y2));
                    ((UcJointShapeViewModel)DataContext).UpdateDrawLines(line);
                    m_drawLine = null;
                }
                return;
            }

            // Resizing rectangle
            if (isDraggingRectangle)
            {
                Canvas_MouseUp(sender, e);
                return;
            }

            if (isDraggingImage)
            {
                isDraggingImage = false;

                Image canvasImage = (Image)sender;
                canvasImage.ReleaseMouseCapture();
            }

            e.Handled = true;
        }

        #endregion

        #region Rectangle functions

        private bool isDraggingRectangle = false;

        private Point LastPoint;

        private enum HitType
        {
            None, Body, UL, UR, LR, LL, L, R, T, B
        };

        HitType MouseHitType = HitType.None;

        private HitType SetHitType(Rectangle rect, Point point)
        {
            return HitType.None;

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
            isDraggingRectangle = true;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDraggingRectangle)
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
            isDraggingRectangle = false;
        }

        #endregion

        #region Canny adjustment

        private void SldrCannyMax_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((UcJointShapeViewModel)DataContext).UpdateHoughLines();
        }

        private void SldrCannyMin_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((UcJointShapeViewModel)DataContext).UpdateHoughLines();
        }

        #endregion

        #region Crop functions

        private double zoomFactor = 1.0;

        private void BtnCrop_Click(object sender, RoutedEventArgs e)
        {
            double scaleX = scaleTransfrom.ScaleX;
            double scaleY = scaleTransfrom.ScaleY;
            double rotate = rotateTransfrom.Angle;
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
                        MessageBox.Show("Fixing the error. Sorry. Value does not fall within the expected range. Over!");
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

                    ((UcJointShapeViewModel)DataContext).SetImage(croppedImg);

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

        #region Reset delegate function

        public delegate void DelegateHandler();

        DelegateHandler handlerReset;

        private void Reset()
        {
            scaleTransfrom.ScaleX = 1.0;
            scaleTransfrom.ScaleY = 1.0;
            rotateTransfrom.Angle = 0.0;

            Canvas.SetLeft(canvasImage, 0);
            Canvas.SetTop(canvasImage, 0);
        }

        #endregion

        private void AUserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            { xGrid.Opacity = 1; xGrid.IsEnabled = true; }
            else
            { xGrid.Opacity = 0.5; xGrid.IsEnabled = false; }
        }
    }
}
