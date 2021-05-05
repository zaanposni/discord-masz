import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UsermapCardComponent } from './usermap-card.component';

describe('UsermapCardComponent', () => {
  let component: UsermapCardComponent;
  let fixture: ComponentFixture<UsermapCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UsermapCardComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UsermapCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
