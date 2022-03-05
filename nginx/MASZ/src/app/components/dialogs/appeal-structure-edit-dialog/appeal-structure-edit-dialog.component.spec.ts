import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppealStructureEditDialogComponent } from './appeal-structure-edit-dialog.component';

describe('AppealStructureEditDialogComponent', () => {
  let component: AppealStructureEditDialogComponent;
  let fixture: ComponentFixture<AppealStructureEditDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AppealStructureEditDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AppealStructureEditDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
