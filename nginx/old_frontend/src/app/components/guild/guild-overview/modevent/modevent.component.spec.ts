import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModeventComponent } from './modevent.component';

describe('ModeventComponent', () => {
  let component: ModeventComponent;
  let fixture: ComponentFixture<ModeventComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ModeventComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ModeventComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
