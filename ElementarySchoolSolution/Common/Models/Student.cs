namespace Common.Models
{
    public class Student
    {
        #region Fields

        private string name;

        private string note;

        private int number;

        private string parent;

        private string parentNumber;

        #endregion

        #region Constructors

        public Student()
        {
        }

        public Student(string name)
        {
            this.name = name;
        }

        public Student(int number, string name, string parent, string parentNumber, string note)
        {
            this.number = number;
            this.name = name;
            this.parent = parent;
            this.parentNumber = parentNumber;
            this.note = note;
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Note
        {
            get { return note; }
            set { note = value; }
        }

        public int Number
        {
            get { return number; }
            set { number = value; }
        }

        public string Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public string ParentNumber
        {
            get { return parentNumber; }
            set { parentNumber = value; }
        }

        #endregion
    }
}
