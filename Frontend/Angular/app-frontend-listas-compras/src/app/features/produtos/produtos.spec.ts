import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Produtos } from './produtos';

describe('Produtos', () => {
  let component: Produtos;
  let fixture: ComponentFixture<Produtos>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [Produtos]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Produtos);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
