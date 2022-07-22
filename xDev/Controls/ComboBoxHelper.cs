using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace xDev.Controls
{
    public partial class ComboBoxHelper
    {
        private void comboboxScrollWithoutFocusEvent(object sender, MouseWheelEventArgs e)
        {
            int index = (sender as ComboBox).SelectedIndex + 1;

            if (e.Delta < 0)
            {
                if (index > 0)
                {
                    (sender as ComboBox).SelectedIndex = index-1;
                }
            }
            else
            {
                if (index < (sender as ComboBox).Items.Count)
                {
                    (sender as ComboBox).SelectedIndex = --index;
                }
            }
        }
    }
}
