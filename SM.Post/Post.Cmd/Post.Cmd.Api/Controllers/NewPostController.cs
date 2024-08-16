using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.DTOs;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
  [ApiController]
  [Route("api/v1/[controller]")]
  public class NewPostController : ControllerBase
  {
    private readonly ILogger<EditMessageController> logger;
    private readonly ICommandDispatcher commandDispatcher;

    public NewPostController(ILogger<EditMessageController> logger, ICommandDispatcher commandDispatcher)
    {
      this.logger = logger;
      this.commandDispatcher = commandDispatcher;
    }

    [HttpPost]
    public async Task<ActionResult> NewPostAsync(NewPostCommand command)
    {
      var id = Guid.NewGuid();
      try
      {
        command.Id = id;

        await commandDispatcher.SendAsync(command);

        return StatusCode(StatusCodes.Status201Created, new NewPostResponse { Message = "New post creation request completed" });
      }
      catch(InvalidOperationException ex) 
      {
        logger.Log(LogLevel.Warning, ex, "Client made a bad request");
        return BadRequest(new BaseResponse
        {
          Message = ex.Message
        });
      }
      catch (Exception ex) 
      {
        const string safeErrorMessage = "Error while processing request to create a new post";
        logger.Log(LogLevel.Error, safeErrorMessage, ex, safeErrorMessage);
        return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse
        {
          Id = id,
          Message = safeErrorMessage
        });
      }

    }
  }
}
