import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuicksearchUsermapsResultComponent } from './quicksearch-usermaps-result.component';

describe('QuicksearchUsermapsResultComponent', () => {
  let component: QuicksearchUsermapsResultComponent;
  let fixture: ComponentFixture<QuicksearchUsermapsResultComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ QuicksearchUsermapsResultComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(QuicksearchUsermapsResultComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
