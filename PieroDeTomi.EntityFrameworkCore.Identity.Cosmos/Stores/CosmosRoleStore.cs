using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PieroDeTomi.EntityFrameworkCore.Identity.Cosmos.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace PieroDeTomi.EntityFrameworkCore.Identity.Cosmos.Stores
{
    internal class CosmosRoleStore<TRoleEntity> : IRoleClaimStore<TRoleEntity> where TRoleEntity : IdentityRole, new()
    {
        private readonly IRepository _repo;

        public CosmosRoleStore(IRepository repo)
        {
            _repo = repo;
        }

        public async Task<IdentityResult> CreateAsync(TRoleEntity role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            try
            {
                role.NormalizedName = role.Name.ToLower();
                _repo.Add(role);
                await _repo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(TRoleEntity role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            try
            {
                _repo.Delete(role);
                await _repo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }

            return IdentityResult.Success;
        }

        public async Task<TRoleEntity> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(roleId) || string.IsNullOrWhiteSpace(roleId))
                throw new ArgumentNullException(nameof(roleId));

            var role = await _repo.Table<TRoleEntity>()
                .SingleOrDefaultAsync(_ => _.Id == roleId, cancellationToken: cancellationToken);

            return role;
        }

        public async Task<TRoleEntity> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(normalizedRoleName) || string.IsNullOrWhiteSpace(normalizedRoleName))
                throw new ArgumentNullException(nameof(normalizedRoleName));

            var role = await _repo.Table<TRoleEntity>()
                .SingleOrDefaultAsync(_ => _.NormalizedName == normalizedRoleName, cancellationToken: cancellationToken);

            return role;
        }

        public Task<string> GetNormalizedRoleNameAsync(TRoleEntity role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(TRoleEntity role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleNameAsync(TRoleEntity role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(TRoleEntity role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            role.NormalizedName = normalizedName;

            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(TRoleEntity role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            role.Name = roleName;

            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(TRoleEntity role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            role.ConcurrencyStamp = Guid.NewGuid().ToString();

            try
            {
                _repo.Update(role);
                await _repo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.Message });
            }

            return IdentityResult.Success;
        }

        public async Task<IList<Claim>> GetClaimsAsync(TRoleEntity role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            var roleClaims = await _repo.FindAsync<IdentityRoleClaim<string>>(x => x.RoleId == role.Id);

            return roleClaims.Select(x => x.ToClaim()).ToList();
        }

        public async Task AddClaimAsync(TRoleEntity role, Claim claim, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            var roleClaims = await _repo.Table<IdentityRoleClaim<string>>().ToListAsync();

            var roleClaim = new IdentityRoleClaim<string>()
            {
                Id = roleClaims.Any() ? roleClaims.Max(x => x.Id) + 1 : 0,
                ClaimType = claim.Type,
                ClaimValue = claim.Value,
                RoleId = role.Id,
            };

            await _repo.AddAsync(roleClaim);
            await _repo.SaveChangesAsync();
        }

        public async Task RemoveClaimAsync(TRoleEntity role, Claim claim, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            await _repo.DeleteAsync<IdentityRoleClaim<string>>(x =>
                x.RoleId == role.Id && 
                x.ClaimType == claim.Type && 
                x.ClaimValue == claim.Value);

            await _repo.SaveChangesAsync();
        }

        public void Dispose()
        {
        }
    }
}