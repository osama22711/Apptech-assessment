import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import {
    Subject,
    catchError,
    debounceTime,
    distinctUntilChanged,
    filter,
    map,
    of,
    switchMap,
    takeUntil,
    tap,
} from 'rxjs';
import { CartStore } from '../../cart/cart.store';
import { ProductAutocompleteItem } from '../product-autocomplete-item.model';
import { ProductSearchService } from '../product-search.service';

@Component({
    selector: 'app-product-search',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './product-search.component.html',
    styleUrl: './product-search.component.scss',
})
export class ProductSearchComponent implements OnInit, OnDestroy {
    readonly searchControl = new FormControl<string>('', {
        nonNullable: true,
    });

    readonly results = signal<ProductAutocompleteItem[]>([]);
    readonly isLoading = signal(false);
    readonly errorMessage = signal<string | null>(null);

    private readonly destroy$ = new Subject<void>();

    constructor(
        private readonly productSearchService: ProductSearchService,
        private readonly cartStore: CartStore
    ) { }

    ngOnInit(): void {
        this.searchControl.valueChanges
            .pipe(
                map(value => value.trim()),

                tap(query => {
                    this.errorMessage.set(null);

                    if (query.length < 3) {
                        this.results.set([]);
                        this.isLoading.set(false);
                    }
                }),

                filter(query => query.length >= 3),

                debounceTime(250),

                distinctUntilChanged(),

                tap(() => {
                    this.isLoading.set(true);
                }),

                switchMap(query =>
                    this.productSearchService.autocomplete(query).pipe(
                        catchError(() => {
                            this.errorMessage.set('Unable to load suggestions.');
                            return of([]);
                        })
                    )
                ),

                takeUntil(this.destroy$)
            )
            .subscribe(items => {
                this.results.set(items);
                this.isLoading.set(false);
            });
    }

    addToCart(product: ProductAutocompleteItem): void {
        this.cartStore.addItem({
            productId: product.productId,
            name: product.name,
            unitPrice: 0, // autocomplete endpoint currently returns no price
            quantity: 1,
        });
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }
}