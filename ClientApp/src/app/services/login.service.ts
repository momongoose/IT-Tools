import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

const URL = "https://localhost:7015/api/"
const httpOptions = {
  headers: new HttpHeaders({ "Content-Type": "application/json", responseType: 'text'}) //this describes the type of Data that will be send
}

@Injectable({
  providedIn: 'root'
})
export class LoginService {

  constructor(private http: HttpClient) { }

  login(data : JSON): Observable<any> {
    return this.http.post(URL + "login", data, httpOptions);
  }
}
