using MaterialAPI.Data;
using MaterialAPI.Model;
using MaterialAPI.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Threading;

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

            group.MapGet("/image/{id}", 
                async (int id, MaterialAPIContext db, CancellationToken token) =>
                {
                    var filePath = new Service(db).GetFilePath(id);
                    return Results.Stream(new MemoryStream(File.ReadAllBytesAsync(filePath, cancellationToken: token).Result), "image/jpeg");
                })
            .WithName("GetImage")
            .WithOpenApi();

            group.MapGet("/image/th/{id}",
            async (int id, MaterialAPIContext db, CancellationToken token) =>
            {
                var filePath = new Service(db).GetThumbnailPath(id);
                return Results.Stream(new MemoryStream(File.ReadAllBytesAsync(filePath, cancellationToken: token).Result), "image/jpeg");
            })
            .WithName("GetThumbnail")
            .WithOpenApi();

            group.MapGet("/recent/{count}/{size}", (int count, int size, MaterialAPIContext db) =>
            {
                return new Service(db).GetRecent(count, size);
            })
            .WithName("GetRecent")
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
