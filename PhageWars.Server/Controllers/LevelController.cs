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

		[HttpGet(nameof(Level))]
		public FileContentResult Level(string name) {
			return new(FileManager.GetLevel(name), MediaTypeNames.Application.Octet) { FileDownloadName = name };
		}
		
		[HttpGet]
		public JsonResult Levels() {
			return new(FileManager.GetLevelNames());
		}	
		
		[HttpPost(nameof(UploadLevel))]
		public StatusCodeResult UploadLevel(string name, string levelData) {
			FileManager.StoreLevel(name, levelData);
			return new OkResult();
		}
	}
}
