import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuditlogConfigRuleComponent } from './auditlog-config-rule.component';

describe('AuditlogConfigRuleComponent', () => {
  let component: AuditlogConfigRuleComponent;
  let fixture: ComponentFixture<AuditlogConfigRuleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AuditlogConfigRuleComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AuditlogConfigRuleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
