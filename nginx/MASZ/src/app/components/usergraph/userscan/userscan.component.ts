import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { AutoModerationEvent } from 'src/app/models/AutoModerationEvent';
import { AutoModerationType } from 'src/app/models/AutoModerationType';
import { DiscordUser } from 'src/app/models/DiscordUser';
import { Guild } from 'src/app/models/Guild';
import { ModCase } from 'src/app/models/ModCase';
import { UserInvite } from 'src/app/models/UserInvite';
import { UserNetwork } from 'src/app/models/UserNetwork';
import { ApiService } from 'src/app/services/api.service';
import { Network, DataSet, Node, Edge, Data, IdType } from 'vis';

@Component({
  selector: 'app-userscan',
  templateUrl: './userscan.component.html',
  styleUrls: ['./userscan.component.css']
})
export class UserscanComponent implements OnInit {

  @ViewChild('network') el!: ElementRef;
  timeout: any = null;
  loading: boolean = false;
  public search!: string;
  private networkInstance!: Network;
  public showUsage: boolean = true;
  private options = {
    height: '100%',
    width: '100%',
    nodes: {
      shape: 'dot',
      size: 30,
      borderWidth: 3,
      shadow: true,
      font: {
        color: 'white'
      }
    },
    edges: {
      color: { inherit: 'to' },
      smooth: {
        enabled: true,
        type: 'continuous',
        roundness: 0.1
      }
    },
    interaction: {
      hover: true
    }
  };
  private data: {'nodes': Node[], 'edges': Edge[]} = { 'nodes': [], 'edges': [] };

  constructor(private api: ApiService, private toastr: ToastrService, private route: ActivatedRoute) { }

  ngOnInit(): void {
  }

  onSearch(event: any) {
    clearTimeout(this.timeout);
    var $this = this;
    this.timeout = setTimeout(function () {
      if (event.keyCode != 13) {
        $this.executeSearch();
      }
    }, 100);
  }

  executeSearch() {
    if (this.search?.trim()) {
      this.showUsage = false;
      this.loading = true;
      this.reset();
      this.loadDataForUserId(this.search?.trim()).subscribe((data: UserNetwork) => {
        this.calculateNewNetwork(data, this.search?.trim());
        this.loading = false;
      }, () => {
        this.reset();
        this.loading = false;
        this.toastr.error("Failed to load scan information.");
      });
    } else {
      this.reset();
    }
  }

  loadDataForUserId(userId: string): Observable<UserNetwork> {
    return this.api.getSimpleData(`/network/${userId}`);
  }

  ngAfterViewInit() {
     const container = this.el.nativeElement;
     this.networkInstance = new Network(container, this.data, this.options);
     this.networkInstance.on("doubleClick", this.onDoubleClick.bind(this));
     if ('userid' in this.route.snapshot.queryParams) {
      this.search = this.route.snapshot.queryParams['userid'];
      this.executeSearch();
    }
  }

  onDoubleClick(params: any) {
    if (params?.nodes?.length === 1) {
      let node = this.data.nodes.find(x => x.id === params?.nodes[0]) as any;
      if (node?.searchFor) {
        this.search = node?.searchFor;
        this.executeSearch();
        return;
      }
      if (node?.redirectTo) {
        window.open(node.redirectTo, '_blank');
      }
    }
  }

  reset() {
    this.data.nodes = [];
    this.data.edges = [];
    this.redraw();
  }

  calculateNewNetwork(network: UserNetwork, userId: string) {
    let baseNode = this.addNewNode(this.newUserNode, [network?.user, userId, 50, 'basics']) as Node;
    for (let guild of network.guilds) {
      let guildNode = this.addNewNode(this.newGuildNode, [guild, guild.id, 40, `${userId}/${guild.id}`]) as Node;
      this.addNewEdge(baseNode, guildNode);
      for (let invite of network.invitedBy) {
        if (invite.userInvite.guildId !== guild.id) continue;
        let inviteNode = this.addNewNode(this.newInviteNode, [invite.userInvite]) as Node;
        this.addNewEdge(guildNode, inviteNode, `Joined at: ${new Date(invite.userInvite.joinedAt).toLocaleString()}`, true, 'from');
        let invitedUserNode = this.addNewNode(this.newUserNode, [invite?.invitedBy, invite?.userInvite?.inviteIssuerId]) as Node;
        this.addNewEdge(inviteNode, invitedUserNode, `Created at: ${new Date(invite.userInvite.inviteCreatedAt).toLocaleString()}`, false, 'from');
      }
      for (let invite of network.invited) {
        if (invite.userInvite.guildId !== guild.id) continue;
        let inviteNode = this.addNewNode(this.newInviteNode, [invite.userInvite]) as Node;
        this.addNewEdge(guildNode, inviteNode, `Created at: ${new Date(invite.userInvite.inviteCreatedAt).toLocaleString()}`, false, 'to');
        let invitedUserNode = this.addNewNode(this.newUserNode, [invite?.invitedUser, invite?.userInvite?.joinedUserId]) as Node;
        this.addNewEdge(inviteNode, invitedUserNode, `Joined at: ${new Date(invite.userInvite.joinedAt).toLocaleString()}`, true, 'to');
      }
      for (let modCase of network.modCases) {
        if (modCase.guildId !== guild.id) continue;
        let caseBaseNode = this.addNewNode(this.newBasicCasesNode, [userId, guild.id]) as Node;
        this.addNewEdge(guildNode, caseBaseNode);
        let caseNode = this.addNewNode(this.newCaseNode, [modCase]) as Node;
        this.addNewEdge(caseBaseNode, caseNode, `Created at: ${new Date(modCase.createdAt).toLocaleString()}`);
      }
      for (let modEvent of network.modEvents) {
        if (modEvent.guildId !== guild.id) continue;
        let eventBaseNode = this.addNewNode(this.newBasicAutomodsNode, [userId, guild.id]) as Node;
        this.addNewEdge(guildNode, eventBaseNode);
        let eventNode = this.addNewNode(this.newEventNode, [modEvent, 5]) as Node;
        this.addNewEdge(eventBaseNode, eventNode, `Occured at: ${new Date(modEvent.createdAt).toLocaleString()}`);
      }
    }   
    this.redraw();
  }

