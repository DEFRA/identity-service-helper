namespace Defra.Identity.Unit.Tests.Test.Utils.Mongo;

using MongoDB.Driver;

public interface ITestMongoService
{
    public List<CreateIndexModel<MongoServiceTests.TestModel>> GetIndexes();

    public void SetIndexes(List<CreateIndexModel<MongoServiceTests.TestModel>> indexes);
}
