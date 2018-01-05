using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using connect_to_www_hitta_se.Models;

namespace connect_to_www_hitta_se.Controllers
{
	[EnableCors("AllowAllRequests")]
	[Route("api/[controller]")]
	public class FindController : Controller
	{
		[HttpGet]
		public IActionResult Start() {
			var message = @"/api/find/companies?what=&where=&pageNumber=1&pageSize=20";
			return Ok(message);
		}

		// GET http://{url}/api/find/companies?what={companyName|phone}&where={city}&pagenumber=1&pagesize=10
		[HttpGet("companies")]
		public async Task<IActionResult> SearchCompanies([FromQuery] FindCompaniesAndPersonsModel model)
		{
			try
			{
				var client = new Client();
				var res = await client.SearchCompaniesAndPersons(SearchTypes.companies, model);

				if (string.IsNullOrWhiteSpace(res))
					return NoContent();
				else
					return Ok(res);

			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// GET http://{url}/api/find/persons?what={name|address|phone}&where={city}&pagenumber=1&pagesize=10
		[HttpGet("persons")]
		public async Task<IActionResult> SearchPersons([FromQuery] FindCompaniesAndPersonsModel model)
		{
			try
			{
				var client = new Client();
				var res = await client.SearchCompaniesAndPersons(SearchTypes.persons, model);

				if (string.IsNullOrWhiteSpace(res))
					return NoContent();
				else
					return Ok(res);

			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// GET http://{url}/api/find/company/{companyId}
		[HttpGet("company/{id}")]
		public async Task<IActionResult> GetCompany([FromRoute] string id)
		{
			try
			{
				var client = new Client();
				var res = await client.GetCompanyOrPerson(SearchTypes.company, id);

				if (string.IsNullOrWhiteSpace(res))
					return NoContent();
				else
					return Ok(res);

			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// GET http://{url}/api/find/person/{personId}
		[HttpGet("person/{id}")]
		public async Task<IActionResult> GetPerson([FromRoute] string id)
		{
			try
			{
				var client = new Client();
				var res = await client.GetCompanyOrPerson(SearchTypes.person, id);

				if (string.IsNullOrWhiteSpace(res))
					return NoContent();
				else
					return Ok(res);

			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

	}
}
