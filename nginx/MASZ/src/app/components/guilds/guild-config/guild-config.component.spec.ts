import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuildConfigComponent } from './guild-config.component';

describe('GuildConfigComponent', () => {
  let component: GuildConfigComponent;
  let fixture: ComponentFixture<GuildConfigComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GuildConfigComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GuildConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
