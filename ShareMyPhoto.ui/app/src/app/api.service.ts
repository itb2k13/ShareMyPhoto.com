import { Injectable } from '@angular/core';
import { Observable, of, throwError } from 'rxjs';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { catchError, tap, map } from 'rxjs/operators';
import config from './app.config';
import { ApiResponse } from './api-response';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  private apiUrl: string;

  constructor(private http: HttpClient) {
    this.apiUrl = config.resourceServer.api;
  }

  private httpHeaders() {
    return new HttpHeaders({
      'Content-Type': 'application/json',
    });
  }

  get(url: string): Observable<ApiResponse> {
    return this.http.get<ApiResponse>(`${this.apiUrl}${url}`, { headers: this.httpHeaders() })
      .pipe(
        tap(cases => console.log('fetched content')),
        catchError(err => { return throwError(err); })
      );
  }


}
