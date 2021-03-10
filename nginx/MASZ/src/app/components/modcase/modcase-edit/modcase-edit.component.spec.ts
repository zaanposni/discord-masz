import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModcaseEditComponent } from './modcase-edit.component';

describe('ModcaseEditComponent', () => {
  let component: ModcaseEditComponent;
  let fixture: ComponentFixture<ModcaseEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ModcaseEditComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ModcaseEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
