import { Component, OnInit } from '@angular/core';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { Guideline } from 'src/app/models/Guideline';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-guidelines',
  templateUrl: './guidelines.component.html',
  styleUrls: ['./guidelines.component.css']
})
export class GuidelinesComponent implements OnInit {

  public guidelines: ContentLoading<Guideline[]> = { loading: true, content: [] };
  public terms: ContentLoading<Guideline[]> = { loading: true, content: [] };

  constructor(private api: ApiService) { }

  ngOnInit(): void {
    this.reload();
  }

  private reload() {
    this.guidelines = { loading: true, content: [] };
    this.terms = { loading: true, content: [] };
    this.api.getSimpleData('/static/guidelines.json', false).subscribe((data) => {
      this.guidelines.content = data;
      this.guidelines.loading = false;
    }, () => {
      this.guidelines.loading = false;
    });
    this.api.getSimpleData('/static/terms.json', false).subscribe((data) => {
      this.terms.content = data;
      this.terms.loading = false;
    }, () => {
      this.terms.loading = false;
    });
  }

}
