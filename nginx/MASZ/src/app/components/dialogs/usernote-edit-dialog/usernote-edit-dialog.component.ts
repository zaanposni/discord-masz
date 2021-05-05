import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { UserNoteDto } from 'src/app/models/UserNoteDto';

@Component({
  selector: 'app-usernote-edit-dialog',
  templateUrl: './usernote-edit-dialog.component.html',
  styleUrls: ['./usernote-edit-dialog.component.css']
})
export class UsernoteEditDialogComponent implements OnInit {

  public initRows = 1;
  constructor(@Inject(MAT_DIALOG_DATA) public settings: UserNoteDto) { }

  ngOnInit(): void {
    this.initRows = Math.min(this.settings.description.split(/\r\n|\r|\n/).length, 15);
  }
}
