import { Component, Input, OnInit } from '@angular/core';
import { UserNoteView } from 'src/app/models/UserNoteView';

@Component({
  selector: 'app-usernote-card',
  templateUrl: './usernote-card.component.html',
  styleUrls: ['./usernote-card.component.css']
})
export class UsernoteCardComponent implements OnInit {

  @Input() userNote!: UserNoteView;
  constructor() { }

  ngOnInit(): void {
  }

}
