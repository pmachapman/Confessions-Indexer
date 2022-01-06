// -----------------------------------------------------------------------
// <copyright file="DataContext.cs" company="Conglomo">
// Copyright 2021-2022 Conglomo Limited. Please see LICENSE.md for license details.
// </copyright>
// -----------------------------------------------------------------------

namespace Conglomo.Confessions.Indexer;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

/// <summary>
/// The data context.
/// </summary>
/// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
public class DataContext : DbContext
{
    /// <summary>
    /// Initialises a new instance of the <see cref="DataContext"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public DataContext(DbContextOptions<DataContext> options)
      : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the confessions.
    /// </summary>
    /// <value>
    /// The confessions.
    /// </value>
    public DbSet<Confession> Confessions { get; set; } = default!;

    /// <summary>
    /// Gets or sets the database tables.
    /// </summary>
    /// <value>
    /// The database tables.
    /// </value>
    public DbSet<DatabaseTable> DatabaseTables { get; set; } = default!;

    /// <summary>
    /// Gets or sets the index for the scripture references.
    /// </summary>
    /// <value>
    /// The index for the scripture references.
    /// </value>
    public DbSet<ScriptureIndex> ScriptureIndex { get; set; } = default!;

    /// <summary>
    /// Gets or sets the index for the search.
    /// </summary>
    /// <value>
    /// The index for the search.
    /// </value>
    public DbSet<SearchIndex> SearchIndex { get; set; } = default!;

    /// <summary>
    /// Gets or sets the synonyms.
    /// </summary>
    /// <value>
    /// The synonyms.
    /// </value>
    public DbSet<Synonym> Synonyms { get; set; } = default!;

    /// <summary>
    /// Drops the tables from database.
    /// </summary>
    /// <param name="tableName">Name of the table. Specify this to drop a specific table.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The task.
    /// </returns>
    public async Task DropTablesAsync(string? tableName = null, CancellationToken cancellationToken = default)
    {
        List<DatabaseTable> databaseTables;
        if (string.IsNullOrWhiteSpace(tableName))
        {
            databaseTables = this.DatabaseTables.AsNoTracking().ToList();
        }
        else
        {
            databaseTables = new List<DatabaseTable>
                {
                    new DatabaseTable
                    {
                        TableName = tableName,
                    },
                };
        }

        foreach (DatabaseTable databaseTable in databaseTables)
        {
            if (this.Database.IsSqlServer())
            {
                databaseTable.TableName = databaseTable.TableName
                    .Replace("[", "\\[", StringComparison.OrdinalIgnoreCase)
                    .Replace("]", "\\]", StringComparison.OrdinalIgnoreCase);
                await this.Database.ExecuteSqlRawAsync($"DROP TABLE IF EXISTS [{databaseTable.TableName}]", cancellationToken);
            }
            else if (!databaseTable.TableName.Contains('_', StringComparison.OrdinalIgnoreCase))
            {
                // SQLite created tables contain underscores
                databaseTable.TableName = databaseTable.TableName
                    .Replace("`", "\\`", StringComparison.OrdinalIgnoreCase)
                    .Replace("`", "\\`", StringComparison.OrdinalIgnoreCase);
                await this.Database.ExecuteSqlRawAsync($"DROP TABLE IF EXISTS `{databaseTable.TableName}`", cancellationToken);
            }
        }
    }

    /// <summary>
    /// Asynchronously saves all changes made in this context to the database, with identity insert.
    /// </summary>
    /// <typeparam name="T">The entity type to enable identity insert on.</typeparam>
    /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous save operation. The task result contains
    /// the number of state entries written to the database.
    /// </returns>
    /// <remarks>
    /// This method will automatically call Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.DetectChanges
    /// to discover any changes to entity instances before saving to the underlying database.
    /// This can be disabled via Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled.
    /// Multiple active operations on the same context instance are not supported. Use
    /// 'await' to ensure that any asynchronous operations have completed before calling
    /// another method on this context.
    /// </remarks>
    public async Task<int> SaveChangesWithIdentityInsertAsync<T>(CancellationToken cancellationToken = default)
        where T : IIdentifiable
    {
        if (this.Database.IsSqlServer())
        {
            // See https://docs.microsoft.com/en-us/ef/core/saving/explicit-values-generated-properties for details
            await this.Database.OpenConnectionAsync(cancellationToken).ConfigureAwait(true);
            int returnValue;
            try
            {
                await this.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT [{typeof(T).Name}] ON", cancellationToken).ConfigureAwait(true);
                returnValue = await this.SaveChangesAsync(cancellationToken).ConfigureAwait(true);
                await this.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT [{typeof(T).Name}] ON", cancellationToken).ConfigureAwait(true);
            }
            finally
            {
                await this.Database.CloseConnectionAsync().ConfigureAwait(true);
            }

            return returnValue;
        }
        else
        {
            // SQLite doesn't require identity insert
            return await this.SaveChangesAsync(cancellationToken).ConfigureAwait(true);
        }
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // EF Core creates that table anyway, so lets just ensure it is singular so we can drop it
        string query;
        if (this.Database.IsSqlServer())
        {
            query = "SELECT TABLE_NAME AS TABLENAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME NOT LIKE '%Migration%' AND TABLE_NAME <> 'sysdiagrams' ORDER BY TABLE_NAME";
        }
        else
        {
            query = "SELECT name AS TABLENAME FROM sqlite_master WHERE type = 'table' ORDER BY name;";
        }

        modelBuilder.Entity<DatabaseTable>()
            .ToSqlQuery(query)
            .HasNoKey();
        modelBuilder.Entity<ScriptureIndex>()
            .HasKey(s => s.Id);
        modelBuilder.Entity<ScriptureIndex>()
            .Property(s => s.Book).HasMaxLength(45);
        modelBuilder.Entity<ScriptureIndex>()
            .HasIndex(s => new { s.Book, s.ChapterNumber });
        modelBuilder.Entity<ScriptureIndex>()
            .HasIndex(s => new { s.SearchIndexId, s.Book, s.ChapterNumber })
            .IsUnique();
        modelBuilder.Entity<ScriptureIndex>()
            .HasIndex(s => s.SearchIndexId);
        modelBuilder.Entity<ScriptureIndex>()
            .HasOne(s => s.SearchIndex)
            .WithMany(i => i!.ScriptureIndex)
            .HasForeignKey(s => s.SearchIndexId);
        modelBuilder.Entity<SearchIndex>()
            .HasKey(s => s.Id);
        modelBuilder.Entity<SearchIndex>()
            .HasIndex(s => s.ConfessionId);
        modelBuilder.Entity<SearchIndex>()
            .HasOne(s => s.Confession)
            .WithMany(c => c!.SearchIndex)
            .HasForeignKey(s => s.ConfessionId);
        modelBuilder.Entity<Synonym>()
            .HasKey(s => s.Id);

        // Use singular table names
        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Use the entity name instead of the Context.DbSet<T> name
            // refs https://docs.microsoft.com/en-us/ef/core/modeling/entity-types?tabs=fluent-api#table-name
            modelBuilder.Entity(entityType.ClrType).ToTable(entityType.ClrType.Name);
        }
    }
}
