using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LeoLemon.Converters
{
    public class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            long val = System.Convert.ToInt64(value);

            int ms = (int)(val % 1000);
            long full = val / 1000;
            int sec = (int)(full % 60);
            int min = (int)(full / 60 % 60);
            int hour = (int)(full / 3600);

            return string.Format("{0:00}:{1:00}:{2:00}.{3:000}", hour, min, sec, ms);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string str = value.ToString();
            string[] seperated = str.Split(":.".ToCharArray());

            if(seperated != null && seperated.Length == 4)
                return (long)(
                    long.Parse(seperated[0]) * 3600 * 1000 +
                    long.Parse(seperated[1]) * 60 * 1000 +
                    long.Parse(seperated[2]) * 1000 +
                    long.Parse(seperated[3])
                    );

            return 0;
        }
    }
}
