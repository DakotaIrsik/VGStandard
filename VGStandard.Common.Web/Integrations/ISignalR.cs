using Refit;

namespace VGStandard.Common.Web.Integrations;

/// <summary>
/// API Integration for SignalR.
/// </summary>
public interface ISignalR
{
    /// <summary>
    /// Broadcasts an alert to the hostname specified, for the clientlist.
    /// </summary>
    /// <param name="alertJson">Alert Json Payload</param>
    /// <param name="hostname">URL of the ze-manager target</param>
    /// <param name="clientIdList">CSV list of IDs.</param>
    /// <returns></returns>
    [Post("/alerts/broadcast")]
    Task BroadcastAlert([Body] string alertJson, string hostname, string clientIdList);

    [Post("/messages/broadcast")]
    Task BroadcastMessage([Body] string message);

}
