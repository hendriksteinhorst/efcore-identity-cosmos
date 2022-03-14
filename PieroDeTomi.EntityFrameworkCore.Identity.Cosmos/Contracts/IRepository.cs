using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PieroDeTomi.EntityFrameworkCore.Identity.Cosmos.Contracts
{
    public interface IRepository
    {
        DbSet<TEntity> Table<TEntity>() where TEntity : class, new();

        TEntity GetById<TEntity>(string id) where TEntity : class, new();

        Task<TEntity> GetByIdAsync<TEntity>(string id) where TEntity : class, new();

        TEntity TryFindOne<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, new();

        Task<TEntity> TryFindOneAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, new();

        IQueryable<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, new();

        Task<List<TEntity>> FindAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, new();

        void Add<TEntity>(TEntity entity) where TEntity : class, new();

        Task AddAsync<TEntity>(TEntity entity) where TEntity : class, new();

        void Update<TEntity>(TEntity entity) where TEntity : class, new();

        void DeleteById<TEntity>(string id) where TEntity : class, new();

        void Delete<TEntity>(TEntity entity) where TEntity : class, new();

        void Delete<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, new();

        Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, new();

        Task SaveChangesAsync();
    }
}