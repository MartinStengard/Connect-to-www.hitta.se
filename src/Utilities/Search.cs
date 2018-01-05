using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using connect_to_www_hitta_se.Models;
using System.Text;
using System.Security.Cryptography;

namespace connect_to_www_hitta_se
{
	/*
		* This module connects to "open" api at www.hita.se.
		* http://hitta.github.io/public/http-api
		* http://hitta.github.io/public/http-api/authentication.html
		* http://hitta.github.io/public/http-api/search/combined.html
		* You need to create an account at www.hitta.se to get required callerId and apiKey.
	*/


	public class Client {

		#region Client

		/// <summary>
		/// Return client with required request headers.
		/// </summary>
		private HttpClient GetClient()
		{
			var callerId = "{callerId}";
			var time = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			var key = "{apiKey}";
			var randomChars = RandomString(16);
			var stringToHash = callerId + time + key + randomChars;
			var hashedString = Sha1.SHA1HashStringForUTF8String(stringToHash);

			var client = new HttpClient();
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Add("X-Hitta-CallerId", callerId);
			client.DefaultRequestHeaders.Add("X-Hitta-Time", time.ToString());
			client.DefaultRequestHeaders.Add("X-Hitta-Random", randomChars);
			client.DefaultRequestHeaders.Add("X-Hitta-Hash", hashedString.ToString());

			return client;
		}

		private string RandomString(int length)
		{
			var random = new Random();
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[random.Next(s.Length)]).ToArray());
		}

		#endregion


		#region Search Methods

		public async Task<string> SearchCompaniesAndPersons(SearchTypes type, FindCompaniesAndPersonsModel find)
		{
			var queryList = new List<string>();

			if (!string.IsNullOrWhiteSpace(find.what)) queryList.Add("what=" + find.what.Trim());
			if (!string.IsNullOrWhiteSpace(find.where)) queryList.Add("where=" + find.where.Trim());

			if (find.pageNumber < 1)
				find.pageNumber = 1;
			queryList.Add("page.number=" + find.pageNumber.ToString());

			if (find.pageSize < 1)
				find.pageSize = 1;
			queryList.Add("page.size=" + find.pageSize.ToString());

			var query = string.Join("&", queryList);

			if (query == "")
				throw new Exception("Search parameters are missing");

			query = type.ToString() + "?" + query;

			var client = GetClient();

			HttpResponseMessage response = await client.GetAsync("https://api.hitta.se/publicsearch/v1/" + query);
			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadAsStringAsync();
			}
			else
			{
				return "";
			}
		}


		public async Task<string> GetCompanyOrPerson(SearchTypes type, string id)
		{
			if (string.IsNullOrWhiteSpace(id))
				throw new Exception("Id parameter is missing");

			var query = type.ToString() + "/" + id;

			var client = GetClient();

			HttpResponseMessage response = await client.GetAsync("https://api.hitta.se/publicsearch/v1/" + query);
			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadAsStringAsync();
			}
			else
			{
				return "";
			}
		}

		#endregion

	}


	#region Sha1

	public class Sha1
	{
		/// <summary>
		/// Compute hash for string encoded as UTF8
		/// </summary>
		/// <param name="s">String to be hashed</param>
		/// <returns>40-character hex string</returns>
		public static string SHA1HashStringForUTF8String(string s)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(s);

			var sha1 = SHA1.Create();
			byte[] hashBytes = sha1.ComputeHash(bytes);

			return HexStringFromBytes(hashBytes);
		}

		/// <summary>
		/// Convert an array of bytes to a string of hex digits
		/// </summary>
		/// <param name="bytes">array of bytes</param>
		/// <returns>String of hex digits</returns>
		public static string HexStringFromBytes(byte[] bytes)
		{
			var sb = new StringBuilder();
			foreach (byte b in bytes)
			{
				var hex = b.ToString("x2");
				sb.Append(hex);
			}
			return sb.ToString();
		}
	}

	#endregion

}
