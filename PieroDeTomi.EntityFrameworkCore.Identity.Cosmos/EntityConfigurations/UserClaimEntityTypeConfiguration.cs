﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace PieroDeTomi.EntityFrameworkCore.Identity.Cosmos.EntityConfigurations
{
    public class UserClaimEntityTypeConfiguration : IEntityTypeConfiguration<IdentityUserClaim<string>>
    {
        private readonly string _tableName;

        public UserClaimEntityTypeConfiguration(string tableName = "Identity_UserClaims")
        {
            _tableName = tableName;
        }

        public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
        {
            builder
                .Property(_ => _.Id)
                .HasConversion(_ => _.ToString(), _ => Convert.ToInt32(_));

            builder
                .UseETagConcurrency()
                .HasPartitionKey(_ => _.UserId)
                .ToContainer(_tableName);
        }
    }
}