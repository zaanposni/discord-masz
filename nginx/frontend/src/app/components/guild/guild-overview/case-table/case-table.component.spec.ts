import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CaseTableComponent } from './case-table.component';

describe('CaseTableComponent', () => {
  let component: CaseTableComponent;
  let fixture: ComponentFixture<CaseTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CaseTableComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CaseTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
