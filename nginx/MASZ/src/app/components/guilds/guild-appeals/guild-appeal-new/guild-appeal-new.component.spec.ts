import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuildAppealNewComponent } from './guild-appeal-new.component';

describe('GuildAppealNewComponent', () => {
  let component: GuildAppealNewComponent;
  let fixture: ComponentFixture<GuildAppealNewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GuildAppealNewComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GuildAppealNewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
