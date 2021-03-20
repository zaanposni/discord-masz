import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TemplateSettings, TemplateViewPermissionOptions } from 'src/app/models/TemplateSettings';

@Component({
  selector: 'app-template-create-dialog',
  templateUrl: './template-create-dialog.component.html',
  styleUrls: ['./template-create-dialog.component.css']
})
export class TemplateCreateDialogComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public settings: TemplateSettings) { }

  templateViewPermissionOptions = TemplateViewPermissionOptions;

  ngOnInit(): void {    
  }

}
