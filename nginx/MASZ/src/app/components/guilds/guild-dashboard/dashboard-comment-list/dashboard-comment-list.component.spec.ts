import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardCommentListComponent } from './dashboard-comment-list.component';

describe('DashboardCommentListComponent', () => {
  let component: DashboardCommentListComponent;
  let fixture: ComponentFixture<DashboardCommentListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DashboardCommentListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardCommentListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
