import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuildUsermappingComponent } from './guild-usermapping.component';

describe('GuildUsermappingComponent', () => {
  let component: GuildUsermappingComponent;
  let fixture: ComponentFixture<GuildUsermappingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GuildUsermappingComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GuildUsermappingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
