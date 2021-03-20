import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardCaseListComponent } from './dashboard-case-list.component';

describe('DashboardCaseListComponent', () => {
  let component: DashboardCaseListComponent;
  let fixture: ComponentFixture<DashboardCaseListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DashboardCaseListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardCaseListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
