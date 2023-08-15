import {Routes} from "@angular/router";
import {MATERIAL_SANITY_CHECKS} from '@angular/material/core';
import { NO_ERRORS_SCHEMA, Type} from "@angular/core";
import {render, RenderResult} from "@testing-library/angular";
import {APP_BASE_HREF, CommonModule} from "@angular/common";
import {RouterTestingModule} from "@angular/router/testing";
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {ReactiveFormsModule} from "@angular/forms";

interface TestRenderOptions<T> {
  imports?: unknown[];
  providers?: unknown[];
  declarations?: unknown[];
  componentProperties?: Partial<T>;
  routes?: Routes;
}

export async function renderRootComponent<T>(
  component: Type<T>,
  {
    imports = [],
    providers = [],
    componentProperties = {},
    declarations = [],
    routes = []
  }: TestRenderOptions<T> = {}
): Promise<RenderResult<T,T>> {
  return await render(component, {
    excludeComponentDeclaration: false,
    declarations: [...declarations],
    imports: [RouterTestingModule.withRoutes(routes),CommonModule,HttpClientTestingModule,ReactiveFormsModule,...imports],
    providers: [{provide: MATERIAL_SANITY_CHECKS, useValue: false},{provide: APP_BASE_HREF, useValue: '/'},...providers],
    schemas: [NO_ERRORS_SCHEMA],
    componentProperties
  });
}
