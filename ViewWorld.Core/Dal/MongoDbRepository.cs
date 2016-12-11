using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViewWorld.Core.Models;
using MongoDB.Bson;
using ViewWorld.Core.Dal;
using System.Threading;

namespace ViewWorld.Core.Dal
{
    public class MongoDbRepository : IMongoDbRepository
    {

        ApplicationIdentityContext _mongoDbContext { get; set; }
        public MongoDbRepository(ApplicationIdentityContext mongoDbContext)
        {
            _mongoDbContext = _mongoDbContext != null ? _mongoDbContext : mongoDbContext;
        }

        #region Get
        public GetOneResult<TEntity> GetOne<TEntity>(string id) where TEntity : class, new()
        {
            var filter = Builders<TEntity>.Filter.Eq("Id", id);
            return GetOne<TEntity>(filter);
        }

        public GetOneResult<TEntity> GetOne<TEntity>(FilterDefinition<TEntity> filter) where TEntity : class, new()
        {
            var res = new GetOneResult<TEntity>();
            try
            {
                var collection = GetCollection<TEntity>();
                var entity = collection.Find(filter).SingleOrDefault();
                if (entity != null)
                {
                    res.Entity = entity;
                }
                res.Success = true;
                res.ErrorCode = 200;
                return res;
            }
            catch (Exception ex)
            {
                res.Message = DatabaseHelper.NotifyException("GetOne", "Exception getting one " + typeof(TEntity).Name, ex);
                return res;
            }
        }
        /// <summary>
        /// A generic GetOne method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<GetOneResult<TEntity>> GetOneAsync<TEntity>(string id) where TEntity : class, new()
        {
            var filter = Builders<TEntity>.Filter.Eq("Id", id);
            return await GetOneAsync<TEntity>(filter);
        }

        /// <summary>
        /// A generic GetOne method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<GetOneResult<TEntity>> GetOneAsync<TEntity>(FilterDefinition<TEntity> filter) where TEntity : class, new()
        {
            var res = new GetOneResult<TEntity>();
            try
            {
                var collection = GetCollection<TEntity>();
                var entity = await collection.Find(filter).SingleOrDefaultAsync();
                if (entity != null)
                {
                    res.Entity = entity;
                }
                res.Success = true;
                res.ErrorCode = 200;
                return res;
            }
            catch (Exception ex)
            {
                res.Message = DatabaseHelper.NotifyException("GetOne", "Exception getting one " + typeof(TEntity).Name, ex);
                return res;
            }
        }

        /// <summary>
        /// A generic get many method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<GetManyResult<TEntity>> GetManyAsync<TEntity>(IEnumerable<string> ids) where TEntity : class, new()
        {
            try
            {
                var collection = GetCollection<TEntity>();
                var filter = Builders<TEntity>.Filter.In("Id", ids);
                return await GetManyAsync<TEntity>(filter);
            }
            catch (Exception ex)
            {
                var res = new GetManyResult<TEntity>();
                res.Message = DatabaseHelper.NotifyException("GetMany", "Exception getting many " + typeof(TEntity).Name + "s", ex);
                return res;
            }
        }

        /// <summary>
        /// A generic get many method with filter
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<GetManyResult<TEntity>> GetManyAsync<TEntity>(FilterDefinition<TEntity> filter) where TEntity : class, new()
        {
            var res = new GetManyResult<TEntity>();
            try
            {
                var collection = GetCollection<TEntity>();
                var entities = await collection.Find(filter).ToListAsync();
                if (entities != null)
                {
                    res.Entities = entities;
                }
                res.Success = true;
                res.ErrorCode = 200;
                return res;
            }
            catch (Exception ex)
            {
                res.Message = DatabaseHelper.NotifyException("GetMany", "Exception getting many " + typeof(TEntity).Name + "s", ex);
                return res;
            }
        }
        public async Task<GetManyResult<TEntity>> GetMany<TEntity>(FilterDefinition<TEntity> filter, SortDefinition<TEntity> sort) where TEntity : class, new()
        {
            var res = new GetManyResult<TEntity>();
            try
            {
                var collection = GetCollection<TEntity>();
                var entities = await collection.Find(filter).Sort(sort).ToListAsync();
                if (entities != null)
                {
                    res.Entities = entities;
                }
                res.Success = true;
                res.ErrorCode = 200;
                return res;
            }
            catch (Exception ex)
            {
                res.Message = DatabaseHelper.NotifyException("GetMany", "Exception getting many " + typeof(TEntity).Name + "s", ex);
                return res;
            }
        }
        /// <summary>
        /// FindCursor
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="filter"></param>
        /// <returns>A cursor for the query</returns>
        public IFindFluent<TEntity, TEntity> FindCursor<TEntity>(FilterDefinition<TEntity> filter) where TEntity : class, new()
        {
            var res = new GetManyResult<TEntity>();
            var collection = GetCollection<TEntity>();
            var cursor = collection.Find(filter);
            return cursor;
        }

