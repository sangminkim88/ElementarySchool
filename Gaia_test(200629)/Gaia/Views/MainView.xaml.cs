namespace Gaia.Views
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class FirstViewModel : INotifyPropertyChanged
    {
        #region Fields

        private List<Tuple<string, Brush>> polygonColor;

        private string sampleForm;

        private Tuple<string, string> soilColumn;

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        public List<Tuple<string, Brush>> PolygonColor
        {
            get { return polygonColor; }
            set { polygonColor = value; OnPropertyChanged("PolygonColor"); }
        }

        public string SampleForm
        {
            get { return sampleForm; }
            set { sampleForm = value; OnPropertyChanged("SampleForm"); }
        }

        public Tuple<string, string> SoilColumn
        {
            get { return soilColumn; }
            set { soilColumn = value; OnPropertyChanged("SoilColumn"); }
        }

        #endregion

        #region Methods

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public partial class MainView : Grid, INotifyPropertyChanged
    {
        #region Fields

        private int bridgeCount;

        private int cutCount;

        private bool enableBridge;

        private bool enableCut;

        private bool enableGetSand;

        private bool enableHandAuger;

        private bool enablePile;

        private bool enableTestPit;

        private bool enableTunnel;

        private FirstView firstView;

        private FirstViewModel firstViewModel = new FirstViewModel();

        private FourthView fourthView;

        private Action<string> GetB1;

        private Action<Tuple<string, string>> GetB10;

        private Action<Tuple<string, string>> GetB11;

        private Action<bool> GetB12;

        private Action<List<Tuple<string, string>>> GetB13;

        private Action<List<Tuple<string, string>>> GetB2;

        private Action<List<Tuple<string, string>>> GetB3;

        private Action<List<Tuple<string, string>>> GetB4;

        private Action<string> GetB5;

        private Action<string> GetB6;

        private Action<List<Tuple<string, string>>> GetB7;

        private Action<List<Tuple<string, string>>> GetB8;

        private Action<Tuple<string, string>> GetB9;

        private int getSandCount;

        private int handAugetCount;

        private RadioButton oldSelectedMenu;

        private int pileCount;

        private SecondView secondView;

        private int testPitCount;

        private string textBridge;

        private string textCut;

        private string textGetSand;

        private string textHandAuger;

        private string textPile;

        private string textTestPit;

        private string textTunnel;

        private ThirdView thirdView;

        private int tunnelCount;

        #endregion

        #region Constructors

        public MainView(string a, string b, string c, string d, string e)
        {
            InitializeComponent();
            this.DataContext = this;
            this.TextPile = "쌓기부(" + a + ")";
            this.TextBridge = "교량부(" + b + ")";
            this.TextCut = "깍기부(" + c + ")";
            this.TextTunnel = "터널부(" + d + ")";
            this.TextGetSand = "토취장(" + e + ")";
            this.TextTestPit = "시험굴(" + a + ")";
            this.TextHandAuger = "핸드오거(" + b + ")";

            this.EnablePile = true;
            this.EnableBridge = true;
            this.EnableCut = true;
            this.EnableTunnel = true;
            this.EnableGetSand = true;
            this.EnableTestPit = true;
            this.EnableHandAuger = true;

            //GetA1 = data =>
            //{
            //    if (data != null)
            //    {
            //        this.a10.Text = data.Item1;
            //        this.a11.Text = data.Item2;
            //    }
            //};
            //GetA2 = data =>
            //{
            //    if (data.Count.Equals(2))
            //    {
            //        this.a20.Text = data[0].Item1 + " " + data[0].Item2;
            //        this.a21.Text = data[1].Item1 + " " + data[1].Item2;
            //    }
            //};
            //GetA3 = data =>
            //{
            //    this.a3.Text = data;
            //};
            GetB1 = data =>
            {
                this.b1.Text = data;
            };
            GetB2 = data =>
            {
                if (data.Count.Equals(2))
                {
                    this.b10.Text = data[0].Item1 + ";" + data[1].Item1;
                }
            };
            GetB3 = data =>
            {
                if (data.Count.Equals(2))
                {
                    this.b11.Text = data[0].Item1 + ";" + data[1].Item1;
                }
            };
            GetB4 = data =>
            {
                if (data.Count.Equals(2))
                {
                    this.b12.Text = data[0].Item1 + ";" + data[1].Item1;
                }
            };
            GetB5 = data =>
            {
                this.b2.Text = data;
            };
            GetB6 = data =>
            {
                this.b20.Text = data;
            };
            GetB7 = data =>
            {
                if (data.Count.Equals(2))
                {
                    this.b21.Text = data[0].Item1 + ";" + data[1].Item1;
                }
            };
            GetB8 = data =>
            {
                if (data.Count.Equals(2))
                {
                    this.b22.Text = data[0].Item1 + ";" + data[1].Item1;
                }
            };
            GetB9 = data =>
            {
                if (data != null)
                {
                    this.b230.Text = data.Item1 + ";" + data.Item2;
                }
            };
            GetB10 = data =>
            {
                if (data != null)
                {
                    this.b231.Text = data.Item1 + ";" + data.Item2;
                }
            };
            GetB11 = data =>
            {
                if (data != null)
                {
                    this.b232.Text = data.Item1 + ";" + data.Item2;
                }
            };
            GetB12 = data =>
            {
                this.b233.IsChecked = data;
            };
            GetB13 = data =>
            {
                if (data.Count.Equals(2))
                {
                    this.b24.Text = data[0].Item1 + ":" + data[0].Item2 + ";" + data[1].Item2;
                }
            };
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        public bool EnableBridge
        {
            get { return enableBridge; }
            set
            {
                enableBridge = value;
                OnPropertyChanged("EnableBridge");
            }
        }

        public bool EnableCut
        {
            get { return enableCut; }
            set
            {
                enableCut = value;
                OnPropertyChanged("EnableCut");
            }
        }

        public bool EnableGetSand
        {
            get { return enableGetSand; }
            set
            {
                enableGetSand = value;
                OnPropertyChanged("EnableGetSand");
            }
        }

        public bool EnableHandAuger
        {
            get { return enableHandAuger; }
            set
            {
                enableHandAuger = value;
                OnPropertyChanged("EnableHandAuger");
            }
        }

        public bool EnablePile
        {
            get { return enablePile; }
            set
            {
                enablePile = value;
                OnPropertyChanged("EnablePile");
            }
        }

        public bool EnableTestPit
        {
            get { return enableTestPit; }
            set
            {
                enableTestPit = value;
                OnPropertyChanged("EnableTestPit");
            }
        }

        public bool EnableTunnel
        {
            get { return enableTunnel; }
            set
            {
                enableTunnel = value;
                OnPropertyChanged("EnableTunnel");
            }
        }

        public FirstViewModel FirstViewModelProp
        {
            get { return firstViewModel; }
            set { firstViewModel = value; OnPropertyChanged("FirstViewModelProp"); }
        }

        public string TextBridge
        {
            get { return textBridge; }
            set
            {
                textBridge = value;
                OnPropertyChanged("TextBridge");
            }
        }

        public string TextCut
        {
            get { return textCut; }
            set
            {
                textCut = value;
                OnPropertyChanged("TextCut");
            }
        }

        public string TextGetSand
        {
            get { return textGetSand; }
            set
            {
                textGetSand = value;
                OnPropertyChanged("TextGetSand");
            }
        }

        public string TextHandAuger
        {
            get { return textHandAuger; }
            set
            {
                textHandAuger = value;
                OnPropertyChanged("TextHandAuger");
            }
        }

        public string TextPile
        {
            get { return textPile; }
            set
            {
                textPile = value;
                OnPropertyChanged("TextPile");
            }
        }

        public string TextTestPit
        {
            get { return textTestPit; }
            set
            {
                textTestPit = value;
                OnPropertyChanged("TextTestPit");
            }
        }

        public string TextTunnel
        {
            get { return textTunnel; }
            set
            {
                textTunnel = value;
                OnPropertyChanged("TextTunnel");
            }
        }

        #endregion

        #region Methods

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void a1Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.firstView != null)
            {
                this.firstView.soilColumn.SetSelectedByCode(this.a11.Text);
            }
        }

        private void a2Button_Click(object sender, RoutedEventArgs e)
        {
            List<string> tmp = new List<string>();
            tmp.Add(this.a20.Text);
            tmp.Add(this.a21.Text);
            this.firstView.polygonColorPicker.SetSelectedColorByNames(tmp);
        }

        private void a3Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.firstView != null)
            {
                this.firstView.sampleForm.SetSelectedSampleForm(this.a3.Text);
            }
        }

        private void b10Button_Click(object sender, RoutedEventArgs e)
        {
            this.secondView.SetWetsByTitle(this.b10.Text.Split(';').ToList());
        }

        private void b11Button_Click(object sender, RoutedEventArgs e)
        {
            this.secondView.SetSandyDensitiesByTitle(this.b11.Text.Split(';').ToList());
        }

        private void b12Button_Click(object sender, RoutedEventArgs e)
        {
            this.secondView.SetCohesiveDensitiesByTitle(this.b12.Text.Split(';').ToList());
        }

        private void b1Button_Click(object sender, RoutedEventArgs e)
        {
            this.secondView.SetSand(this.b1.Text);
        }

        private void b20Button_Click(object sender, RoutedEventArgs e)
        {
            this.secondView.SetRockKinds(this.b20.Text);
        }

        private void b21Button_Click(object sender, RoutedEventArgs e)
        {
            this.secondView.SetWeatheredByTitle(this.b21.Text.Split(';').ToList());
        }

        private void b22Button_Click(object sender, RoutedEventArgs e)
        {
            this.secondView.SetStrengthByTitle(this.b22.Text.Split(';').ToList());
        }

        private void b230Button_Click(object sender, RoutedEventArgs e)
        {
            this.secondView.Set1Join(this.b230.Text.Split(';').ToList());
        }

        private void b231Button_Click(object sender, RoutedEventArgs e)
        {
            this.secondView.Set2Join(this.b231.Text.Split(';').ToList());
        }

        private void b232Button_Click(object sender, RoutedEventArgs e)
        {
            this.secondView.Set3Join(this.b232.Text.Split(';').ToList());
        }

        private void B233_Clicked(object sender, RoutedEventArgs e)
        {
            this.secondView.SetPartJoin((bool)this.b233.IsChecked);
        }

        private void b24Button_Click(object sender, RoutedEventArgs e)
        {
            string first = this.b24.Text.Split(':')[0];
            string second0 = this.b24.Text.Split(':')[1].Split(';')[0];
            string second1 = this.b24.Text.Split(':')[1].Split(';')[1];
            List<Tuple<string, string>> targets = new List<Tuple<string, string>>();
            targets.Add(new Tuple<string, string>(first, second0));
            targets.Add(new Tuple<string, string>(first, second1));
            this.secondView.SetRough(targets);
        }

        private void b2Button_Click(object sender, RoutedEventArgs e)
        {
            this.secondView.SetRock(this.b2.Text);
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (oldSelectedMenu == null || !radioButton.Equals(oldSelectedMenu))
            {
                oldSelectedMenu = radioButton;
                this.inputPanel.Children.Clear();
                string selectedMenu = radioButton.Content.ToString();
                if (radioButton.Content.ToString().Contains("("))
                {
                    selectedMenu = selectedMenu.Substring(0, radioButton.Content.ToString().IndexOf('('));
                }

                switch (selectedMenu)
                {
                    case "쌓기부":
                        this.groupBox0.Visibility = Visibility.Visible;
                        this.groupBox1.Visibility = Visibility.Collapsed;
                        //if (this.firstView == null) this.firstView = new FirstView(this.GetA1, this.GetA2, this.GetA3);
                        if (this.firstView == null) this.firstView = new FirstView(this.FirstViewModelProp);
                        Grid.SetRow(this.firstView, 1);
                        Grid.SetColumn(this.firstView, 1);
                        this.inputPanel.Children.Add(this.firstView);
                        break;
                    case "교량부":
                        this.groupBox0.Visibility = Visibility.Collapsed;
                        this.groupBox1.Visibility = Visibility.Visible;
                        if (this.secondView == null) this.secondView = new SecondView(this.GetB1, this.GetB2, this.GetB3, this.GetB4,
                            this.GetB5, this.GetB6, this.GetB7, this.GetB8, this.GetB9, this.GetB10, this.GetB11, this.GetB12, this.GetB13);
                        Grid.SetRow(this.secondView, 1);
                        Grid.SetColumn(this.secondView, 1);
                        this.inputPanel.Children.Add(this.secondView);
                        break;
                    case "깍기부":
                        if (this.thirdView == null) this.thirdView = new ThirdView();
                        Grid.SetRow(this.thirdView, 1);
                        Grid.SetColumn(this.thirdView, 1);
                        this.inputPanel.Children.Add(this.thirdView);
                        break;
                    case "터널부":

                        break;
                    case "토취장":

                        break;
                    case "시험굴":
                        if (this.fourthView == null) this.fourthView = new FourthView();
                        Grid.SetRow(this.fourthView, 1);
                        Grid.SetColumn(this.fourthView, 1);
                        this.inputPanel.Children.Add(this.fourthView);
                        break;
                }
            }
        }

        private void Viewbox_Loaded(object sender, RoutedEventArgs e)
        {
            Viewbox viewbox = sender as Viewbox;
            if (this.scrollView.ActualWidth > viewbox.MinWidth)
            {
                viewbox.MinWidth = this.scrollView.ActualWidth;
            }
            if (this.scrollView.ActualHeight > viewbox.MinHeight)
            {
                viewbox.MinHeight = this.scrollView.ActualHeight;
            }
        }

        #endregion
    }
}
