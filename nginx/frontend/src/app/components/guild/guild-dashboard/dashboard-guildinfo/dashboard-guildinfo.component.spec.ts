import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardGuildinfoComponent } from './dashboard-guildinfo.component';

describe('DashboardGuildinfoComponent', () => {
  let component: DashboardGuildinfoComponent;
  let fixture: ComponentFixture<DashboardGuildinfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DashboardGuildinfoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardGuildinfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
