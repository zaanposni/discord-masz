import { Component, Input, OnInit, Output } from '@angular/core';
import { EventEmitter } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { TemplateView } from 'src/app/models/TemplateView';
import { ApiService } from 'src/app/services/api.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-template-card',
  templateUrl: './template-card.component.html',
  styleUrls: ['./template-card.component.scss']
})
export class TemplateCardComponent implements OnInit {

  @Input() template: TemplateView;
  @Input() userId: string;
  @Input() showCreator: boolean = false;

  @Output() reload = new EventEmitter();
  @Output() useTemplate = new EventEmitter();

  constructor(private api: ApiService, private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  deleteTemplate() {
    Swal.fire({
      title: 'Delete this template?',
      text: `Name: ${this.template.caseTemplate.templateName}, Id: ${this.template.caseTemplate.id}`,
      icon: 'warning',
      confirmButtonText: 'Delete',
      showCancelButton: true
    }).then((data) => {
      if (data.isConfirmed) {
        this.api.deleteData(`/templates/${this.template.caseTemplate.id}`).subscribe(() => {
          this.toastr.success('Template deleted.');
          this.reload.emit('reload');
        }, () => { this.toastr.error('Cannot delete case.', 'Something went wrong.');})
      }
    });
  }
}
