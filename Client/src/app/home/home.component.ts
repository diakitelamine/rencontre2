import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  registerMode = false;
  users: any;

  constructor(){}

  ngOnInit(): void {
  
  }

  //Enregistrement
  registerToggle(){
    this.registerMode = !this.registerMode;
  }

//Annulation d'enregistrement
cancelRegisterMode(event : boolean){
  this.registerMode = event;
}

}
