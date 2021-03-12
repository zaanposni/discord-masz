import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModcaseTableComponent } from './modcase-table.component';

describe('ModcaseTableComponent', () => {
  let component: ModcaseTableComponent;
  let fixture: ComponentFixture<ModcaseTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ModcaseTableComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ModcaseTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
