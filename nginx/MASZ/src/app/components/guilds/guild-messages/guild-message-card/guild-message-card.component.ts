import { Component, Input, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ScheduledMessageEditDialogComponent } from 'src/app/components/dialogs/scheduled-message-edit-dialog/scheduled-message-edit-dialog.component';
import { APIEnumTypes } from 'src/app/models/APIEmumTypes';
import { IScheduledMessage } from 'src/app/models/IScheduledMessage';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';
import { EnumManagerService } from 'src/app/services/enum-manager.service';

@Component({
  selector: 'app-guild-message-card',
  templateUrl: './guild-message-card.component.html',
  styleUrls: ['./guild-message-card.component.css']
})
export class GuildMessageCardComponent implements OnInit {

  public guildId!: string;
  public isAdminOrHigher: boolean = false;

  public status?: string;
  public failureReason?: string;

  @Input() message!: IScheduledMessage;

  constructor(private auth: AuthService, private enumManager: EnumManagerService, private toastr: ToastrService, private route: ActivatedRoute, private api: ApiService, private dialog: MatDialog) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid') as string;

    this.auth.isAdminInGuild(this.guildId).subscribe(data => {
      this.isAdminOrHigher = data;
    });

    this.enumManager.getEnum(APIEnumTypes.SCHEDULEMESSAGESTATUS).subscribe(data => {
      this.status = data?.find(x => x.key === this.message.status)?.value;
    });
    this.enumManager.getEnum(APIEnumTypes.SCHEDULEMESSAGEFAILUREREASON).subscribe(data => {
      this.failureReason = data?.find(x => x.key === this.message.failureReason)?.value;
    });
  }

  editMessage() {
    let messageDto: IScheduledMessage = {
      id: this.message.id,
      content: this.message.content,
      scheduledFor: this.message.scheduledFor,
      name: this.message.name,
      channelId: this.message.channelId
    } as IScheduledMessage;
    const editDialogRef = this.dialog.open(ScheduledMessageEditDialogComponent, {
      data: messageDto,
      minWidth: '800px'
    });
    editDialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        let body = {
          name: messageDto.name,
          content: messageDto.content,
          channelId: messageDto.channelId,
          scheduledFor: messageDto.scheduledFor?.toISOString()
        }

        this.api.putSimpleData(`/guilds/${this.guildId}/scheduledmessages/${this.message.id}`, body, undefined, true, true).subscribe((data) => {
          this.message = data;
        }, error => {
          console.error(error);
        });
      }
    });
  }

}
