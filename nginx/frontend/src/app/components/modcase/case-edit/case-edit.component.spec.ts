import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CaseEditComponent } from './case-edit.component';

describe('CaseEditComponent', () => {
  let component: CaseEditComponent;
  let fixture: ComponentFixture<CaseEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CaseEditComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CaseEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
