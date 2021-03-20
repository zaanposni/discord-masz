import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MotdConfigComponent } from './motd-config.component';

describe('MotdConfigComponent', () => {
  let component: MotdConfigComponent;
  let fixture: ComponentFixture<MotdConfigComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MotdConfigComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MotdConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
