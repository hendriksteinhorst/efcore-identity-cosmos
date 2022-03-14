using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PieroDeTomi.EntityFrameworkCore.Identity.Cosmos.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PieroDeTomi.EntityFrameworkCore.Identity.Cosmos.Repositories
{
    public class CosmosIdentityRepository<TDbContext, TUserEntity> : IRepository
        where TDbContext : CosmosIdentityDbContext<TUserEntity>
        where TUserEntity : IdentityUser
    {
        protected TDbContext _db;

        public CosmosIdentityRepository(TDbContext db)
        {
            _db = db;
        }

        public DbSet<TEntity> Table<TEntity>() where TEntity : class, new()
        {
            return _db.Set<TEntity>();
        }

        public TEntity GetById<TEntity>(string id) where TEntity : class, new()
        {
            return _db.Set<TEntity>().WithPartitionKey(id).Single();
        }

        public Task<TEntity> GetByIdAsync<TEntity>(string id) where TEntity : class, new()
        {
            return _db.Set<TEntity>().WithPartitionKey(id).SingleAsync();
        }

        public TEntity TryFindOne<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, new()
        {
            return _db.Set<TEntity>().SingleOrDefault(predicate);
        }

        public Task<TEntity> TryFindOneAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, new()
        {
            return _db.Set<TEntity>().SingleOrDefaultAsync(predicate);
        }

        public IQueryable<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, new()
        {
            return _db.Set<TEntity>().Where(predicate);
        }

        public Task<List<TEntity>> FindAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, new()
        {
            return _db.Set<TEntity>().Where(predicate).ToListAsync();
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class, new()
        {
            _db.Add(entity);
        }

        public async Task AddAsync<TEntity>(TEntity entity) where TEntity : class, new()
        {
            await _db.AddAsync(entity);
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class, new()
        {
            var dbEntry = _db.Entry(entity);
            dbEntry.State = EntityState.Modified;
        }

        public void DeleteById<TEntity>(string id) where TEntity : class, new()
        {
            var entity = GetById<TEntity>(id);
            Delete(entity);
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class, new()
        {
            _db.Remove(entity);
        }

        public void Delete<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, new()
        {
            var entities = _db.Set<TEntity>().Where(predicate).ToList();
            entities.ForEach(entity => _db.Remove(entity));
        }

        public async Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, new()
        {
            var entities = await _db.Set<TEntity>().Where(predicate).ToListAsync();
            entities.ForEach(entity => _db.Remove(entity));
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}