using LiveCryptoStats.Models;
using LiveCryptoStats.ViewModel;
using System.Windows.Controls;


namespace LiveCryptoStats.View
{
	/// <summary>
	/// Interaction logic for CurrencyDetails.xaml
	/// </summary>
	public partial class CurrencyDetails : UserControl
	{
		public CurrencyDetails()
		{
			InitializeComponent();
		}

		public CurrencyDetails(Currency currency) : this()
		{
			DataContext = new CurrencyDetailsVM(currency); 
		}
	}
}
