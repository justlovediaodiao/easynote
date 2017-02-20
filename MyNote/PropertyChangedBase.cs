using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MyNote
{
    /// <summary>
    /// 提供INotifyPropertyChanged接口实现的基类
    /// </summary>
    class PropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected PropertyChangedBase()
        {
        }

        protected virtual void OnProperChange([CallerMemberName]string propertyName = null)

        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
