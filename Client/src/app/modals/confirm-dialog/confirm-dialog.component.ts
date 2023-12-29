import { Component, OnInit } from '@angular/core';
import { BsModalService } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.css']
})
export class ConfirmDialogComponent implements OnInit {
  title = '';
  message = '';
  btnOkText = '';
  btnCancelText = '';
  result: boolean = false;

  constructor(private modalService : BsModalService) { }




  ngOnInit(): void {
    
  }

  confirm(){
    this.result = true;
    this.modalService.hide();
  }

  decline(){
    this.result = false;
    this.modalService.hide();
  }

}
