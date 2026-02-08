import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Listas } from './listas';

describe('Listas', () => {
  let component: Listas;
  let fixture: ComponentFixture<Listas>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [Listas]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Listas);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
