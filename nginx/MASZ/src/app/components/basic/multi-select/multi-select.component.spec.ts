import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MemberSelectComponent } from './multi-select.component';

describe('MemberSelectComponent', () => {
  let component: MemberSelectComponent;
  let fixture: ComponentFixture<MemberSelectComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MemberSelectComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MemberSelectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
