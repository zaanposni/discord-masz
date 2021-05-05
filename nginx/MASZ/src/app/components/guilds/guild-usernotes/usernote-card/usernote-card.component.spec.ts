import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UsernoteCardComponent } from './usernote-card.component';

describe('UsernoteCardComponent', () => {
  let component: UsernoteCardComponent;
  let fixture: ComponentFixture<UsernoteCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UsernoteCardComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UsernoteCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
