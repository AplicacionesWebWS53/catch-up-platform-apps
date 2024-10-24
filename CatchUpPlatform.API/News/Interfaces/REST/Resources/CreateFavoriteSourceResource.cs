namespace CatchUpPlatform.API.News.Interfaces.REST.Resources;
/// <summary>
/// Resource to create a favorite source
/// </summary>
/// <param name="NewsApiKey">The news API Key generated by the new provider</param>
/// <param name="SourceId">The new provider source ID</param>
public record CreateFavoriteSourceResource(string NewsApiKey, string SourceId);