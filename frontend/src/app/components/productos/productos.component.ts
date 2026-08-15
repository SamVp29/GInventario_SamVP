import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Observable } from 'rxjs';

import { ProductoService } from '../../services/producto.service';
import { Producto, CrearProductoDto, PagedResult } from '../../models/producto.model';

interface ToastMsg {
  type: 'success' | 'error' | 'warning';
  title: string;
  text: string;
}

@Component({
  selector: 'app-productos',
  standalone: true,
  imports: [CommonModule, FormsModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './productos.component.html',
  styleUrls: ['./productos.component.css']
})
export class ProductosComponent implements OnInit {
  productos$: Observable<PagedResult<Producto>>;
  loading$: Observable<boolean>;

  // Filtros dinámicos
  busqueda: string = '';
  categoria: string = '';
  page: number = 1;
  pageSize: number = 8;

  // Modal Crear/Editar
  productoDialog: boolean = false;
  isEditMode: boolean = false;
  selectedId: number = 0;

  // Modal Confirmación Borrar
  confirmDialogVisible: boolean = false;
  selectedProductoParaBorrar?: Producto;

  // Toast Notificaciones
  toastMessage?: ToastMsg;

  productoForm: CrearProductoDto = {
    nombre: '',
    descripcion: '',
    categoria: '',
    imagen: '',
    precio: 0,
    stock: 0
  };

  constructor(
    private productoService: ProductoService,
    private cdr: ChangeDetectorRef
  ) {
    this.productos$ = this.productoService.productos$;
    this.loading$ = this.productoService.loading$;
  }

  ngOnInit(): void {
    this.cargarProductos();
  }

  trackById(index: number, item: Producto): number {
    return item.id;
  }

  cargarProductos(): void {
    this.productoService.cargarProductos(this.busqueda, this.categoria, this.page, this.pageSize);
  }

  calcularValorTotal(items: Producto[]): number {
    return items.reduce((acc, item) => acc + (item.precio * item.stock), 0);
  }

  productosStockBajoCount(items: Producto[]): number {
    return items.filter(p => p.stock <= 5).length;
  }

  getTotalPages(totalRecords: number): number {
    return Math.ceil(totalRecords / this.pageSize) || 1;
  }

  previousPage(totalRecords: number): void {
    if (this.page > 1) {
      this.page--;
      this.cargarProductos();
    }
  }

  nextPage(totalRecords: number): void {
    if (this.page < this.getTotalPages(totalRecords)) {
      this.page++;
      this.cargarProductos();
    }
  }

  onSearch(): void {
    this.page = 1;
    this.cargarProductos();
  }

  limpiarFiltros(): void {
    this.busqueda = '';
    this.categoria = '';
    this.page = 1;
    this.cargarProductos();
  }

  openNew(): void {
    this.isEditMode = false;
    this.productoForm = {
      nombre: '',
      descripcion: '',
      categoria: '',
      imagen: 'https://images.unsplash.com/photo-1526738549149-8e07eca6c147?w=500',
      precio: 0,
      stock: 0
    };
    this.productoDialog = true;
  }

  editProducto(prod: Producto): void {
    this.isEditMode = true;
    this.selectedId = prod.id;
    this.productoForm = {
      nombre: prod.nombre,
      descripcion: prod.descripcion,
      categoria: prod.categoria,
      imagen: prod.imagen,
      precio: prod.precio,
      stock: prod.stock
    };
    this.productoDialog = true;
  }

  deleteProducto(prod: Producto): void {
    this.productoService.eliminarProducto(prod.id).subscribe({
      next: () => {
        this.showToast('success', 'Producto Eliminado', `El producto "${prod.nombre}" se eliminó correctamente.`);
      },
      error: (err) => {
        const errorMsg = this.extractErrorMessage(err, 'No se pudo eliminar el producto.');
        this.showToast('error', 'Acción Bloqueada', errorMsg);
      }
    });
  }

  guardarProducto(): void {
    if (!this.productoForm.nombre.trim() || !this.productoForm.categoria.trim()) {
      this.showToast('warning', 'Validación', 'Nombre y Categoría son campos obligatorios.');
      return;
    }

    if (this.productoForm.precio <= 0) {
      this.showToast('warning', 'Validación', 'El precio debe ser mayor a cero.');
      return;
    }

    if (this.isEditMode) {
      this.productoService.editarProducto(this.selectedId, this.productoForm).subscribe({
        next: () => {
          this.showToast('success', 'Éxito', 'Producto actualizado correctamente.');
          this.productoDialog = false;
        },
        error: (err) => {
          const errorMsg = this.extractErrorMessage(err, 'No se pudo actualizar el producto.');
          this.showToast('error', 'Error al Guardar', errorMsg);
        }
      });
    } else {
      this.productoService.crearProducto(this.productoForm).subscribe({
        next: () => {
          this.showToast('success', 'Éxito', 'Producto creado correctamente.');
          this.productoDialog = false;
        },
        error: (err) => {
          const errorMsg = this.extractErrorMessage(err, 'No se pudo crear el producto.');
          this.showToast('error', 'Error al Crear', errorMsg);
        }
      });
    }
  }

  hideDialog(): void {
    this.productoDialog = false;
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
