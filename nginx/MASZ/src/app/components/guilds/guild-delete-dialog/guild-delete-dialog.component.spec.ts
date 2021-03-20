import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuildDeleteDialogComponent } from './guild-delete-dialog.component';

describe('GuildDeleteDialogComponent', () => {
  let component: GuildDeleteDialogComponent;
  let fixture: ComponentFixture<GuildDeleteDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GuildDeleteDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GuildDeleteDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
