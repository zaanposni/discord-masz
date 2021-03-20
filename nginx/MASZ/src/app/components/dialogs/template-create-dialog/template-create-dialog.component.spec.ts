import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TemplateCreateDialogComponent } from './template-create-dialog.component';

describe('TemplateCreateDialogComponent', () => {
  let component: TemplateCreateDialogComponent;
  let fixture: ComponentFixture<TemplateCreateDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TemplateCreateDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TemplateCreateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
