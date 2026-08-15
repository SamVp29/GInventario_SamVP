export interface Transaccion {
  id: number;
  fecha: string;
  tipoTransaccion: 'COMPRA' | 'VENTA';
  productoId: number;
  nombreProducto: string;
  stockProductoActual: number;
  cantidad: number;
  precioUnitario: number;
  precioTotal: number;
  detalle: string;
}

export interface CrearTransaccionDto {
  tipoTransaccion: 'COMPRA' | 'VENTA';
  productoId: number;
  cantidad: number;
  precioUnitario: number;
  detalle: string;
}

export interface EditarTransaccionDto {
  tipoTransaccion: 'COMPRA' | 'VENTA';
  cantidad: number;
  precioUnitario: number;
  detalle: string;
}
