using System.Net;
using System.Text.Json;
using System.Threading;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace ApprenticeManagement.POC.Service;

public static class DeviceManagementApi
{
    private static DeviceManagementService _deviceManagementService = new DeviceManagementService();

    [Function(nameof(GetDeviceUsers))]
    public static async Task<HttpResponseData> GetDeviceUsers(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "devices")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("SayHello");
        var devices = _deviceManagementService.GetAllUsers();
        logger.LogInformation($"Got {devices.Count} devices.");
        var payload = new { devices };
        var response = req.CreateResponse();
        await response.WriteAsJsonAsync(payload, HttpStatusCode.OK);
        return response;

    }

    [Function(nameof(RegisterUserDevice))]
    public static async Task<HttpResponseData> RegisterUserDevice(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "employers/{employer}/users/{user}/devices/{deviceId}")]
        HttpRequestData req,
        string employer,
        string user,
        string deviceId, 
        FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger(nameof(DeviceManagementApi));
        logger.LogInformation($"Request to register device to user. Employer: {employer}, User: {user}, Device: {deviceId}");
        await _deviceManagementService.Register(employer, user, deviceId, logger);
        return req.CreateResponse(HttpStatusCode.OK);
    }

    [Function(nameof(RemoveUserDevice))]
    public static async Task<HttpResponseData> RemoveUserDevice(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "employers/{employer}/users/{user}/devices/{device}")]
        HttpRequestData req,
        string employer,
        string user,
        string deviceId,
        ILogger logger)
    {
        //TODO: not needed for demo
        return req.CreateResponse(HttpStatusCode.OK);
    }
}