import { TestBed } from '@angular/core/testing';

import { CookieTrackerService } from './cookie-tracker.service';

describe('CookieTrackerService', () => {
  let service: CookieTrackerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CookieTrackerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
