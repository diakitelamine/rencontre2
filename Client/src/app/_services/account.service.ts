import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { User } from '../_models/user';
import { environment } from 'src/environments/environment.development';
import { Token } from '@angular/compiler';


@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private currentUserSource = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currentUserSource.asObservable();
  private baseUrl =  environment.apiUrl;

  constructor(private http: HttpClient) {}
  /**
   * Login
   * @param model 
   * @returns 
   */
  login(model: any): Observable<User> {
    return this.http.post<User>(`${this.baseUrl}account/login`, model).pipe(
      map((user: User) => {
        if (user) {
          this.setCurrentUser(user);
        }
        return user;
      })
    );
  }
   /**
    * Inscription
    * @param model 
    * 
    * @returns 
    */
  register(model: any){
    return this.http.post<User>(`${this.baseUrl}account/register`, model).pipe(
      map((user: User) => {
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  /**
   * Récupère le user courant
   * @param user 
   */
  setCurrentUser(user: User): void {
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  logout(): void {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }

  /**
   * Récupère le token
   * @returns 
   */
  getDecodedToken(token: string) {
    return JSON.parse(atob(token.split('.')[1]));
  }

}