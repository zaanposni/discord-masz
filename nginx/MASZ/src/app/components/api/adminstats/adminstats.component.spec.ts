import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminstatsComponent } from './adminstats.component';

describe('AdminstatsComponent', () => {
  let component: AdminstatsComponent;
  let fixture: ComponentFixture<AdminstatsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AdminstatsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AdminstatsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
