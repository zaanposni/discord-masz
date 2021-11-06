import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuditlogConfigComponent } from './auditlog-config.component';

describe('AuditlogConfigComponent', () => {
  let component: AuditlogConfigComponent;
  let fixture: ComponentFixture<AuditlogConfigComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AuditlogConfigComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AuditlogConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
