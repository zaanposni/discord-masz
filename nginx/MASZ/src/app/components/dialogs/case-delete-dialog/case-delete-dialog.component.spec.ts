import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CaseDeleteDialogComponent } from './case-delete-dialog.component';

describe('CaseDeleteDialogComponent', () => {
  let component: CaseDeleteDialogComponent;
  let fixture: ComponentFixture<CaseDeleteDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CaseDeleteDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CaseDeleteDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
