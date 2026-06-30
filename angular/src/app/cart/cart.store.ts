import { computed, Injectable, signal } from '@angular/core';

export interface CartItem {
    productId: string;
    name: string;
    unitPrice: number;
    quantity: number;
}

export type OrderStatus =
    | 'idle'
    | 'pending-payment'
    | 'paid'
    | 'expired'
    | 'failed';

@Injectable({
    providedIn: 'root',
})
export class CartStore {
    private readonly _items = signal<CartItem[]>([]);
    private readonly _orderStatus = signal<OrderStatus>('idle');

    readonly items = this._items.asReadonly();
    readonly orderStatus = this._orderStatus.asReadonly();

    readonly totalQuantity = computed(() =>
        this._items().reduce((sum, item) => sum + item.quantity, 0)
    );

    readonly totalPrice = computed(() =>
        this._items().reduce(
            (sum, item) => sum + item.quantity * item.unitPrice,
            0
        )
    );

    readonly isEmpty = computed(() => this._items().length === 0);

    addItem(item: CartItem): void {
        this._items.update(items => {
            const existing = items.find(x => x.productId === item.productId);

            if (!existing) {
                return [...items, item];
            }

            return items.map(x =>
                x.productId === item.productId
                    ? { ...x, quantity: x.quantity + item.quantity }
                    : x
            );
        });
    }

    removeItem(productId: string): void {
        this._items.update(items =>
            items.filter(item => item.productId !== productId)
        );
    }

    increase(productId: string): void {
        this._items.update(items =>
            items.map(item =>
                item.productId === productId
                    ? { ...item, quantity: item.quantity + 1 }
                    : item
            )
        );
    }

    decrease(productId: string): void {
        this._items.update(items =>
            items
                .map(item =>
                    item.productId === productId
                        ? { ...item, quantity: item.quantity - 1 }
                        : item
                )
                .filter(item => item.quantity > 0)
        );
    }

    clear(): void {
        this._items.set([]);
        this._orderStatus.set('idle');
    }

    setOrderStatus(status: OrderStatus): void {
        this._orderStatus.set(status);
    }
}