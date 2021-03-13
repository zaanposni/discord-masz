import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModeventsTableComponent } from './modevents-table.component';

describe('ModeventsTableComponent', () => {
  let component: ModeventsTableComponent;
  let fixture: ComponentFixture<ModeventsTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ModeventsTableComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ModeventsTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
