import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModcaseCardCompactComponent } from './modcase-card-compact.component';

describe('ModcaseCardCompactComponent', () => {
  let component: ModcaseCardCompactComponent;
  let fixture: ComponentFixture<ModcaseCardCompactComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ModcaseCardCompactComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ModcaseCardCompactComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
