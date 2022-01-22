import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuildMessageCardComponent } from './guild-message-card.component';

describe('GuildMessageCardComponent', () => {
  let component: GuildMessageCardComponent;
  let fixture: ComponentFixture<GuildMessageCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GuildMessageCardComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GuildMessageCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
