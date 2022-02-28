import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuildAppealConfigComponent } from './guild-appeal-config.component';

describe('GuildAppealConfigComponent', () => {
  let component: GuildAppealConfigComponent;
  let fixture: ComponentFixture<GuildAppealConfigComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GuildAppealConfigComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GuildAppealConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
