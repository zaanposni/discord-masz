import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardQuicksearchComponent } from './dashboard-quicksearch.component';

describe('DashboardQuicksearchComponent', () => {
  let component: DashboardQuicksearchComponent;
  let fixture: ComponentFixture<DashboardQuicksearchComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DashboardQuicksearchComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardQuicksearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
