using LiveCryptoStats.Models;
using LiveCryptoStats.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LiveCryptoStats.ViewModel
{
	class HomeVM : ViewModelBase
	{
		private readonly PageModel _pageModel;
		public ObservableCollection<Currency> Currencies { get; set; }		
		public HomeVM()
		{
			_pageModel = new PageModel();
			Currencies = new ObservableCollection<Currency>();
			_ = GetAssets(); 
		}

		private async Task GetAssets()
		{
			using (HttpClient client = new HttpClient())
			{
				client.DefaultRequestHeaders.Add("Authorization", "Bearer 58ee904cb0b994e1703c2c6d0ba5a1727b4ce0347d7109f9ddc370078eb3a9c5");

				try
				{
					HttpResponseMessage response = await client.GetAsync("https://rest.coincap.io/v3/assets?limit=10");
					response.EnsureSuccessStatusCode();

					string json = await response.Content.ReadAsStringAsync();

					
					var apiResult = System.Text.Json.JsonSerializer.Deserialize<ApiResponse>(json);

					if (apiResult?.data != null)
					{
						Currencies.Clear(); 

						foreach (var apiCurrency in apiResult.data)
						{
							Currencies.Add(new Currency
							{
								Name = apiCurrency.name,
								Code = apiCurrency.symbol,
								ImageUrl = $"https://assets.coincap.io/assets/icons/{apiCurrency.symbol.ToLower()}@2x.png",
								Price = FormatAsCurrency(apiCurrency.priceUsd),
								Volume = FormatReduction(apiCurrency.volumeUsd24Hr) + "$",
								Supply = FormatReduction(apiCurrency.supply),
								ChangePercent24Hr = FormatAsCurrency(apiCurrency.changePercent24Hr, true) 
							});
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("API error: " + ex.Message);
				}
			}
		}

		private string FormatAsCurrency(string value, bool isPercent = false)
		{
			if (decimal.TryParse(value, System.Globalization.NumberStyles.Any,
								 System.Globalization.CultureInfo.InvariantCulture, out var number))
			{
				return isPercent ? number.ToString("0.##") + "%" : number.ToString("C2");
			}
			return value; 
		}

		private string FormatReduction(string valueString)
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

	public class ApiCurrency
	{
		public string id { get; set; }
		public string rank { get; set; }
		public string symbol { get; set; }
		public string name { get; set; }
		public string supply { get; set; }
		public string maxSupply { get; set; }
		public string marketCapUsd { get; set; }
		public string volumeUsd24Hr { get; set; }
		public string priceUsd { get; set; }
		public string changePercent24Hr { get; set; }
		public string vwap24Hr { get; set; }
	}

	public class ApiResponse
	{
		public List<ApiCurrency> data { get; set; }
	}
}
