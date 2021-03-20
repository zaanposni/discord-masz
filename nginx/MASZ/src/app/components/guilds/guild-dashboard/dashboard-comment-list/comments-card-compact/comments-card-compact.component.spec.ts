import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CommentsCardCompactComponent } from './comments-card-compact.component';

describe('CommentsCardCompactComponent', () => {
  let component: CommentsCardCompactComponent;
  let fixture: ComponentFixture<CommentsCardCompactComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CommentsCardCompactComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CommentsCardCompactComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
