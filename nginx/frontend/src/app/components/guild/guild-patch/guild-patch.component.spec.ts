import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuildPatchComponent } from './guild-patch.component';

describe('GuildPatchComponent', () => {
  let component: GuildPatchComponent;
  let fixture: ComponentFixture<GuildPatchComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GuildPatchComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GuildPatchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
