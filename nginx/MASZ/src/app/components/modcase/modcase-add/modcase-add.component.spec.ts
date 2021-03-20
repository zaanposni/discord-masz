import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModcaseAddComponent } from './modcase-add.component';

describe('ModcaseAddComponent', () => {
  let component: ModcaseAddComponent;
  let fixture: ComponentFixture<ModcaseAddComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ModcaseAddComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ModcaseAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
