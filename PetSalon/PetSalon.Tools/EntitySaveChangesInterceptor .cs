using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using PetSalon.Models;

namespace PetSalon.Tools
{
    public class EntitySaveChangesInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            SetEntityDateTime(eventData);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = new CancellationToken()
        )
        {
            SetEntityDateTime(eventData);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void SetEntityDateTime(DbContextEventData eventData)
        {
            if (eventData.Context == null) return;
            var entries = eventData.Context.ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is not IEntity entity) continue;

                if (entry.State == EntityState.Added)
                {
                    entity.CreateTime = Utility.GetSysCurrentTime();
                    entity.ModifyTime = Utility.GetSysCurrentTime();
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.ModifyTime = Utility.GetSysCurrentTime();
                }
            }
        }
    }
}
