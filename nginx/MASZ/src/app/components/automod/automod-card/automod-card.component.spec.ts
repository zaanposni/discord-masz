import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AutomodCardComponent } from './automod-card.component';

describe('AutomodCardComponent', () => {
  let component: AutomodCardComponent;
  let fixture: ComponentFixture<AutomodCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AutomodCardComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AutomodCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
