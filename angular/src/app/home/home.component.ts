import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { CartStore, OrderStatus } from '../cart/cart.store';
import { ProductSearchComponent } from '../products/product-search/product-search.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, ProductSearchComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent {
  constructor(public readonly cartStore: CartStore) { }

  simulateOrderStatus(status: OrderStatus): void {
    this.cartStore.setOrderStatus(status);
  }

  clearCart(): void {
    this.cartStore.clear();
  }
}