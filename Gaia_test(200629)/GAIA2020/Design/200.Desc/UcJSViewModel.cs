using GAIA2020.Utilities;
using HMFrameWork.Ancestor;
using HMFrameWork.Command;
using HmGeometry;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace GAIA2020.Design
{
    public partial class UcJSViewModel : AViewModel, INotifyPropertyChanged
    {
        private string m_strImageUri;

        public UcJSViewModel()
        {
            m_strImageUri = "";
        }

        #region 속성

        private ImageSource img;
        public ImageSource Img
        {
            get { return img; }
            set { SetValue(ref img, value); }
        }

        private ImageBrush imgBrush;
        public ImageBrush ImgBrush
        {
            get { return imgBrush; }
            set { SetValue(ref imgBrush, value); }
        }

        private int rotationValue;
        public int RotationValue
        {
            get { return rotationValue; }
            set { SetValue(ref rotationValue, value); }
        }

        #endregion

        #region Command

        ICommand commandJS;

        public ICommand CommandJS
        {
            get
            {
                return commandJS ?? (commandJS = new RelayCommand(JSExecute, CanJSExecute));
            }
        }

        private void JSExecute(object parameter)
        {
            try
            {
                string valstr, strkey = parameter.ToString();
                // 명령창 종류
                valstr = CmdParmParser.GetValuefromKey(strkey, "cmd");

                if (valstr == "fileopen")
                {
                    FileOpen();
                }
                else if (valstr == "slidervaluechanged")
                {

                }
            }
            catch (Exception ex)
            { }
            finally
            { }
        }

        private bool CanJSExecute(object parameter)
        {
            return true;
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

            this.Img = ImageConverter.BitMapToBitmapImage(image.img, System.Drawing.Imaging.ImageFormat.Png);

            //this.ImgBrush = new ImageBrush(this.img);

            m_strImageUri = strFilePath;
        }

        public string GetImageUri()
        {
            return m_strImageUri;
        }

        public void SetImage(System.Drawing.Bitmap image)
        {
            this.Img = ImageConverter.BitMapToBitmapImage(image, System.Drawing.Imaging.ImageFormat.Png);
        }

        public void SetImage(ImageSource imgSrc)
        {
            this.Img = imgSrc;
        }

        #endregion

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
