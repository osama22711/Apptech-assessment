using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apptech.Assessment.Orders;

public interface IOrderRepository
{
    Task<List<Order>> GetExpiredPendingPaymentOrdersAsync(
        DateTime nowUtc,
        int maxResultCount
    );

    Task UpdateAsync(Order order);
}