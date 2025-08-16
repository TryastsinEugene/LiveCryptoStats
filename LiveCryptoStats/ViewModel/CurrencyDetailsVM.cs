
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveCryptoStats.Models;
using LiveCryptoStats.Utilities;
using System.Net.Http;
using Newtonsoft.Json;
using LiveChartsCore.Defaults;
using System.Globalization;

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

		private const string ApiKey = "Bearer 58ee904cb0b994e1703c2c6d0ba5a1727b4ce0347d7109f9ddc370078eb3a9c5";

		public CurrencyDetailsVM(Currency currency)
		{
			Currency = currency;
			LoadData();
		}

		private async void LoadData()
		{
			using var client = new HttpClient();
			client.DefaultRequestHeaders.Add("Authorization", ApiKey);
			var json = await client.GetStringAsync($"https://rest.coincap.io/v3/assets/{Currency.Name.ToLower()}/history?interval=h1");

			var result = JsonConvert.DeserializeObject<CoinCapResponse>(json);

			//групуємо по даті
			if(result != null)
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
