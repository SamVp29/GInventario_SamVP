import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Observable } from 'rxjs';

import { TransaccionService } from '../../services/transaccion.service';
import { ProductoService } from '../../services/producto.service';
import { Transaccion, CrearTransaccionDto, EditarTransaccionDto } from '../../models/transaccion.model';
import { Producto, PagedResult } from '../../models/producto.model';

interface ToastMsg {
  type: 'success' | 'error' | 'warning';
  title: string;
  text: string;
}

@Component({
  selector: 'app-transacciones',
  standalone: true,
  imports: [CommonModule, FormsModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './transacciones.component.html',
  styleUrls: ['./transacciones.component.css']
})
export class TransaccionesComponent implements OnInit {
  transacciones$: Observable<PagedResult<Transaccion>>;
  loading$: Observable<boolean>;

  productosList: Producto[] = [];

  // Filtros
  productoIdFiltro?: number;
  tipoTransaccionFiltro: string = '';
  fechaInicioFiltroStr: string = '';
  fechaFinFiltroStr: string = '';
  page: number = 1;
  pageSize: number = 8;

  // Modal
  transaccionDialog: boolean = false;
  isEditMode: boolean = false;
  selectedId: number = 0;
  selectedProductoActual?: Producto;

  // Confirm Dialog
  confirmDialogVisible: boolean = false;
  selectedTxParaBorrar?: Transaccion;

  // Toast
  toastMessage?: ToastMsg;

  transaccionForm: CrearTransaccionDto = {
    tipoTransaccion: 'COMPRA',
    productoId: 0,
    cantidad: 1,
    precioUnitario: 0,
    detalle: ''
  };

  constructor(
    private transaccionService: TransaccionService,
    private productoService: ProductoService,
    private cdr: ChangeDetectorRef
  ) {
    this.transacciones$ = this.transaccionService.transacciones$;
    this.loading$ = this.transaccionService.loading$;
  }

  ngOnInit(): void {
    this.cargarProductosSelector();
    this.cargarTransacciones();
  }

  trackById(index: number, item: Transaccion): number {
    return item.id;
  }

  cargarProductosSelector(): void {
    this.productoService.getProductosList().subscribe({
      next: (res) => {
        this.productosList = res.items;
      }
    });
  }

  cargarTransacciones(): void {
    const fInicio = this.fechaInicioFiltroStr ? new Date(this.fechaInicioFiltroStr).toISOString() : undefined;
    const fFin = this.fechaFinFiltroStr ? new Date(this.fechaFinFiltroStr).toISOString() : undefined;

    this.transaccionService.cargarTransacciones(
      this.productoIdFiltro,
      this.tipoTransaccionFiltro,
      fInicio,
      fFin,
      this.page,
      this.pageSize
    );
  }

  calcularTotalCompras(items: Transaccion[]): number {
    return items
      .filter(t => t.tipoTransaccion === 'COMPRA')
      .reduce((acc, item) => acc + item.precioTotal, 0);
  }

  calcularTotalVentas(items: Transaccion[]): number {
    return items
      .filter(t => t.tipoTransaccion === 'VENTA')
      .reduce((acc, item) => acc + item.precioTotal, 0);
  }

  getTotalPages(totalRecords: number): number {
    return Math.ceil(totalRecords / this.pageSize) || 1;
  }

  previousPage(totalRecords: number): void {
    if (this.page > 1) {
      this.page--;
      this.cargarTransacciones();
    }
  }

  nextPage(totalRecords: number): void {
    if (this.page < this.getTotalPages(totalRecords)) {
      this.page++;
      this.cargarTransacciones();
    }
  }

  onFilter(): void {
    this.page = 1;
    this.cargarTransacciones();
  }

  limpiarFiltros(): void {
    this.productoIdFiltro = undefined;
    this.tipoTransaccionFiltro = '';
    this.fechaInicioFiltroStr = '';
    this.fechaFinFiltroStr = '';
    this.page = 1;
    this.cargarTransacciones();
  }

  openNew(): void {
    this.isEditMode = false;
    this.selectedProductoActual = undefined;
    this.transaccionForm = {
      tipoTransaccion: 'COMPRA',
      productoId: this.productosList.length > 0 ? this.productosList[0].id : 0,
      cantidad: 1,
      precioUnitario: this.productosList.length > 0 ? this.productosList[0].precio : 0,
      detalle: 'Movimiento de inventario'
    };

    if (this.transaccionForm.productoId > 0) {
      this.onProductoChange(this.transaccionForm.productoId);
    }

    this.transaccionDialog = true;
  }

  onProductoChange(productoId: number): void {
    const id = Number(productoId);
    this.selectedProductoActual = this.productosList.find(p => p.id === id);
    if (this.selectedProductoActual && !this.isEditMode) {
      this.transaccionForm.precioUnitario = this.selectedProductoActual.precio;
    }
  }

  editTransaccion(tx: Transaccion): void {
    this.isEditMode = true;
    this.selectedId = tx.id;
    this.transaccionForm = {
      tipoTransaccion: tx.tipoTransaccion,
      productoId: tx.productoId,
      cantidad: tx.cantidad,
      precioUnitario: tx.precioUnitario,
      detalle: tx.detalle
    };
    this.onProductoChange(tx.productoId);
    this.transaccionDialog = true;
  }

  deleteTransaccion(tx: Transaccion): void {
    this.transaccionService.eliminarTransaccion(tx.id).subscribe({
      next: () => {
        this.showToast('success', 'Reversión Exitosa', `Se anuló la transacción del producto "${tx.nombreProducto}" y se reajustó el stock.`);
        this.cargarTransacciones();
        this.cargarProductosSelector();
      },
      error: (err) => {
        const errorMsg = this.extractErrorMessage(err, 'No se pudo anular la transacción.');
        this.showToast('error', 'Error al Anular', errorMsg);
      }
    });
  }

  guardarTransaccion(): void {
    if (!this.transaccionForm.productoId) {
      this.showToast('warning', 'Validación', 'Debe seleccionar un producto.');
      return;
    }

    if (this.transaccionForm.cantidad <= 0) {
      this.showToast('warning', 'Validación', 'La cantidad debe ser mayor a 0.');
      return;
    }

    // Regla de Negocio Compleja (Validación de Stock en Cliente)
    if (this.transaccionForm.tipoTransaccion === 'VENTA' && this.selectedProductoActual) {
      if (this.transaccionForm.cantidad > this.selectedProductoActual.stock) {
        this.showToast(
          'error',
          'Stock Insuficiente',
          `No puedes vender ${this.transaccionForm.cantidad} unidades. El stock disponible actual es de ${this.selectedProductoActual.stock} unidades.`
        );
        return;
      }
    }

    if (this.isEditMode) {
      const editDto: EditarTransaccionDto = {
        tipoTransaccion: this.transaccionForm.tipoTransaccion,
        cantidad: this.transaccionForm.cantidad,
        precioUnitario: this.transaccionForm.precioUnitario,
        detalle: this.transaccionForm.detalle
      };

      this.transaccionService.editarTransaccion(this.selectedId, editDto).subscribe({
        next: () => {
          this.showToast('success', 'Éxito', 'Transacción modificada y stock reajustado correctamente.');
          this.transaccionDialog = false;
          this.cargarTransacciones();
          this.cargarProductosSelector();
        },
        error: (err) => {
          const msg = this.extractErrorMessage(err, 'Error al actualizar transacción.');
          this.showToast('error', 'Error de Servidor', msg);
        }
      });
    } else {
      this.transaccionService.crearTransaccion(this.transaccionForm).subscribe({
        next: () => {
          this.showToast('success', 'Éxito', 'Transacción registrada y stock actualizado en Productos.API.');
          this.transaccionDialog = false;
          this.cargarTransacciones();
          this.cargarProductosSelector();
        },
        error: (err) => {
          const msg = this.extractErrorMessage(err, 'Fallo en la comunicación síncrona o stock insuficiente.');
          this.showToast('error', 'Error', msg);
        }
      });
    }
  }

  hideDialog(): void {
    this.transaccionDialog = false;
    this.cdr.markForCheck();
  }

  private extractErrorMessage(err: any, fallback: string): string {
    if (err?.error?.mensaje) return err.error.mensaje;
    if (typeof err?.error === 'string') return err.error;
    if (err?.message) return err.message;
    return fallback;
  }

  private showToast(type: 'success' | 'error' | 'warning', title: string, text: string): void {
    this.toastMessage = { type, title, text };
    this.cdr.markForCheck();
    setTimeout(() => {
      this.toastMessage = undefined;
      this.cdr.markForCheck();
    }, 5000);
  }
}
