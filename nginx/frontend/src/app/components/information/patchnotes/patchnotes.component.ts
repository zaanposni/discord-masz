import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { PatchNote } from 'src/app/models/PatchNote';
import { AppVersion } from 'src/app/models/AppVersion';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-patchnotes',
  templateUrl: './patchnotes.component.html',
  styleUrls: ['./patchnotes.component.scss']
})
export class PatchnotesComponent implements OnInit {

  patchNotes!: Observable<PatchNote[]>;

  constructor(private router: ActivatedRoute, private api: ApiService, private auth: AuthService) { }

  ngOnInit(): void {
    this.patchNotes = this.api.getSimpleData('/static/patchnotes.json?v=1.7.2', false);
  }
}
