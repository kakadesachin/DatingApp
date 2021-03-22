import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  /*
    in the previous versions of angular whenever we created a service we used to register
    that under app.module.ts under providers array. but in the newer versions we can just
    use the bellow property and set root value to it.

    also note: Services are singleton and will be destroyed once the app is closed in browser
  */
  providedIn: 'root'
})
export class AccountService {
  baseUrl = "https://localhost:5001/api/"
  constructor(private http:HttpClient) {}
  login(model:any){
      return this.http.post(`${this.baseUrl}${'accounts/login'}`,model);
  }
}
