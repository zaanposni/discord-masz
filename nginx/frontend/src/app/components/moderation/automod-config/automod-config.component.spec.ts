import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AutomodConfigComponent } from './automod-config.component';

describe('AutomodConfigComponent', () => {
  let component: AutomodConfigComponent;
  let fixture: ComponentFixture<AutomodConfigComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AutomodConfigComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AutomodConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
