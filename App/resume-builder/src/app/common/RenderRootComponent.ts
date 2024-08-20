import { Routes } from '@angular/router';
import { MATERIAL_SANITY_CHECKS } from '@angular/material/core';
import { render, RenderResult } from '@testing-library/angular';
import { APP_BASE_HREF, CommonModule } from '@angular/common';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA, Type } from '@angular/core';

interface TestRenderOptions<T> {
  imports?: unknown[];
  providers?: unknown[];
  declarations?: unknown[];
  componentProperties?: Partial<T>;
  componentInputs?: Partial<T>;
  routes?: Routes;
  importOverrides?: ImportOverride[];
}

interface ImportOverride {
  remove?: Type<any>;
  add?: Type<any>;
}

export async function renderRootComponent<T>(
  component: Type<T>,
  {
    imports = [],
    providers = [],
    componentProperties = {},
    componentInputs = {},
    declarations = [],
    routes = [],
    importOverrides = [],
  }: TestRenderOptions<T> = {},
): Promise<RenderResult<T, T>> {
  return await render(component, {
    excludeComponentDeclaration: false,
    declarations: [...declarations],
    imports: [
      RouterTestingModule.withRoutes(routes),
      CommonModule,
      HttpClientTestingModule,
      ReactiveFormsModule,
      ...imports,
    ],
    providers: [
      { provide: MATERIAL_SANITY_CHECKS, useValue: false },
      { provide: APP_BASE_HREF, useValue: '/' },
      ...providers,
    ],
    schemas: [NO_ERRORS_SCHEMA, CUSTOM_ELEMENTS_SCHEMA],
    componentProperties,
    componentInputs,
    configureTestBed: testBed => {
      testBed.overrideComponent(component, {
        remove: { imports: importOverrides.map(x => x.remove) },
        add: { imports: importOverrides.map(x => x.add) },
      });
    },
  });
}
