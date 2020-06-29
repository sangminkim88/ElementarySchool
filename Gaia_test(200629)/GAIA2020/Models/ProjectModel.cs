namespace GAIA2020.Models
{
    using HMFrameWork.Ancestor;
    using System.Windows.Controls;

    public class ProjectModel : ANotifyProperty
    {
        #region Fields

        private int? borrowPit = 0;

        private int? bridge = 0;

        private int? cut = 0;

        private string filePath = string.Empty;

        private int? fill = 0;

        private int? handAuger = 0;

        private bool? isNew;

        private string name = string.Empty;

        private string order = string.Empty;

        private object thumbnail;

        private int? trialPit = 0;

        private int? tunnel = 0;

        #endregion

        #region Constructors

        public ProjectModel()
        {
        }

        public ProjectModel(bool isNew, string name, string order, int? fill, int? bridge, int? cut, int? tunnel, int? borrowPit, int? trialPit, int? handAuger)
        {
            this.isNew = isNew;
            this.name = name;
            this.order = order;
            this.fill = fill;
            this.bridge = bridge;
            this.cut = cut;
            this.tunnel = tunnel;
            this.borrowPit = borrowPit;
            this.trialPit = trialPit;
            this.handAuger = handAuger;
        }

        #endregion

        #region Properties

        public int? BorrowPit
        {
            get { return borrowPit; }
            set { SetValue(ref borrowPit, value); }
        }

        public int? Bridge
        {
            get { return bridge; }
            set { SetValue(ref bridge, value); }
        }

        public int? Cut
        {
            get { return cut; }
            set { SetValue(ref cut, value); }
        }

        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        public int? Fill
        {
            get { return fill; }
            set { SetValue(ref fill, value); }
        }

        public int? HandAuger
        {
            get { return handAuger; }
            set { SetValue(ref handAuger, value); }
        }

        public string Name
        {
            get { return name; }
            set { SetValue(ref name, value); }
        }

        public string Order
        {
            get { return order; }
            set { SetValue(ref order, value); }
        }

        public object Thumbnail
        {
            get { return thumbnail; }
            set { SetValue(ref thumbnail, value); }
        }

        public int? TrialPit
        {
            get { return trialPit; }
            set { SetValue(ref trialPit, value); }
        }

        public int? Tunnel
        {
            get { return tunnel; }
            set { SetValue(ref tunnel, value); }
        }

        public bool? IsNew
        {
            get => isNew;
            set => isNew = value;
        }

        #endregion

        #region Methods

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
        }

        #endregion
    }
}