        /// <summary>
        /// A generic get all method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public async Task<GetManyResult<TEntity>> GetAllAsync<TEntity>() where TEntity : class, new()
        {
            var res = new GetManyResult<TEntity>();
            try
            {
                var collection = GetCollection<TEntity>();
                var entities = await collection.Find(new BsonDocument()).ToListAsync();
                if (entities != null)
                {
                    res.Entities = entities;
                }
                res.Success = true;
                res.ErrorCode = 200;
                return res;
            }
            catch (Exception ex)
            {
                res.Message = DatabaseHelper.NotifyException("GetAll", "Exception getting all " + typeof(TEntity).Name + "s", ex);
                return res;
            }
        }

        /// <summary>
        /// A generic Exists method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync<TEntity>(string id) where TEntity : class, new()
        {
            var collection = GetCollection<TEntity>();
            var query = new BsonDocument("Id", id);
            var cursor = collection.Find(query);
            var count = await cursor.CountAsync();
            return (count > 0);
        }

        /// <summary>
        /// A generic count method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<long> CountAsync<TEntity>(string id) where TEntity : class, new()
        {
            var filter = new FilterDefinitionBuilder<TEntity>().Eq("Id", id);
            return await CountAsync<TEntity>(filter);
        }

        /// <summary>
        /// A generic count method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<long> CountAsync<TEntity>(FilterDefinition<TEntity> filter) where TEntity : class, new()
        {
            var collection = GetCollection<TEntity>();
            var cursor = collection.Find(filter);
            var count = await cursor.CountAsync();
            return count;
        }
        #endregion Get

        #region Create
        /// <summary>
        /// A generic Add One method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<Result> AddOneAsync<TEntity>(TEntity item) where TEntity : class, new()
        {
            var res = new Result();
            try
            {
                var collection = GetCollection<TEntity>();
                await collection.InsertOneAsync(item);
                res.Success = true;
                res.Message = "OK";
                res.ErrorCode = 200;
                return res;
            }
            catch (Exception ex)
            {
                res.Message = DatabaseHelper.NotifyException("AddOne", "Exception adding one " + typeof(TEntity).Name, ex);
                return res;
            }
        }
        /// <summary>
        /// Add many documents. 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="documents"></param>
        public void AddMany<TEntity>(IEnumerable<TEntity> documents)
        {
            try
            {
                var collection = GetCollection<TEntity>();
                collection.InsertManyAsync(documents);
            }
            catch (Exception ex)
            {
                throw ex; 
            }
            
        }
        /// <summary>
        /// Add many document async method.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="documents"></param>
        /// <returns></returns>
        public async Task<Result> AddManyAsync<TEntity>(IEnumerable<TEntity> documents)
        {
            var res = new Result();
            try
            {
                var collection = GetCollection<TEntity>();
                await collection.InsertManyAsync(documents);
                res.Success = true;
                res.Message = "OK";
                res.ErrorCode = 200;
                return res;
            }
            catch (Exception ex)
            {
                res.Message = DatabaseHelper.NotifyException("AddManyAsync", "Exception adding many " + typeof(TEntity).Name, ex);
                return res;
            }
        }
        #endregion Create

        #region Delete
        /// <summary>
        /// A generic delete one method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result> DeleteOneAsync<TEntity>(string id) where TEntity : class, new()
        {
            var filter = new FilterDefinitionBuilder<TEntity>().Eq("Id", id);
            return await DeleteOne<TEntity>(filter);
        }

        /// <summary>
        /// A generic delete one method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result> DeleteOne<TEntity>(FilterDefinition<TEntity> filter) where TEntity : class, new()
        {
            var result = new Result();
            try
            {
                var collection = GetCollection<TEntity>();
                var deleteRes = await collection.DeleteOneAsync(filter);
                result.Success = true;
                result.Message = "OK";
                result.ErrorCode = 200;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = DatabaseHelper.NotifyException("DeleteOne", "Exception deleting one " + typeof(TEntity).Name, ex);
                return result;
            }
        }

        /// <summary>
        /// A generic delete many method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<Result> DeleteManyAsync<TEntity>(IEnumerable<string> ids) where TEntity : class, new()
        {
            var filter = new FilterDefinitionBuilder<TEntity>().In("Id", ids);
            return await DeleteMany<TEntity>(filter);
        }

        /// <summary>
        /// A generic delete many method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<Result> DeleteMany<TEntity>(FilterDefinition<TEntity> filter) where TEntity : class, new()
        {
            var result = new Result();
            try
            {
                var collection = GetCollection<TEntity>();
                var deleteRes = await collection.DeleteManyAsync(filter);
                if (deleteRes.DeletedCount < 1)
                {
                    var ex = new Exception();
                    result.Message = DatabaseHelper.NotifyException("DeleteMany", "Some " + typeof(TEntity).Name + "s could not be deleted.", ex);
                    return result;
                }
                result.Success = true;
                result.Message = "OK";
                result.ErrorCode = 200;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = DatabaseHelper.NotifyException("DeleteMany", "Some " + typeof(TEntity).Name + "s could not be deleted.", ex);
                return result;
            }
        }
        #endregion Delete

