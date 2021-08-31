import { TestBed } from '@angular/core/testing';

import { EnumManagerService } from './enum-manager.service';

describe('EnumManagerService', () => {
  let service: EnumManagerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EnumManagerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
