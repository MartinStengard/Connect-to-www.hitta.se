using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace connect_to_www_hitta_se.Models
{
	public class FindCompaniesAndPersonsModel : Paging
	{
		public string what { get; set; }
		public string where { get; set; }
	}

	public class Paging
	{
		public int pageNumber { get; set; }
		public int pageSize { get; set; }
	}

	public enum SearchTypes
	{
		companies,
		persons,
		combined,
		company,
		person
	}

}
