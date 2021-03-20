import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuicksearchModerationResultComponent } from './quicksearch-moderation-result.component';

describe('QuicksearchModerationResultComponent', () => {
  let component: QuicksearchModerationResultComponent;
  let fixture: ComponentFixture<QuicksearchModerationResultComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ QuicksearchModerationResultComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(QuicksearchModerationResultComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
