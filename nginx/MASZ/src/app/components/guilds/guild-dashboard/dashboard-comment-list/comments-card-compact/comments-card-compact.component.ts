import { Component, Input, OnInit } from '@angular/core';
import { CommentListViewEntry } from 'src/app/models/CommentListViewEntry';

@Component({
  selector: 'app-comments-card-compact',
  templateUrl: './comments-card-compact.component.html',
  styleUrls: ['./comments-card-compact.component.css']
})
export class CommentsCardCompactComponent implements OnInit {

  @Input() entry!: CommentListViewEntry;
  constructor() { }

  ngOnInit(): void {
  }

}
