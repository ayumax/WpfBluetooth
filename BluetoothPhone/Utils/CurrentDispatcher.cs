using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothPhone.Utils
{
    class CurrentDispatcher
    {
        public static void Dispatch(Action action)
        {
            System.Windows.Threading.Dispatcher dispatcher = System.Windows.Application.Current.Dispatcher;
            if (dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcher.Invoke(() => action());
            }
        }
    }
}
