import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppUser } from 'src/app/models/AppUser';
import { CaseTemplate } from 'src/app/models/CaseTemplate';
import { DiscordUser } from 'src/app/models/DiscordUser';
import { TemplateView } from 'src/app/models/TemplateView';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-profile-view',
  templateUrl: './profile-view.component.html',
  styleUrls: ['./profile-view.component.scss']
})
export class ProfileViewComponent implements OnInit {

  currentUser: AppUser;
  userId: string;
  user!: Promise<DiscordUser>;
  templates!: Promise<TemplateView[]>;

  constructor(private route: ActivatedRoute, private api: ApiService, private router: Router, private auth: AuthService) { }

  ngOnInit(): void {
    this.userId = this.route.snapshot.paramMap.get('userid');
    this.user = this.api.getSimpleData(`/discord/users/${this.userId}`).toPromise();
    this.user.then(() => {}, () => { this.router.navigate(['notfound']); });
    this.reloadTemplates(null);
    this.auth.getUserProfile().subscribe((data) => {
      this.currentUser = data;
    });
  }

  useTemplate(template: CaseTemplate) {
    let guilds: { [key: string]: string } = {};
    this.currentUser.modGuilds.forEach((element) => {
      guilds[element.id] = element.name;
    });
    this.currentUser.adminGuilds.forEach((element) => {
      guilds[element.id] = element.name;
    });
    if (Object.keys(guilds).length === 0) {
      return;  // if no mod anywhere
    }
    Swal.fire({
      title: 'Choose a guild',
      text: `Name: ${template.templateName}, Id: ${template.id}`,
      icon: 'question',
      input: 'select',
      inputOptions: guilds,
      confirmButtonText: 'Use',
      showCancelButton: true
    }).then((data) => {
      if (data.isConfirmed) {
        this.router.navigate(['guilds', data.value, 'cases', 'new'], { queryParams: { templateid: template.id }});
      }
    });
  }

  reloadTemplates(event: any) {
    this.templates = null;
    this.templates = this.api.getSimpleData(`/templatesview?userid=${this.userId}`).toPromise();
  }
}