  redraw() {
    this.networkInstance.destroy
    this.networkInstance.setData(this.data);
    this.networkInstance.redraw();
  }

  addNewNode(func: CallableFunction, params: any[]): Node|Node[] {
    let newNode = func(...params);
    if (Array.isArray(newNode)) {
      newNode.forEach(element => {
        if (this.data.nodes.filter(x => x.id === element?.id)?.length === 0) {
          this.data.nodes.push(element);
        }
      });
    } else {
      if (this.data.nodes.filter(x => x.id === newNode?.id)?.length === 0) {
        this.data.nodes.push(newNode);
      }

    }
    return newNode;
  }

  addNewEdge(from: Node, to: Node, title: string = '', addWithRoundness: boolean = false, arrow: 'to'|'from'|'no' = 'no'): Edge {
    let newEdge = {id: `${from.id}/node/${to.id}`, from: from.id, to: to.id, title: title.trim() === '' ? undefined : title} as any;
    if (arrow === 'to') {
      newEdge['arrows'] = { middle: { scaleFactor: 0.5 }, to: true };
    }
    if (arrow === 'from') {
      newEdge['arrows'] = { middle: { scaleFactor: 0.5 }, from: true };
    }
    let existingEdges = this.data.edges.filter(x => x.id === newEdge?.id);
    if (existingEdges.length === 0) {
      this.data.edges.push(newEdge);
    } else if(addWithRoundness) {
      let roundness = 0;
      for (let edge of existingEdges) {
        let s = edge?.smooth as any;
        if (s && 'roundness' in s) {
          if (s['roundness'] > roundness) roundness = s['roundness'];
        }
      }
      roundness += 0.2;
      newEdge['id'] += `/${roundness}`;
      newEdge['smooth'] = {type: 'curvedCW', roundness: roundness };
      this.data.edges.push(newEdge);
    }
    return newEdge;
  }

  newUserNode(user: DiscordUser, backupUserId: string = '', size: number = 30, group: string = 'otherusers'): Node {
    return {
      id: user?.id ?? backupUserId,
      shape: 'circularImage',
      group: group,
      image: user?.imageUrl ?? '/assets/img/default_profile.png',
      label: user != null ? `${user.username}#${user.discriminator}` : backupUserId,
      title: user?.id ?? backupUserId,
      size: size,
      searchFor: user?.id ?? backupUserId
    } as Node
  }
  
  newGuildNode(guild: Guild, guildId: string, size: number = 30, idPrefix: string = ''): Node {
    return {
      id: idPrefix + guild?.id ?? guildId,
      shape: 'circularImage',
      group: 'basics',
      image: guild != null ? `https://cdn.discordapp.com/icons/${guild.id}/${guild.icon}.png` : '/assets/img/default_profile.png',
      label: guild != null ? guild.name : guildId,
      title: guild?.id ?? guildId,
      size: size
    }
  }

  newCaseNode(modCase: ModCase, size: number = 20): Node {
    let punishmentString = '';
    if (modCase.punishedUntil != null) {
      punishmentString = `, Until: ${new Date(modCase.punishedUntil).toLocaleString()}`;
    }
    return {
      id: `${modCase.guildId}/case/${modCase.caseId}`,
      label: `Case #${modCase.caseId}\n${modCase.title.substr(0, 50)}`,
      title: `Punishment: ${modCase.punishment}${punishmentString}${modCase.description.substr(0, 200)}`,
      group: `${modCase.guildId}/cases`,
      size: size,
      redirectTo: `/guilds/${modCase.guildId}/cases/${modCase.caseId}`
    } as Node
  }

  newEventNode(modEvent: AutoModerationEvent, size: number = 20): Node {
    return {
      id: `${modEvent.guildId}/automod/${modEvent.id}`,
      group: `${modEvent.guildId}/automods`,
      title: `${AutoModerationType[modEvent.autoModerationType] ?? modEvent.autoModerationType} ${modEvent.messageContent}`,
      size: size
    }
  }

  newInviteNode(invite: UserInvite): Node {
    return {
      id: `${invite.guildId}/${invite.usedInvite}/${new Date(invite.inviteCreatedAt).getTime()}`,
      label: invite.usedInvite.substr(invite.usedInvite.lastIndexOf("/") + 1),
      group: `${invite.guildId}/invites`,
      shape: 'diamond',
      title: invite?.usedInvite,
      size: 15
    }
  }

  newBasicCasesNode(userId: string, guildId: string): Node {
    return {
      id: `${userId}/${guildId}/cases`,
      label: 'Cases',
      group: `basics/sub`,
      shape: 'triangle',
      size: 15
    }
  }

  newBasicAutomodsNode(userId: string, guildId: string): Node {
    return {
      id: `${userId}/${guildId}/automods`,
      label: 'Automoderations',
      group: `basics/sub`,
      shape: 'triangle',
      size: 15
    }
  }
}
