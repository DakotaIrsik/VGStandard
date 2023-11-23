using Refit;

namespace VGStandard.Common.Web.Integrations;

/// <summary>
/// Api Integration for OneSignal.
/// </summary>
public interface IOneSignal
{
    /// <summary>
    /// Retrives an individual player from Signal R
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    #region Players
    [Get("/players/{Id")]
    Task<List<object>> Get(string playerId);
    #endregion

    /// <summary>
    /// Retrives AllSegments from OneSignal
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    #region Segments
    [Headers("Authorization: Bearer")]
    [Get("/{AppId}/segments")]
    Task<object> GetSegments(string AppId);
    #endregion
}
