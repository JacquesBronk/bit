using Bit.Lib.Domain.Service;
using Bit.Lib.Infra;
using Bit.Log.Common;
using Microsoft.AspNetCore.Mvc;

namespace Bit.EndpointHandlers;

public static class BitEndpoints
{
    public static async Task<IResult> MapGetFlagEndpointHandler(string flag, IFlagService flagService)
    {
        try
        {
            var cloudEvent = await flagService.GetFlag(flag);
            if (cloudEvent.Type == ExceptionCodes.Data.CannotFindKey)
            {
                return Results.NoContent();
            }
            var cloudEventJson = cloudEvent.ConvertCloudEventToJson();
            return Results.Ok(cloudEventJson);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> MapIsEnabledEndpoint(string flag, IFlagService flagService)
    {
        try
        {
            var cloudEvent = await flagService.IsFlagEnabled(flag);
            var cloudEventJson = cloudEvent.ConvertCloudEventToJson();
            return Results.Ok(cloudEventJson);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> MapUpdateFlagEndpoint(string flag, bool enabled, IFlagService flagService)
    {
        try
        {
            var cloudEvent = await flagService.UpdateFlag(flag, enabled);
            var cloudEventJson = cloudEvent.ConvertCloudEventToJson();
            return Results.Ok(cloudEventJson);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> MapCreateFlagEndpoint(string flag, bool enabled, IFlagService flagService)
    {
        try
        {
            var cloudEvent = await flagService.CreateFlag(flag, enabled);
            var cloudEventJson = cloudEvent.ConvertCloudEventToJson();
            return Results.Ok(cloudEventJson);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> MapDeleteFlagEndpoint(string flag, IFlagService flagService)
    {
        try
        {
            var cloudEvent = await flagService.DeleteFlag(flag);
            var cloudEventJson = cloudEvent.ConvertCloudEventToJson();
            return Results.Ok(cloudEventJson);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> MapFlushEndpoint(IFlagService flagService)
    {
        try
        {
            var cloudEvent = await flagService.FlushCache();
            var cloudEventJson = cloudEvent.ConvertCloudEventToJson();
            return Results.Ok(cloudEventJson);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}