        #region Update
        /// <summary>
        /// UpdateOne by id
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public async Task<Result> UpdateOneAsync<TEntity>(string id, UpdateDefinition<TEntity> update) where TEntity : class, new()
        {
            var filter = new FilterDefinitionBuilder<TEntity>().Eq("Id", id);
            return await UpdateOneAsync<TEntity>(filter, update);
        }

        /// <summary>
        /// UpdateOne with filter
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="filter"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public async Task<Result> UpdateOneAsync<TEntity>(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update) where TEntity : class, new()
        {
            var result = new Result();
            try
            {
                var collection = GetCollection<TEntity>();
                var updateRes = await collection.UpdateOneAsync(filter, update);
                //if (updateRes.MatchedCount < 1)
                //{
                //    var ex = new Exception();
                //    result.Message = DatabaseHelper.NotifyException("UpdateOne", "ERROR: updateRes.MatchedCount < 1 for entity: " + typeof(TEntity).Name, ex);
                //    return result;
                //}
                result.Success = true;
                result.Message = "OK";
                result.ErrorCode = 200;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = DatabaseHelper.NotifyException("UpdateOne", "Exception updating entity: " + typeof(TEntity).Name, ex);
                return result;
            }
        }
        /// <summary>
        /// Update an entity without changing it's Id
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Result> ReplaceOneAsync<TEntity>(string id,TEntity model) where TEntity : class, new()
        {
            var filter = new FilterDefinitionBuilder<TEntity>().Eq("Id", id);
            return await ReplaceOneAsync<TEntity>(filter, model);
        }
        /// <summary>
        /// Replacing an entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="filter"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Result> ReplaceOneAsync<TEntity>(FilterDefinition<TEntity> filter,TEntity model) where TEntity : class, new()
        {
            var result = new Result();
            try
            {
                var collection = GetCollection<TEntity>();
                var updateRes = await collection.ReplaceOneAsync(filter, model);
                result.Success = true;
                result.Message = "OK";
                result.ErrorCode = 200;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = DatabaseHelper.NotifyException("UpdateOne", "Exception replacing entity: " + typeof(TEntity).Name, ex);
                return result;
            }
        }
        /// <summary>
        /// UpdateMany with Ids
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public async Task<Result> UpdateManyAsync<TEntity>(IEnumerable<string> ids, UpdateDefinition<TEntity> update) where TEntity : class, new()
        {
            var filter = new FilterDefinitionBuilder<TEntity>().In("Id", ids);
            return await UpdateOneAsync<TEntity>(filter, update);
        }

        /// <summary>
        /// UpdateMany with filter
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="filter"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public async Task<Result> UpdateManyAsync<TEntity>(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update) where TEntity : class, new()
        {
            var result = new Result();
            try
            {
                var collection = GetCollection<TEntity>();
                var updateRes = await collection.UpdateManyAsync(filter, update);
                //if (updateRes.MatchedCount < 1)
                //{
                //    var ex = new Exception();
                //    result.Message = DatabaseHelper.NotifyException("UpdateMany", "ERROR: updateRes.MatchedCount < 1 for entities: " + typeof(TEntity).Name + "s", ex);
                //    return result;
                //}
                result.Success = true;
                result.Message = "OK";
                result.ErrorCode = 200;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = DatabaseHelper.NotifyException("UpdateMany", "Exception updating entities: " + typeof(TEntity).Name + "s", ex);
                return result;
            }
        }
        #endregion Update

        #region Find And Update

        /// <summary>
        /// GetAndUpdateOne with filter
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="filter"></param>
        /// <param name="update"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<GetOneResult<TEntity>> GetAndUpdateOneAsync<TEntity>(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update, FindOneAndUpdateOptions<TEntity, TEntity> options) where TEntity : class, new()
        {
            var result = new GetOneResult<TEntity>();
            try
            {
                var collection = GetCollection<TEntity>();
                result.Entity = await collection.FindOneAndUpdateAsync(filter, update, options);
                result.Success = true;
                result.Message = "OK";
                result.ErrorCode = 200;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = DatabaseHelper.NotifyException("GetAndUpdateOne", "Exception getting and updating entity: " + typeof(TEntity).Name, ex);
                return result;
            }
        }

        #endregion Find And Update


        /// <summary>
        /// The private GetCollection method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        private IMongoCollection<TEntity> GetCollection<TEntity>()
        {
            return _mongoDbContext.GetCollection<TEntity>();
        }

        
    }
}