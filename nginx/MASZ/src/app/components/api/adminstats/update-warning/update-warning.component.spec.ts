import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateWarningComponent } from './update-warning.component';

describe('UpdateWarningComponent', () => {
  let component: UpdateWarningComponent;
  let fixture: ComponentFixture<UpdateWarningComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UpdateWarningComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UpdateWarningComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
