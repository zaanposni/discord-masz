import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuildAppealsComponent } from './guild-appeals.component';

describe('GuildAppealsComponent', () => {
  let component: GuildAppealsComponent;
  let fixture: ComponentFixture<GuildAppealsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GuildAppealsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GuildAppealsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
