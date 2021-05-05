import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UsernoteEditDialogComponent } from './usernote-edit-dialog.component';

describe('UsernoteEditDialogComponent', () => {
  let component: UsernoteEditDialogComponent;
  let fixture: ComponentFixture<UsernoteEditDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UsernoteEditDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UsernoteEditDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
