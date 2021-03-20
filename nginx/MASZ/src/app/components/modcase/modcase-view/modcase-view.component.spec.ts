import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModcaseViewComponent } from './modcase-view.component';

describe('ModcaseViewComponent', () => {
  let component: ModcaseViewComponent;
  let fixture: ComponentFixture<ModcaseViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ModcaseViewComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ModcaseViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
