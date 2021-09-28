import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { CommentListViewEntry } from 'src/app/models/CommentListViewEntry';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-dashboard-comment-list',
  templateUrl: './dashboard-comment-list.component.html',
  styleUrls: ['./dashboard-comment-list.component.css']
})
export class DashboardCommentListComponent implements OnInit {

  public guildId!: string;
  public comments: ContentLoading<CommentListViewEntry[]> = { loading: true, content: [] };
  constructor(private api: ApiService, private toastr: ToastrService, private route: ActivatedRoute, private translator: TranslateService) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid') as string;
    this.reload();
  }

  private reload() {
    this.comments = { loading: true, content: [] };
    this.api.getSimpleData(`/guilds/${this.guildId}/dashboard/latestcomments`).subscribe((data) => {
      this.comments.content = data.slice(0, 5);
      this.comments.loading = false;
    }, error => {
      console.error(error);
      this.comments.loading = false;
      this.toastr.error(this.translator.instant('DashboardComments.FailedToLoad'));
    });
  }

}
