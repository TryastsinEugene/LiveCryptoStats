
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveCryptoStats.Models;
using LiveCryptoStats.Utilities;
using System.Net.Http;
using Newtonsoft.Json;
using LiveChartsCore.Defaults;
using System.Globalization;
using System.Windows.Input;

namespace LiveCryptoStats.ViewModel
{
    class CurrencyDetailsVM : ViewModelBase
    {
		public IEnumerable<ISeries> Series { get; set; }
		public Axis[] XAxes { get; set; }
		
		private Currency _currency;
		public Currency Currency
		{
			get => _currency;
			set
			{
				if (_currency != value)
				{
					_currency = value;
					OnPropertyChanged();
				}
			}
		}

		public ICommand LoadDataCommand { get; set; }

		private const string ApiKey = "Bearer 087a7841ebc2ca77cecf769dd34b3af28bb3783d5308df1ffa2f886fd1b058ae";

		public CurrencyDetailsVM(Currency currency)
		{
			Currency = currency;
			LoadDataCommand = new RelayCommand(p =>
			{
				if(p is string interval)
					LoadData(interval);
			});
			LoadData("h1");
		}

		private async void LoadData(string time)
		{
			using var client = new HttpClient();
			client.DefaultRequestHeaders.Add("Authorization", ApiKey);
			var json = await client.GetStringAsync($"https://rest.coincap.io/v3/assets/{Currency.Name.ToLower()}/history?interval={time}");

			var result = JsonConvert.DeserializeObject<CoinCapResponse>(json);

			//group by date and create candle points
			if (result != null)
			{
				var grouped = result.Data
				.GroupBy(p => DateTimeOffset.FromUnixTimeMilliseconds(p.Time).Date)
				.Select(g => new CandlePoint
				{
					Date = g.Key,
					Open = double.Parse(g.First().PriceUsd, CultureInfo.InvariantCulture),
					High = g.Max(p => double.Parse(p.PriceUsd, CultureInfo.InvariantCulture)),
					Low = g.Min(p => double.Parse(p.PriceUsd, CultureInfo.InvariantCulture)),
					Close = double.Parse(g.Last().PriceUsd, CultureInfo.InvariantCulture)
				})
				.ToList();

				Series = new ISeries[]
				{
					new CandlesticksSeries<FinancialPointI>
					{
						Values = grouped
						.Select(c => new FinancialPointI
						{
							Open = c.Open,
							High = c.High,
							Low = c.Low,
							Close = c.Close
						}).ToArray()
					}
				};

				XAxes = new Axis[]
				{
					new Axis
					{
						LabelsRotation = 15,
						Labels = grouped
								.Select(c => c.Date.ToString("yyyy MMM dd"))
								.ToArray()
					}
				};


				OnPropertyChanged(nameof(Series));
				OnPropertyChanged(nameof(XAxes));
				
			}
			
		}
	}

	public class CoinCapResponse
	{
		[JsonProperty("data")]
		public List<PricePoint> Data { get; set; }
	}

	public class PricePoint
	{
		[JsonProperty("priceUsd")]
		public string PriceUsd { get; set; }

		[JsonProperty("time")]
		public long Time { get; set; }
	}
}
