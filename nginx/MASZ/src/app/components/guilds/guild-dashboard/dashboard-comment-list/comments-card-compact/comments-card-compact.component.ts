import { Component, Input, OnInit } from '@angular/core';
import { ICommentListTableViewEntry } from 'src/app/models/ICommentListTableViewEntry';

@Component({
  selector: 'app-comments-card-compact',
  templateUrl: './comments-card-compact.component.html',
  styleUrls: ['./comments-card-compact.component.css']
})
export class CommentsCardCompactComponent implements OnInit {

  @Input() entry!: ICommentListTableViewEntry;
  constructor() { }

  ngOnInit(): void {
  }

}
