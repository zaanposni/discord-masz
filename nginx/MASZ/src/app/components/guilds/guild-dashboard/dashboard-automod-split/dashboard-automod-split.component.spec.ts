import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardAutomodSplitComponent } from './dashboard-automod-split.component';

describe('DashboardAutomodSplitComponent', () => {
  let component: DashboardAutomodSplitComponent;
  let fixture: ComponentFixture<DashboardAutomodSplitComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DashboardAutomodSplitComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardAutomodSplitComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
