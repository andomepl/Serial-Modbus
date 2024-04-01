using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ModbusTool.Converter
{
    public class IndexToNetworkConnection:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is int i)
            {
                if (i == 0 || i == 1)
                    return false;
                else if (i == 2 || i == 3)
                    return true;
            }


            return false;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
