import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuicksearchCaseResultComponent } from './quicksearch-case-result.component';

describe('QuicksearchCaseResultComponent', () => {
  let component: QuicksearchCaseResultComponent;
  let fixture: ComponentFixture<QuicksearchCaseResultComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ QuicksearchCaseResultComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(QuicksearchCaseResultComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
