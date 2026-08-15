export interface Producto {
  id: number;
  nombre: string;
  descripcion: string;
  categoria: string;
  imagen: string;
  precio: number;
  stock: number;
}

export interface CrearProductoDto {
  nombre: string;
  descripcion: string;
  categoria: string;
  imagen: string;
  precio: number;
  stock: number;
}

export interface EditarProductoDto {
  nombre: string;
  descripcion: string;
  categoria: string;
  imagen: string;
  precio: number;
  stock: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
