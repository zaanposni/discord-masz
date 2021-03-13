import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ModcaseCardComponent } from './modcase-card.component';

describe('ModcaseCardComponent', () => {
  let component: ModcaseCardComponent;
  let fixture: ComponentFixture<ModcaseCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ModcaseCardComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ModcaseCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
