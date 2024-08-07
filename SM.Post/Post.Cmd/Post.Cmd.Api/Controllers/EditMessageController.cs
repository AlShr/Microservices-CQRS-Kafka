using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.DTOs;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
  [ApiController]
  [Route("api/v1/[controller]")]
  public class EditMessageController : ControllerBase
  {
    private readonly ILogger<EditMessageController> logger;
    private readonly ICommandDispatcher commandDispatcher;

    public EditMessageController(ILogger<EditMessageController> logger, ICommandDispatcher commandDispatcher)
    {
      this.logger = logger;
      this.commandDispatcher = commandDispatcher;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> EditMessageAsync(Guid id, EditMessageCommand command)
    {
      try
      {
        command.Id = id;
        await commandDispatcher.SendAsync(command);
        return Ok(new BaseResponse { Message = "" });
      }
      catch (InvalidOperationException ex)
      {
        logger.Log(LogLevel.Warning, ex, "Client made a bad request");
        return BadRequest(new BaseResponse { Message = ex.Message });
      }
      catch (AggregateNotFoundException ex)
      {
        logger.Log(LogLevel.Warning, ex, "Could not retrieve aggregate");
        return BadRequest(new BaseResponse { Message = ex.Message });
      }
      catch (Exception ex)
      {
        const string SAFE_ERRROR = "Error while processing request";
        logger.Log(LogLevel.Error, SAFE_ERRROR, ex);
        return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse
        {
          Id = id,
          Message = SAFE_ERRROR
        });
      }
    }
  }
}
