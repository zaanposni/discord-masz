import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuildAddComponent } from './guild-add.component';

describe('GuildAddComponent', () => {
  let component: GuildAddComponent;
  let fixture: ComponentFixture<GuildAddComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GuildAddComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GuildAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
