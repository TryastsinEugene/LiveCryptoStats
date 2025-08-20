using LiveCryptoStats.Models;
using LiveCryptoStats.Utilities;
using LiveCryptoStats.View;
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

		public ICommand CurrencyDetailsCommand { get; }
		public Action<Currency> NavigateToCurrencyDetailsAction { get; set; } 
		public ObservableCollection<Currency> Currencies { get; set; }

		// API key and base URL for CoinCap API
		private const string ApiKey = "Bearer 087a7841ebc2ca77cecf769dd34b3af28bb3783d5308df1ffa2f886fd1b058ae";
		private const string BaseUrl = "https://rest.coincap.io/v3/assets";
		
		public HomeVM()
		{
			_pageModel = new PageModel();
			Currencies = new ObservableCollection<Currency>();
			SearchCommand = new RelayCommand(ExecuteSearch);
			CurrencyDetailsCommand = new RelayCommand(OpenCurrencyDetails);
			//_ = GetAssets(); 
		}

		private void OpenCurrencyDetails(object obj)
		{
			if (obj is Currency currency)
			{
				NavigateToCurrencyDetailsAction?.Invoke(currency);
			}
		}

		private void ExecuteSearch(object obj)
		{
			ExecuteSearchAsync();
		}

		// Search
		private Task ExecuteSearchAsync() => LoadAssetsAsync(SearchText);

		// Top N actives
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
