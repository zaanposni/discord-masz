import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardGuildStatsComponent } from './dashboard-guild-stats.component';

describe('DashboardGuildStatsComponent', () => {
  let component: DashboardGuildStatsComponent;
  let fixture: ComponentFixture<DashboardGuildStatsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DashboardGuildStatsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardGuildStatsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
