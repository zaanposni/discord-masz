import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { NewTokenDialog } from 'src/app/models/NewTokenDialog';

@Component({
  selector: 'app-new-token-dialog',
  templateUrl: './new-token-dialog.component.html',
  styleUrls: ['./new-token-dialog.component.css']
})
export class NewTokenDialogComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public token: NewTokenDialog) { }

  ngOnInit(): void {
  }

}
