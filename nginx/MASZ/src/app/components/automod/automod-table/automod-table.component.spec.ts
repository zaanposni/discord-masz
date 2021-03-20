import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AutomodTableComponent } from './automod-table.component';

describe('AutomodTableComponent', () => {
  let component: AutomodTableComponent;
  let fixture: ComponentFixture<AutomodTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AutomodTableComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AutomodTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
