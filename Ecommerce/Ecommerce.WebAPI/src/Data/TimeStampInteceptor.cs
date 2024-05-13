using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Ecommerce.WebAPI.src.Data
{
    public class TimeStampInteceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var entries = eventData.Context!.ChangeTracker.Entries(); // get all monitored entries

            var addedEntries = entries.Where(e => e.State == EntityState.Added); // get all added entries
            var modifiedEntries = entries.Where(e => e.State == EntityState.Modified); // get all modified entries

            foreach (var entry in entries)
            {
                if (entry.Entity is TimeStamp timeStamp)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            timeStamp.CreatedAt = DateTime.UtcNow; // Store all times in UTC
                            timeStamp.UpdatedAt = DateTime.UtcNow;
                            break;
                        case EntityState.Modified:
                            timeStamp.UpdatedAt = DateTime.UtcNow; // Update with current UTC time
                            break;
                    }
                }
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}