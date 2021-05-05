import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UsermapEditDialogComponent } from './usermap-edit-dialog.component';

describe('UsermapEditDialogComponent', () => {
  let component: UsermapEditDialogComponent;
  let fixture: ComponentFixture<UsermapEditDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UsermapEditDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UsermapEditDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
