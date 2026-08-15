import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { Producto, CrearProductoDto, EditarProductoDto, PagedResult } from '../models/producto.model';

@Injectable({
  providedIn: 'root'
})
export class ProductoService {
  private apiUrl = environment.productosApiUrl;

  private currentBusqueda?: string;
  private currentCategoria?: string;
  private currentPage: number = 1;
  private currentPageSize: number = 8;
  private lastJsonString: string = '';

  private productosSubject = new BehaviorSubject<PagedResult<Producto>>({
    items: [],
    totalCount: 0,
    page: 1,
    pageSize: 8,
    totalPages: 1
  });
  public productos$: Observable<PagedResult<Producto>> = this.productosSubject.asObservable();

  private loadingSubject = new BehaviorSubject<boolean>(false);
  public loading$: Observable<boolean> = this.loadingSubject.asObservable();

  constructor(private http: HttpClient) {
    // Polling background silencioso con verificación de cambios reales
    setInterval(() => {
      this.refreshSilencioso();
    }, 10000);
  }

  cargarProductos(busqueda?: string, categoria?: string, page: number = 1, pageSize: number = 8): void {
    this.currentBusqueda = busqueda;
    this.currentCategoria = categoria;
    this.currentPage = page;
    this.currentPageSize = pageSize;

    this.loadingSubject.next(true);
    this.fetchProductos().subscribe({
      next: (res) => {
        this.lastJsonString = JSON.stringify(res);
        this.productosSubject.next(res);
        this.loadingSubject.next(false);
      },
      error: () => {
        this.loadingSubject.next(false);
      }
    });
  }

  refreshSilencioso(): void {
    this.fetchProductos().subscribe({
      next: (res) => {
        const newJsonString = JSON.stringify(res);
        // Únicamente notificar al DOM si los datos sufrieron un cambio real en la BD
        if (newJsonString !== this.lastJsonString) {
          this.lastJsonString = newJsonString;
          this.productosSubject.next(res);
        }
      }
    });
  }

  private fetchProductos(): Observable<PagedResult<Producto>> {
    let params = new HttpParams()
      .set('page', this.currentPage.toString())
      .set('pageSize', this.currentPageSize.toString());

    if (this.currentBusqueda) params = params.set('busqueda', this.currentBusqueda);
    if (this.currentCategoria) params = params.set('categoria', this.currentCategoria);

    return this.http.get<PagedResult<Producto>>(this.apiUrl, { params });
  }

  getProductosList(): Observable<PagedResult<Producto>> {
    return this.http.get<PagedResult<Producto>>(`${this.apiUrl}?page=1&pageSize=100`);
  }

  getProductoById(id: number): Observable<Producto> {
    return this.http.get<Producto>(`${this.apiUrl}/${id}`);
  }

  crearProducto(dto: CrearProductoDto): Observable<Producto> {
    return this.http.post<Producto>(this.apiUrl, dto).pipe(
      tap(() => this.cargarProductos(this.currentBusqueda, this.currentCategoria, this.currentPage, this.currentPageSize))
    );
  }

  editarProducto(id: number, dto: EditarProductoDto): Observable<Producto> {
    return this.http.put<Producto>(`${this.apiUrl}/${id}`, dto).pipe(
      tap(() => this.cargarProductos(this.currentBusqueda, this.currentCategoria, this.currentPage, this.currentPageSize))
    );
  }

  eliminarProducto(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      tap(() => this.cargarProductos(this.currentBusqueda, this.currentCategoria, this.currentPage, this.currentPageSize))
    );
  }
}
