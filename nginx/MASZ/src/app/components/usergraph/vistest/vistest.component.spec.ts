import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VistestComponent } from './vistest.component';

describe('VistestComponent', () => {
  let component: VistestComponent;
  let fixture: ComponentFixture<VistestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ VistestComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(VistestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
