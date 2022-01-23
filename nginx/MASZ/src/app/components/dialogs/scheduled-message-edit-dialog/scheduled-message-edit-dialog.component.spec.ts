import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScheduledMessageEditDialogComponent } from './scheduled-message-edit-dialog.component';

describe('ScheduledMessageEditDialogComponent', () => {
  let component: ScheduledMessageEditDialogComponent;
  let fixture: ComponentFixture<ScheduledMessageEditDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ScheduledMessageEditDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ScheduledMessageEditDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
