using LanguageCenter.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Extensions;

/// <summary>
/// Map <see cref="OperationResult{T}"/> sang HTTP response.
/// Nhờ đó Controller chỉ cần một dòng — không chứa if/else nghiệp vụ dài.
/// </summary>
public static class OperationResultExtensions
{
    public static IActionResult ToActionResult<T>(this OperationResult<T> result)
        => result.Status switch
        {
            OperationStatus.Ok => new OkObjectResult(result.Value),

            OperationStatus.Created => new ObjectResult(result.Value)
            {
                StatusCode = StatusCodes.Status201Created
            },

            OperationStatus.NoContent => new NoContentResult(),

            OperationStatus.BadRequest => new BadRequestObjectResult(
                BuildProblem(result, StatusCodes.Status400BadRequest)),

            OperationStatus.NotFound => new NotFoundObjectResult(
                BuildProblem(result, StatusCodes.Status404NotFound)),

            OperationStatus.Conflict => new ConflictObjectResult(
                BuildProblem(result, StatusCodes.Status409Conflict)),

            OperationStatus.Forbidden => new ObjectResult(
                BuildProblem(result, StatusCodes.Status403Forbidden))
            {
                StatusCode = StatusCodes.Status403Forbidden
            },

            _ => new StatusCodeResult(StatusCodes.Status500InternalServerError)
        };

    private static object BuildProblem<T>(OperationResult<T> result, int statusCode) => new
    {
        status = statusCode,
        message = result.Message,
        errors = result.Errors
    };
}
