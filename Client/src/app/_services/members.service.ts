import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Member } from '../_models/member';
import { Observable, map, of } from 'rxjs';
import { PaginatedResult, Pagination } from '../_models/Pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { User } from '../_models/user';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';
import { environment } from 'src/environments/environment.development';


@Injectable({
  providedIn: 'root'
})
export class MembersService {
  private baseUrl =  environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map();
  userParams: UserParams | undefined;
  user: User | undefined;

  constructor(private http: HttpClient, private accountService : AccountService) { 
    this.accountService.currentUser$.pipe().subscribe( {
      next: user => {
        if (user){
          this.user = user;
          this.userParams = new UserParams(user);
        }
      }
    })
  }

  getUserParams() {
    return this.userParams;
  }

  setUserParams(params: UserParams) {
    this.userParams = params;
  }

  resetUserParams(){
    if (this.user){
      this.userParams = new UserParams(this.user);
      return this.userParams;
    }
    return;
  }
  //Recuperation des membres
  getMembers(UserParams: UserParams) {
    const response = this.memberCache.get(Object.values(UserParams).join('-'));
    if (response) {
      return of(response);
    }
    let params = getPaginationHeaders( UserParams.pageNumber, UserParams.pageSize);
        params = params.append('minAge', UserParams.minAge);
        params = params.append('maxAge', UserParams.maxAge);
        params = params.append('gender', UserParams.gender);
        params = params.append('orderBy', UserParams.orderBy);
    
    return  getPaginatedResult<Member[]>(this.baseUrl + 'users', params, this.http).pipe(
      map(response => {
        this.memberCache.set(Object.values(UserParams).join('-'), response);
        return response;
      })
    )   
  }
  
  //Recuperation d'un membre
  getMember(username: string): Observable<Member> {
     const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])
      .find((member: Member) => member.userName === username);
      if (member) {
        return of(member);
      }
    return this.http.get<Member>(`${this.baseUrl}users/${username}`);
  }

  //Ajout d'un membre
  addMember(member: Member): Observable<any> {
    return this.http.post(`${this.baseUrl}users`, member);
  }

  //Mise à jour d'un membre
  updateMember(member: Member): Observable<any> {
    return this.http.put(`${this.baseUrl}users`, member).pipe(
      map(() => {
        //On met à jour le tableau des membres
        const index = this.members.indexOf(member);
        //On met à jour le tableau des membres
        this.members[index] = {...this.members[index], ...member};
      }
    ));
  }
  
  //Suppression d'un membre
  deleteMember(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}users/${id}`);
  }

  /**
   * Permet de définir une photo comme photo principale
   * @param photoId 
   * @returns 
   */
 setMainPhoto(photoId: number): Observable<any> {
    return this.http.put(`${this.baseUrl}users/set-main-photo/${photoId}`, {});
  }

  /**
   * Supprression d'une photo
   * @param photoId
   * @returns
   */
  deletePhoto(photoId: number) {
    return this.http.delete(`${this.baseUrl}users/delete-photo/${photoId}`);
  }

  addLike(username: string) {
    return this.http.post(`${this.baseUrl}likes/${username}`, {});
  }

  //Recuperation des likes
  getLikes(predicate: string, pageNumber: number, pageSize: number) {
    
    //On recupere les parametres de la pagination
    let params = getPaginationHeaders(pageNumber, pageSize);
    //On ajoute le parametre predicate
    params = params.append('predicate', predicate);
    //On retourne le resultat de la pagination
    return  getPaginatedResult<Member[]>(this.baseUrl + 'likes', params, this.http);
  }

  //Resultat de la pagination
  

  loadings(member : Member){
    this.accountService.currentUser$.subscribe(response =>{
       response?.username,
       response?.photoUrl
       this.resetUserParams.apply(HttpClient);
       this.accountService.setCurrentUser.length
       this.addMember
    })
  }

  //Recuperation de la liste des membres
  memberList(){
     this.accountService.currentUser$.subscribe(member =>{
       this.getMember; 
       
     })
  }


  
} 

