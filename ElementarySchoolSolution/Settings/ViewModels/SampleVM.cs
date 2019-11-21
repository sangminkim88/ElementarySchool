namespace Settings.ViewModels
{
    using Common.Models;
    using System.Collections.ObjectModel;
    using WpfBase.Bases;

    public class SampleVM : ViewModelBase
    {
        private ObservableCollection<Student> students = new ObservableCollection<Student>();

        public ObservableCollection<Student> Students
        {
            get { return students; }
            set { students = value; }
        }

        public SampleVM()
        {
        }

    }
}
