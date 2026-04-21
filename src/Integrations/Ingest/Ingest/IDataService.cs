namespace Defra.Identity.Ingest;

public interface IDataService<in TModel>
{
    Task Upsert(TModel model, CancellationToken cancellationToken = default);
}