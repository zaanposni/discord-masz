import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuildAppealsTableComponent } from './guild-appeals-table.component';

describe('GuildAppealsTableComponent', () => {
  let component: GuildAppealsTableComponent;
  let fixture: ComponentFixture<GuildAppealsTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GuildAppealsTableComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GuildAppealsTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
