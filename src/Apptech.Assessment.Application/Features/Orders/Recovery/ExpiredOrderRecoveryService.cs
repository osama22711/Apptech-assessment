using System.Threading.Tasks;
using Apptech.Assessment.Orders;
using Apptech.Assessment.Products;
using Volo.Abp.Application.Services;
using Volo.Abp.Timing;
using Volo.Abp.Uow;

namespace Apptech.Assessment.Features.Orders.Recovery;

public class ExpiredOrderRecoveryService : ApplicationService
{
    private const int BatchSize = 100;

    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IClock _clock;

    public ExpiredOrderRecoveryService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IClock clock)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _clock = clock;
    }

    [UnitOfWork(isTransactional: true)]
    public virtual async Task<int> RecoverExpiredOrdersAsync()
    {
        var nowUtc = _clock.Now.ToUniversalTime();

        var expiredOrders = await _orderRepository.GetExpiredPendingPaymentOrdersAsync(
            nowUtc,
            BatchSize
        );

        foreach (var order in expiredOrders)
        {
            foreach (var item in order.Items)
            {
                await _productRepository.ReleaseStockAsync(
                    item.ProductId,
                    item.Quantity
                );
            }

            order.MarkExpired();

            await _orderRepository.UpdateAsync(order);
        }

        return expiredOrders.Count;
    }
}