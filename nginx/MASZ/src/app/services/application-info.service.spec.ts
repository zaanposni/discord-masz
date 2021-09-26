import { TestBed } from '@angular/core/testing';

import { ApplicationInfoService } from './application-info.service';

describe('ApplicationInfoService', () => {
  let service: ApplicationInfoService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ApplicationInfoService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
