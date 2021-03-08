import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EpiclistComponent } from './epiclist.component';

describe('EpiclistComponent', () => {
  let component: EpiclistComponent;
  let fixture: ComponentFixture<EpiclistComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EpiclistComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EpiclistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
