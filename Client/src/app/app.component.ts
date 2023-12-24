import { Component, OnInit } from '@angular/core';
import { AccountService } from './_services/account.service';
import { User } from './_models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  users: any;
  constructor(private accountService : AccountService) {}

  ngOnInit() {
    this.setCurrentUser();
  }
  // La recuperation de tous les utilisateurs 
 
  setCurrentUser(){
    const userString = localStorage.getItem('user');
    if(!userString) return;

    const user: User = JSON.parse(userString);
    this.accountService.setCurrentUser(user);
  }
}