import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CaseDeleteDialogData } from 'src/app/models/CaseDeleteDialogData';

@Component({
  selector: 'app-case-delete-dialog',
  templateUrl: './case-delete-dialog.component.html',
  styleUrls: ['./case-delete-dialog.component.css']
})
export class CaseDeleteDialogComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public config: CaseDeleteDialogData) { }

  ngOnInit(): void {
  }

}
