using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace IMOOC.EduCourses.Utils
{
    public class FastSwitchListItem
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private ObservableCollection<string> pptNameList;

        public ObservableCollection<string> PPTNameList
        {
            get { return pptNameList; }
            set { pptNameList = value; }
        }
    }
}
