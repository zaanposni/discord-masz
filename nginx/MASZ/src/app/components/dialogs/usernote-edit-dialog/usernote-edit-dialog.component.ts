import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UserNoteDto } from 'src/app/models/UserNoteDto';

@Component({
  selector: 'app-usernote-edit-dialog',
  templateUrl: './usernote-edit-dialog.component.html',
  styleUrls: ['./usernote-edit-dialog.component.css']
})
export class UsernoteEditDialogComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public settings: UserNoteDto) { }

  ngOnInit(): void {
  }

}
