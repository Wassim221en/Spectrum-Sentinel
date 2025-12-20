using System.Net;
using Microsoft.AspNetCore.Mvc;
using Template.Dashboard.Core.Response;

public static class OperationResponseExtensions
{
    public static IActionResult ToActionResult<T>(this OperationResponse<T> response)
    {
        if (!response.Success)
            return new ObjectResult(response)
            {
                StatusCode = (int)(response.StatusCode ?? HttpStatusCode.BadRequest)
            };

        return new OkObjectResult(response);
    }

    public static IActionResult ToActionResult(this OperationResponse response)
    {
        if (!response.Success)
            return new ObjectResult(response)
            {
                StatusCode = (int)(response.StatusCode ?? HttpStatusCode.BadRequest)
            };

        return new OkObjectResult(response);
    }
}