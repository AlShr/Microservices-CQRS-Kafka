﻿using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
  [ApiController]
  [Route("api/v1/[controller]")]
  public class RemoveCommentController : ControllerBase
  {
    private readonly ILogger<RemoveCommentController> logger;
    private readonly ICommandDispatcher commandDispatcher;

    public RemoveCommentController(ILogger<RemoveCommentController> logger, ICommandDispatcher commandDispatcher)
    {
      this.logger = logger;
      this.commandDispatcher = commandDispatcher;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> RemoveCommentAsync(Guid id, RemoveCommentCommand command)
    {
      try
      {
        command.Id = id;
        await commandDispatcher.SendAsync(command);
        return Ok(new BaseResponse
        {
          Message = "Remove comment request completed successfully"
        });
      }
      catch (InvalidOperationException ex)
      {
        logger.Log(LogLevel.Warning, ex, "Client made a bad request");
        return BadRequest(new BaseResponse
        {
          Message = ex.Message
        });
      }
      catch (AggregateNotFoundException ex)
      {
        logger.Log(LogLevel.Warning, ex, "Could not retrieve aggregate, client passed an incorrect post ID targeting the aggregate");
        return BadRequest(new BaseResponse
        {
          Message = ex.Message
        });
      }
      catch (Exception ex)
      {
        const string safeErrorMessage = "Error while processing request to remove comment from a post";
        logger.Log(LogLevel.Error, safeErrorMessage, ex);
        return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
        {
          Message = safeErrorMessage
        });
      }
    }
  }
}
