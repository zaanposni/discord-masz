import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { IAppealStructure } from 'src/app/models/IAppealStructure';

@Component({
  selector: 'app-appeal-structure-edit-dialog',
  templateUrl: './appeal-structure-edit-dialog.component.html',
  styleUrls: ['./appeal-structure-edit-dialog.component.css']
})
export class AppealStructureEditDialogComponent implements OnInit {

  public form!: FormGroup;
  maxLength4096 = { length: 4096 };

  constructor(@Inject(MAT_DIALOG_DATA) public structure: IAppealStructure, private _formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.form = this._formBuilder.group({
      question: ['', [ Validators.required, Validators.maxLength(4096) ]]
    });

    this.form.setValue({
      question: this.structure.question
    });

    this.form.valueChanges.subscribe((data) => {
      this.structure.question = data.question;
    });
  }

  get newQuestion() { return this.form.get('question'); }
}
