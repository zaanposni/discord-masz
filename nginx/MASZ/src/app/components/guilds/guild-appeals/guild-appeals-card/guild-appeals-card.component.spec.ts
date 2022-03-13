import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuildAppealsCardComponent } from './guild-appeals-card.component';

describe('GuildAppealsCardComponent', () => {
  let component: GuildAppealsCardComponent;
  let fixture: ComponentFixture<GuildAppealsCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GuildAppealsCardComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GuildAppealsCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
