import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuildAppealViewComponent } from './guild-appeal-view.component';

describe('GuildAppealViewComponent', () => {
  let component: GuildAppealViewComponent;
  let fixture: ComponentFixture<GuildAppealViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GuildAppealViewComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GuildAppealViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
