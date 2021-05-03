import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuildUsernotesComponent } from './guild-usernotes.component';

describe('GuildUsernotesComponent', () => {
  let component: GuildUsernotesComponent;
  let fixture: ComponentFixture<GuildUsernotesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GuildUsernotesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GuildUsernotesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
