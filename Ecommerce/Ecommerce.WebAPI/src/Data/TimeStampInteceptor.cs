using Ecommerce.Core.src.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Ecommerce.WebAPI.src.Data
{
    public class TimeStampInteceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var entries = eventData.Context!.ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                if (entry.Entity is TimeStamp timeStamp)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            timeStamp.CreatedAt = DateTime.UtcNow;
                            timeStamp.UpdatedAt = DateTime.UtcNow;
                            break;
                        case EntityState.Modified:
                            timeStamp.UpdatedAt = DateTime.UtcNow;
                            break;
                    }
                }
            }
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}