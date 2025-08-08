using LiveCryptoStats.Models;
using LiveCryptoStats.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveCryptoStats.ViewModel
{
	class HomeVM : ViewModelBase
	{
		private readonly PageModel _pageModel;
		

		public HomeVM()
		{
			_pageModel = new PageModel();
		}
	}
}
