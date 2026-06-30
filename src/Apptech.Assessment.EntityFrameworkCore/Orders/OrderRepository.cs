using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apptech.Assessment.EntityFrameworkCore;
using Apptech.Assessment.Orders;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.DependencyInjection;

namespace Apptech.Assessment.EntityFrameworkCore.Orders;

public class OrderRepository : IOrderRepository, ITransientDependency
{
    private readonly AssessmentDbContext _dbContext;

    public OrderRepository(AssessmentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Order>> GetExpiredPendingPaymentOrdersAsync(
        DateTime nowUtc,
        int maxResultCount
    )
    {
        return await _dbContext.Orders
            .Include(order => order.Items)
            .Where(order =>
                order.Status == OrderStatus.PendingPayment &&
                order.ExpiresAtUtc <= nowUtc
            )
            .OrderBy(order => order.ExpiresAtUtc)
            .Take(maxResultCount)
            .ToListAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        _dbContext.Orders.Update(order);
        await _dbContext.SaveChangesAsync();
    }
}