import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { AskRequest } from '../models/ask-request';
import { AskResponse } from '../models/ask-response';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AiService {

  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  ask(request: AskRequest): Observable<AskResponse> {
    return this.http.post<AskResponse>(
      `${this.apiUrl}/api/AI/ask`,
      request
    );
  }
}