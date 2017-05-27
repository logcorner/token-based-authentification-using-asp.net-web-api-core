using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace TokenAuthWebApiCore.Server.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	public class ValuesController : Controller
	{
		// GET api/values
		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var result = await Task.FromResult(
						 new string[] { "value1", "value2" });
			return Ok((result));
		}

		// GET api/values/5
		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			var result = await Task.FromResult("value");
			return Ok((result));
		}

		// POST api/values
		[HttpPost]
		public async Task<IActionResult> Post([FromBody]string value)
		{
			await Task.CompletedTask;
			return Ok();
		}

		// PUT api/values/5
		[HttpPut("{id}")]
		public async Task<IActionResult> Put(int id, [FromBody]string value)
		{
			await Task.CompletedTask;
			return Ok();
		}

		// DELETE api/values/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			await Task.CompletedTask;
			return Ok();
		}
	}
}