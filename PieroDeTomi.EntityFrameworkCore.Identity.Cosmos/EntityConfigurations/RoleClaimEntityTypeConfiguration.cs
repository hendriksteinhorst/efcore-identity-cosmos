using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace PieroDeTomi.EntityFrameworkCore.Identity.Cosmos.EntityConfigurations
{
    public class RoleClaimEntityTypeConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
    {
        private readonly string _tableName;

        public RoleClaimEntityTypeConfiguration(string tableName = "Identity_RoleClaims")
        {
            _tableName = tableName;
        }

        public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
        {
            builder
                .Property(_ => _.Id)
                .HasConversion(_ => _.ToString(), _ => Convert.ToInt32(_));

            builder
                .UseETagConcurrency()
                .HasPartitionKey(_ => _.RoleId);

            builder.ToContainer(_tableName);
        }
    }
}