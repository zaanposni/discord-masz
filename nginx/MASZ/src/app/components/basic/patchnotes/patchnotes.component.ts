import { Component, OnInit } from '@angular/core';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { PatchNote } from 'src/app/models/PatchNote';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-patchnotes',
  templateUrl: './patchnotes.component.html',
  styleUrls: ['./patchnotes.component.css']
})
export class PatchnotesComponent implements OnInit {

  public patchnotes: ContentLoading<PatchNote[]> = { loading: true, content: [] };

  constructor(private api: ApiService) { }

  ngOnInit(): void {
    this.reload();
  }

  private reload() {
    this.patchnotes = { loading: true, content: [] };
    this.api.getSimpleData('/static/patchnotes.json', false).subscribe((data) => {
      this.patchnotes.content = data;
      this.patchnotes.loading = false;
    }, () => {
      this.patchnotes.loading = false;
    });
  }
}
