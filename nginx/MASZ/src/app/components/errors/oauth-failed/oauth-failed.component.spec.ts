import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OauthFailedComponent } from './oauth-failed.component';

describe('OauthFailedComponent', () => {
  let component: OauthFailedComponent;
  let fixture: ComponentFixture<OauthFailedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OauthFailedComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OauthFailedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
