import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuildEditComponent } from './guild-edit.component';

describe('GuildEditComponent', () => {
  let component: GuildEditComponent;
  let fixture: ComponentFixture<GuildEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GuildEditComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GuildEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
