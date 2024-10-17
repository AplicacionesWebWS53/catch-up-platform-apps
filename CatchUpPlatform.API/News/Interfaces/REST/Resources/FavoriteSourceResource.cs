namespace CatchUpPlatform.API.News.Interfaces.REST.Resources;

/// <summary>
/// Resource to get a favorite source
/// </summary>
/// <param name="Id">The Favorite Source ID generated by this API</param>
/// <param name="NewsApiKey">The news API key generated by the news provider</param>
/// <param name="SourceId">The news provider source ID</param>
public record FavoriteSourceResource(int Id, string NewsApiKey, string SourceId);