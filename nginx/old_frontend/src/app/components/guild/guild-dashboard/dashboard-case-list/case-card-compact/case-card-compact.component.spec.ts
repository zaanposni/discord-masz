import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CaseCardCompactComponent } from './case-card-compact.component';

describe('CaseCardCompactComponent', () => {
  let component: CaseCardCompactComponent;
  let fixture: ComponentFixture<CaseCardCompactComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CaseCardCompactComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CaseCardCompactComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
