using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Mime;

namespace PhageWars.Server.Controllers {
	[ApiController]
	[Route("[controller]")]
	public class LevelController
		: ControllerBase {

		private readonly ILogger<LevelController> _logger;

		public LevelController(ILogger<LevelController> logger) {
			_logger = logger;
		}

		[HttpGet]
		public FileContentResult Level(string name) {
			return new FileContentResult(FileManager.GetLevel(name), MediaTypeNames.Application.Octet) { FileDownloadName = name };
		}
	}
}
