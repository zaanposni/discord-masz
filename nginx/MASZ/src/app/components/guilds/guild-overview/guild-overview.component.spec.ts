import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuildOverviewComponent } from './guild-overview.component';

describe('GuildOverviewComponent', () => {
  let component: GuildOverviewComponent;
  let fixture: ComponentFixture<GuildOverviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GuildOverviewComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GuildOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
