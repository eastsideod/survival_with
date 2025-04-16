namespace AspNetCoreStudy.Service;

using AspNetCoreStudy.DB.Context;
using AspNetCoreStudy.DB.Entity;
using AspNetCoreStudy.DTO;
using Microsoft.EntityFrameworkCore;

public sealed class AccountService
{
    private ILogger<AccountService> logger;

    public AccountService(ILogger<AccountService> logger)
    {
        this.logger = logger;
    }

    public async ValueTask<bool> TryRegister(AccountDTO dto)
    {
        using (var accountDb = new AccountDbContext())
        {
            var entity = new AccountEntity
            {
                Id = dto.Id,
                Password = dto.Password,
                CreatedTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            try
            {
                await accountDb.Account.AddAsync(entity);
                await accountDb.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                this.logger.LogWarning($"Failed to TryRegister. InnerException.Message:{ex.InnerException?.Message}, dto.Id:{dto.Id}.");
                return false;
            }
        }

        return true;
    }

    public async ValueTask<bool> TryDeregister(AccountDTO dto)
    {
        using (var accountDb = new AccountDbContext())
        {
            var entity = await accountDb.Account
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == dto.Id && e.Password == dto.Password);

            if (entity == null)
            {
                return false;
            }

            var entry = accountDb.Remove(entity);
            if (entry.State != EntityState.Deleted)
            {
                return false;
            }

            await accountDb.SaveChangesAsync();

            return true;
        }
    }

    public async ValueTask<bool> TryLogin(AccountDTO dto)
    {
        using (var accountDb = new AccountDbContext())
        {
            var entity = await accountDb.Account
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == dto.Id);

            if (entity == null)
            {
                return false;
            }

            return dto.Password == entity.Password;
        }
    }
}
