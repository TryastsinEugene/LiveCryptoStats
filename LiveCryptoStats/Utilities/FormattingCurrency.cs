
using System.Globalization;

namespace LiveCryptoStats.Utilities
{
	static public class FormattingCurrency
	{
		public static string FormatAsCurrency(string value, bool isPercent = false)
		{
			if (decimal.TryParse(value, System.Globalization.NumberStyles.Any,
								 System.Globalization.CultureInfo.InvariantCulture, out var number))
			{
				return isPercent ? number.ToString("0.##") + "%" : number.ToString("C2");
			}
			return value;
		}

		public static string FormatReduction(string valueString)
		{
			if (!decimal.TryParse(valueString, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal supply))
				return valueString; // Якщо не вдалось розпарсити - повертаємо як є

			if (supply >= 1_000_000_000)
				return (supply / 1_000_000_000M).ToString("0.##") + "b"; // мільярди
			if (supply >= 1_000_000)
				return (supply / 1_000_000M).ToString("0.##") + "m"; // мільйони
			if (supply >= 1_000)
				return (supply / 1_000M).ToString("0.##") + "k"; // тисячі

			return supply.ToString("0.##"); // менше тисячі - без скорочення
		}
	}
}
