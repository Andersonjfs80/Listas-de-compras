import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListasComponent } from './listas';

describe('ListasComponent', () => {
  let component: ListasComponent;
  let fixture: ComponentFixture<ListasComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ListasComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ListasComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
