using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator_RISCV
{
    class Data : INotifyPropertyChanged
    {
        private string data_memory;
        public string Data_memory
        {
            get { return data_memory; }
            set
            {
                data_memory = value;
                RaisePropertyChanged("Data_memory");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
