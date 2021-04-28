import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserscanComponent } from './userscan.component';

describe('UserscanComponent', () => {
  let component: UserscanComponent;
  let fixture: ComponentFixture<UserscanComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserscanComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UserscanComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
