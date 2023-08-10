using MaterialAPI.Data;
using MaterialAPI.Model;
using MaterialAPI.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace MaterialAPI
{
    public static class BatchEndPoints
    {
        public static void MapBatchEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/batch").WithTags(nameof(Category));

            group.MapGet("/", (MaterialAPIContext db) =>
            {
                return new Service(db).GetRecent().ToList();
            })
            .WithName("GetRecentCategories")
            .WithOpenApi();

            group.MapGet("/{id}", (int id, MaterialAPIContext db) =>
            {
                return new Service(db).BuildBatchFromCategory(id);
            })
            .WithName("BuildBatchFromCategory")
            .WithOpenApi();

            group.MapPut("/", (Category category, MaterialAPIContext db) =>
            {
                int changes = new Service(db).UpdateBatch(category);
                return TypedResults.Ok(changes);
            })
            .WithName("UpdateBatchCounters")
            .WithOpenApi();

            group.MapPut("/reset/{id}", (int id, MaterialAPIContext db) =>
            {
                int changes = new Service(db).ResetCategory(id);
                return TypedResults.Ok(changes);
            })
            .WithName("ResetCategory")
            .WithOpenApi();
        }
    }
}
