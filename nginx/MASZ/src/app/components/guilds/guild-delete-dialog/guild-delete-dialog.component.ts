import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Guild } from 'src/app/models/Guild';
import { GuildDeleteDialogData } from 'src/app/models/GuildDeleteDialogData';

@Component({
  selector: 'app-guild-delete-dialog',
  templateUrl: './guild-delete-dialog.component.html',
  styleUrls: ['./guild-delete-dialog.component.css']
})
export class GuildDeleteDialogComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public guildToDelete: GuildDeleteDialogData) { }

  ngOnInit(): void {
  }
}
