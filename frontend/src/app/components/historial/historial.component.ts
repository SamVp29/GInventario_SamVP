import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Observable } from 'rxjs';

import { TransaccionService } from '../../services/transaccion.service';
import { ProductoService } from '../../services/producto.service';
import { Transaccion } from '../../models/transaccion.model';
import { Producto } from '../../models/producto.model';

@Component({
  selector: 'app-historial',
  standalone: true,
  imports: [CommonModule, FormsModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './historial.component.html',
  styleUrls: ['./historial.component.css']
})
export class HistorialComponent implements OnInit {
  productosList: Producto[] = [];
  selectedProductoId: number = 0;
  selectedProducto?: Producto;

  historial$: Observable<Transaccion[]>;
  loading$: Observable<boolean>;

  // Filtros
  tipoTransaccionFiltro: string = '';
  fechaInicioFiltroStr: string = '';
  fechaFinFiltroStr: string = '';

  constructor(
    private transaccionService: TransaccionService,
    private productoService: ProductoService
  ) {
    this.historial$ = this.transaccionService.historial$;
    this.loading$ = this.transaccionService.loading$;
  }

  ngOnInit(): void {
    this.cargarProductos();
  }

  cargarProductos(): void {
    this.productoService.getProductosList().subscribe({
      next: (res) => {
        this.productosList = res.items;
        if (this.productosList.length > 0) {
          this.selectedProductoId = this.productosList[0].id;
          this.onProductoChange(this.selectedProductoId);
        }
      }
    });
  }

  onProductoChange(id: number): void {
    const pId = Number(id);
    this.selectedProductoId = pId;
    this.selectedProducto = this.productosList.find(p => p.id === pId);
    this.cargarHistorial();
  }

  cargarHistorial(): void {
    if (!this.selectedProductoId) return;

    const fInicio = this.fechaInicioFiltroStr ? new Date(this.fechaInicioFiltroStr).toISOString() : undefined;
    const fFin = this.fechaFinFiltroStr ? new Date(this.fechaFinFiltroStr).toISOString() : undefined;

    this.transaccionService.cargarHistorialPorProducto(
      this.selectedProductoId,
      fInicio,
      fFin,
      this.tipoTransaccionFiltro
    );
  }

  onFilter(): void {
    this.cargarHistorial();
  }

  limpiarFiltros(): void {
    this.tipoTransaccionFiltro = '';
    this.fechaInicioFiltroStr = '';
    this.fechaFinFiltroStr = '';
    this.cargarHistorial();
  }
}
