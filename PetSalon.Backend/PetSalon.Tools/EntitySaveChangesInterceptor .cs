using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using PetSalon.Models;

namespace PetSalon.Tools
{
    /// <summary>
    /// EF Core 8.0 compatible SaveChangesInterceptor for automatic audit field management.
    /// Automatically sets CreateTime, CreateUser, ModifyTime, and ModifyUser for entities implementing IEntity.
    /// </summary>
    public class EntitySaveChangesInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateAuditFields(eventData);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, 
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            UpdateAuditFields(eventData);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        /// <summary>
        /// Updates audit fields (CreateTime, CreateUser, ModifyTime, ModifyUser) for entities implementing IEntity.
        /// Compatible with EF Core 8.0 change tracking improvements.
        /// </summary>
        /// <param name="eventData">The event data containing the DbContext and change tracker information</param>
        private void UpdateAuditFields(DbContextEventData eventData)
        {
            if (eventData.Context == null) return;

            var now = Utility.GetSysCurrentTime();
            const string systemUser = "SYSTEM"; // TODO: Replace with proper user context service

            var entries = eventData.Context.ChangeTracker.Entries()
                .Where(e => e.Entity is IEntity && 
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (IEntity)entry.Entity;

                switch (entry.State)
                {
                    case EntityState.Added:
                        entity.CreateTime = now;
                        entity.ModifyTime = now;
                        entity.CreateUser = systemUser;
                        entity.ModifyUser = systemUser;
                        break;

                    case EntityState.Modified:
                        entity.ModifyTime = now;
                        entity.ModifyUser = systemUser;
                        
                        // Prevent CreateUser and CreateTime from being overwritten
                        // This is more explicit and compatible with EF Core 8.0
                        var createTimeProperty = entry.Property(nameof(IEntity.CreateTime));
                        var createUserProperty = entry.Property(nameof(IEntity.CreateUser));
                        
                        if (createTimeProperty != null)
                            createTimeProperty.IsModified = false;
                            
                        if (createUserProperty != null)
                            createUserProperty.IsModified = false;
                        break;
                }
            }
        }
    }
}
