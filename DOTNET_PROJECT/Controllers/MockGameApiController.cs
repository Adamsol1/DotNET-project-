// Mock API controller used only when running the project without a real backend.
// This serves as a temporary stand-in for the actual Game API, allowing frontend
// development and testing to continue even if the database or real logic isn't ready.
//
// It simulates basic game actions like starting a game, fetching the current story node,
// and handling player choices, returning static (hardcoded) responses for demonstration.
#if FRONTEND_ONLY
using Microsoft.AspNetCore.Mvc;
using DOTNET_PROJECT.Application.Dtos;

namespace DOTNET_PROJECT.Controllers
{
    [ApiController]
    [Route("api/game")]
    public class MockGameApiController : ControllerBase
    {
        [HttpPost("start")]
        public ActionResult<GameSessionDto> Start([FromBody] StartGameRequest req)
            => Ok(new GameSessionDto { PlayerCharacterId = 1, UserId = req.UserId, StoryNodeId = 1 });

        [HttpGet("current")]
        public ActionResult<StoryNodeDto> Current([FromQuery] int playerId)
            => Ok(new StoryNodeDto
            {
                Id = 1,
                Title = "Awakening in Cryopod",
                BackgroundUrl = "assets/bg/space-tunnel.png",
                Description = "Cold. Darkness. Why am I awake?",
                Dialogues = new List<DialogueDto>
                {
                    new DialogueDto { Order = 1, Text = "Alarms echo in the distance." },
                    new DialogueDto { Order = 2, Text = "The pod seal hisses…" }
                },
                Choices = new List<ChoiceDto>
                {
                    new ChoiceDto { Id = 10, Text = "Force the hatch open" },
                    new ChoiceDto { Id = 11, Text = "Wait for emergency release" }
                }
            });

        [HttpPost("choice")]
        public IActionResult Choice([FromBody] MakeChoiceRequest req) => Ok();
    }

    public class StartGameRequest { public int UserId { get; set; } public string Name { get; set; } = string.Empty; }
    public class MakeChoiceRequest { public int PlayerId { get; set; } public int ChoiceId { get; set; } }
}
#endif
