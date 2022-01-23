import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuildMessagesComponent } from './guild-messages.component';

describe('GuildMessagesComponent', () => {
  let component: GuildMessagesComponent;
  let fixture: ComponentFixture<GuildMessagesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GuildMessagesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GuildMessagesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
