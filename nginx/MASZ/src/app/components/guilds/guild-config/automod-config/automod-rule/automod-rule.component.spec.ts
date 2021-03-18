import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AutomodRuleComponent } from './automod-rule.component';

describe('AutomodRuleComponent', () => {
  let component: AutomodRuleComponent;
  let fixture: ComponentFixture<AutomodRuleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AutomodRuleComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AutomodRuleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
