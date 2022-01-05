using System.ComponentModel;

namespace IMOOC.EduCourses
{
    public class NotificationObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged<T>(ref T properValue, T newValue, string propertyName)
        {
            if (!Equals(properValue, newValue))
            {
                properValue = newValue;
                if(PropertyChanged!=null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
                
            }
        }
    }

   
}

