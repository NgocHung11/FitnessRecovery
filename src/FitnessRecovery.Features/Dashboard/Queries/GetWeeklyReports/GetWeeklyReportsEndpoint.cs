using System;
using System.Collections.Generic;
using System.Security.Claims;
using FitnessRecovery.Features.Dashboard.DTOs;
using FitnessRecovery.SharedKernel.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FitnessRecovery.Features.Dashboard.Queries.GetWeeklyReports;

public static class GetWeeklyReportsEndpoint
{
    public static void MapGetWeeklyReports(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/reports/weekly", async (
            GetWeeklyReportsHandler handler,
            ClaimsPrincipal claimsPrincipal) =>
        {
            var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var query = new GetWeeklyReportsQuery(userId);
            var result = await handler.HandleAsync(query);

            if (result.IsFailure)
            {
                return Results.BadRequest(ApiResponse.CreateError(result.Error.Description));
            }

            return Results.Ok(ApiResponse<List<WeeklyReportDto>>.CreateSuccess(result.Value));
        })
        .WithName("GetWeeklyReports")
        .RequireAuthorization();
    }
}
