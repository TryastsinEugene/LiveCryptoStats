using LiveCryptoStats.Models;
using LiveCryptoStats.Utilities;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows.Input;

namespace LiveCryptoStats.ViewModel
{
	class HomeVM : ViewModelBase
	{
		private readonly PageModel _pageModel;
		public string SearchText { get; set; } = string.Empty;
		public ICommand SearchCommand { get; }
		public ObservableCollection<Currency> Currencies { get; set; }

		// API ключ та базовий URL для CoinCap API
		private const string ApiKey = "Bearer 58ee904cb0b994e1703c2c6d0ba5a1727b4ce0347d7109f9ddc370078eb3a9c5";
		private const string BaseUrl = "https://rest.coincap.io/v3/assets";
		
		public HomeVM()
		{
			_pageModel = new PageModel();
			Currencies = new ObservableCollection<Currency>();
			SearchCommand = new RelayCommand(ExecuteSearch);
			//_ = GetAssets(); 
		}

		private void ExecuteSearch(object obj)
		{
			ExecuteSearchAsync();
		}

		// Пошук
		private Task ExecuteSearchAsync() => LoadAssetsAsync(SearchText);

		// Топ N активів
		private Task GetAssets() => LoadAssetsAsync(limit: 10);

		private async Task LoadAssetsAsync(string query = null, int? limit = null)
		{
			using (HttpClient client = new HttpClient())
			{
				client.DefaultRequestHeaders.Add("Authorization", ApiKey);

				try
				{
					// Формуємо URL
					var url = BaseUrl;
					if (!string.IsNullOrWhiteSpace(query))
						url += $"?search={Uri.EscapeDataString(query)}";
					else if (limit.HasValue)
						url += $"?limit={limit.Value}";

					// Виконуємо запит
					HttpResponseMessage response = await client.GetAsync(url);
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
								Price = FormattingCurrency.FormatAsCurrency(apiCurrency.priceUsd),
								Volume = FormattingCurrency.FormatReduction(apiCurrency.volumeUsd24Hr) + "$",
								Supply = FormattingCurrency.FormatReduction(apiCurrency.supply),
								ChangePercent24Hr = FormattingCurrency.FormatAsCurrency(apiCurrency.changePercent24Hr, true)
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

		



	}

	

	
}
