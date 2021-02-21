import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardMotdComponent } from './dashboard-motd.component';

describe('DashboardMotdComponent', () => {
  let component: DashboardMotdComponent;
  let fixture: ComponentFixture<DashboardMotdComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DashboardMotdComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardMotdComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
