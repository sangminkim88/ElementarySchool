namespace Common.Models
{
    using Common.Enums;

    public class Student
    {
        #region Fields

        private string name;

        private string number;

        private eSex sex;

        #endregion

        #region Properties

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Number
        {
            get { return number; }
            set { number = value; }
        }

        public eSex Sex
        {
            get { return sex; }
            set { sex = value; }
        }

        #endregion
    }
}
