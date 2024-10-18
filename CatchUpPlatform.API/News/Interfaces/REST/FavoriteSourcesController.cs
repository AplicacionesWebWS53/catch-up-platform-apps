using System.Net.Mime;
using CatchUpPlatform.API.News.Domain.Model.Queries;
using CatchUpPlatform.API.News.Domain.Services;
using CatchUpPlatform.API.News.Interfaces.REST.Resources;
using CatchUpPlatform.API.News.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CatchUpPlatform.API.News.Interfaces.REST;

/// <summary>
/// REST API Controller for Favorite Sources
/// </summary>
/// <param name="favoriteSourceCommandService">The Favorite Source Command Service</param>
/// <param name="favoriteSourceQueryService">The Favorite Source Query Service</param>
/// <see cref="IFavoriteSourceCommandService"/>
/// <see cref="IFavoriteSourceQueryService"/>
/// since 1.0.0
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Favorite Sources")]

public class FavoriteSourcesController (
    IFavoriteSourceCommandService favoriteSourceCommandService,
    IFavoriteSourceQueryService favoriteSourceQueryService
    ): ControllerBase
{
    /// <summary>
    /// Get Favorite Sources by ID
    /// </summary>
    /// <param name="id">The Favorite Source ID provided by this API</param>
    /// <returns>
    /// A FavoriteSourceResource object
    /// </returns>
    [SwaggerOperation(
        Summary = "Get Favorite Source by ID",
        Description = "Get a Favorite Source Resource by given ID",
        OperationId = "GetFavoriteSourceById")]
    [SwaggerResponse(200, "The Favorite Source Resource was found", typeof(FavoriteSourceResource))]
    [SwaggerResponse(404, "The Favorite Source Resource was not found")]
    [HttpGet("{id}")]
    public async Task<ActionResult> GetFavoriteSourceById(int id)
    {
        var getFavoriteSourceByIdQuery = new GetFavoriteSourceByIdQuery(id);
        var result = await favoriteSourceQueryService.Handle(getFavoriteSourceByIdQuery);
        if(result is null)
        {
            return NotFound();
        }
        var resource = FavoriteSourceResourceFromEntityAssembler.ToResourceFromEntity(result);
        return Ok(result);
    }

    /// <summary>
    /// Create a favorite source
    /// </summary>
    /// <param name="resource">The <see cref="CreateFavoriteSourceResource"/> resource</param>
    /// <returns>
    /// A <see cref="FavoriteSourceResource"/> object containing the created favorite source including the ID
    /// </returns>
    [SwaggerOperation(
        Summary = "Create Favorite Source",
        Description = "Create a Favorite Source with the given news API key and source ID",
        OperationId = "CreateFavoriteSource")]
    [SwaggerResponse(201, "The Favorite Source was created", typeof(FavoriteSourceResource))]
    [SwaggerResponse(400, "The Favorite Source was not created")]
    [HttpPost]
    public async Task<ActionResult> CreateFavoriteSource([FromBody] CreateFavoriteSourceResource resource)
    {
        var createFavoriteSourceCommand = CreateFavoriteSourceCommandFromResourceAssembler
            .ToCommandFromResource(resource);
        var result = await favoriteSourceCommandService.Handle(createFavoriteSourceCommand);
        if (result is null) return BadRequest();
        return CreatedAtAction(nameof(GetFavoriteSourceById), new { id = result.Id },
            FavoriteSourceResourceFromEntityAssembler.ToResourceFromEntity(result));
    }
    
    /// <summary>
    /// Get all favorite sources by news API key
    /// </summary>
    /// <param name="newsApiKey">A string containing the News API Key from the news provider</param>
    /// <returns>
    /// A list of <see cref="FavoriteSourceResource"/> objects for the given news API Key
    /// </returns>
    private async Task<ActionResult> GetAllFavoriteSourcesByNewsApiKey(string newsApiKey)
    {
        var getFavoriteSourcesByNewsApiKeyQuery = new GetAllFavoriteSourcesByNewsApiKeyQuery(newsApiKey);
        var result = await favoriteSourceQueryService.Handle(getFavoriteSourcesByNewsApiKeyQuery);
        var resources = 
            result.Select(FavoriteSourceResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
    
    /// <summary>
    /// Get favorite source by news API key and source ID
    /// </summary>
    /// <param name="newsApiKey">A string containing the news API Key from the news provider</param>
    /// <param name="sourceId">The Source ID from the news provider</param>
    /// <returns>
    /// A <see cref="FavoriteSourceResource"/> object for the given news API Key and Source ID if found, otherwise NotFound
    /// </returns>
    private async Task<ActionResult> getFavoriteSourceByNewsApiKeyAndSourceId(string newsApiKey, string sourceId)
    {
        var getFavoriteSourceByNewsApiKeyAndSourceIdQuery = 
            new GetFavoriteSourceByNewsApiKeyAndSourceIdQuery(newsApiKey, sourceId);
        var result = await favoriteSourceQueryService.Handle(getFavoriteSourceByNewsApiKeyAndSourceIdQuery);
        if(result is null)
        {
            return NotFound();
        }
        var resource = FavoriteSourceResourceFromEntityAssembler.ToResourceFromEntity(result);
        return Ok(resource);
    }
    
    /// <summary>
    /// get Favorite Source(s) according to the query parameters
    /// </summary>
    /// <param name="newsApiKey">The news API Key generated by the news service provider</param>
    /// <param name="sourceId">The Source ID from the news service provider</param>
    /// <returns>
    /// A response as an ActionResult containing the favorite source if News API Key and source ID is provided,
    /// otherwise all favorite sources for the given News API Key
    /// </returns>
    [SwaggerOperation(
        Summary = "Get Favorite Source(s) according to the query parameters",
        Description = "Get a Favorite Source(s) according to the query parameters",
        OperationId = "GetFavoriteSourceFromQuery")]
    [SwaggerResponse(200, "The Favorite Source(s) were found", typeof(FavoriteSourceResource))]
    [HttpGet]
    public async Task<ActionResult> GetFavoriteSourceFromQuery (
        [FromQuery] string newsApiKey, 
        [FromQuery] string sourceId)
    {
        return string.IsNullOrEmpty(sourceId)
            ? await GetAllFavoriteSourcesByNewsApiKey(newsApiKey)
            : await getFavoriteSourceByNewsApiKeyAndSourceId(newsApiKey, sourceId);
    }
}