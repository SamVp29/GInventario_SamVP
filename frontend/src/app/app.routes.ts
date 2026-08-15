import { Routes } from '@angular/router';
import { ProductosComponent } from './components/productos/productos.component';
import { TransaccionesComponent } from './components/transacciones/transacciones.component';
import { HistorialComponent } from './components/historial/historial.component';

export const routes: Routes = [
  { path: '', redirectTo: 'productos', pathMatch: 'full' },
  { path: 'productos', component: ProductosComponent },
  { path: 'transacciones', component: TransaccionesComponent },
  { path: 'historial', component: HistorialComponent },
  { path: '**', redirectTo: 'productos' }
];
