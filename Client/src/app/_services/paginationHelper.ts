import { HttpParams } from "@angular/common/http";
import { PaginatedResult } from "../_models/Pagination";
import { map } from "rxjs";
import { HttpClient } from "@angular/common/http";


export function getPaginatedResult<T>(url : string, params: HttpParams, http: HttpClient) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();

    return http.get<T>(url, { observe: 'response', params }).pipe(
        map(response => {

            if (response.body) {
                paginatedResult.result = response.body;
            }
            const pagination = response.headers.get('Pagination');
            if (pagination !== null) {
                paginatedResult.pagination = JSON.parse(pagination);
            }

            return  paginatedResult;

        })
    );
}
  
  //Headers de la pagination
  
  export function getPaginationHeaders(pageNumber?: number, pageSize?: number) {
    let params = new HttpParams();
  
    if (pageNumber !== undefined) {
      params = params.append('pageNumber', pageNumber);
    }
  
    if (pageSize !== undefined) {
      params = params.append('pageSize', pageSize);
    }
  
    return params;
  }