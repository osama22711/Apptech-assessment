import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ProductAutocompleteItem } from './product-autocomplete-item.model';

@Injectable({
    providedIn: 'root',
})
export class ProductSearchService {
    private readonly baseUrl =
        `${environment.apis.default.url}/api/app/product-autocomplete`;

    constructor(private readonly http: HttpClient) { }

    autocomplete(query: string): Observable<ProductAutocompleteItem[]> {
        const params = new HttpParams().set('query', query);

        return this.http.get<ProductAutocompleteItem[]>(this.baseUrl, {
            params,
        });
    }
}
