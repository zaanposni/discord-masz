import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminlistComponent } from './adminlist.component';

describe('AdminlistComponent', () => {
  let component: AdminlistComponent;
  let fixture: ComponentFixture<AdminlistComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AdminlistComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AdminlistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
