
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LiveCryptoStats.Utilities
{
	public class StringPriceToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			
			if (value is string)
			{
				if(value.ToString().StartsWith("-"))
					return Brushes.Red;
				else
					return Brushes.Green;
				
			}
			return Brushes.White;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
