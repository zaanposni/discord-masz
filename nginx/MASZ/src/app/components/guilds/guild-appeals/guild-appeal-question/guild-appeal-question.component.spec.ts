import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuildAppealQuestionComponent } from './guild-appeal-question.component';

describe('GuildAppealQuestionComponent', () => {
  let component: GuildAppealQuestionComponent;
  let fixture: ComponentFixture<GuildAppealQuestionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GuildAppealQuestionComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GuildAppealQuestionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
