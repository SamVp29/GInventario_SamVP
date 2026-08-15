import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { Transaccion, CrearTransaccionDto, EditarTransaccionDto } from '../models/transaccion.model';
import { PagedResult } from '../models/producto.model';
import { ProductoService } from './producto.service';

@Injectable({
  providedIn: 'root'
})
export class TransaccionService {
  private apiUrl = environment.transaccionesApiUrl;

  private currentProductoId?: number;
  private currentTipoTransaccion?: string;
  private currentFechaInicio?: string;
  private currentFechaFin?: string;
  private currentPage: number = 1;
  private currentPageSize: number = 8;
  private lastJsonString: string = '';

  private transaccionesSubject = new BehaviorSubject<PagedResult<Transaccion>>({
    items: [],
    totalCount: 0,
    page: 1,
    pageSize: 8,
    totalPages: 1
  });
  public transacciones$: Observable<PagedResult<Transaccion>> = this.transaccionesSubject.asObservable();

  private historialSubject = new BehaviorSubject<Transaccion[]>([]);
  public historial$: Observable<Transaccion[]> = this.historialSubject.asObservable();

  private loadingSubject = new BehaviorSubject<boolean>(false);
  public loading$: Observable<boolean> = this.loadingSubject.asObservable();

  constructor(
    private http: HttpClient,
    private productoService: ProductoService
  ) {
    setInterval(() => {
      this.refreshSilencioso();
    }, 10000);
  }

  cargarTransacciones(
    productoId?: number,
    tipoTransaccion?: string,
    fechaInicio?: string,
    fechaFin?: string,
    page: number = 1,
    pageSize: number = 8
  ): void {
    this.currentProductoId = productoId;
    this.currentTipoTransaccion = tipoTransaccion;
    this.currentFechaInicio = fechaInicio;
    this.currentFechaFin = fechaFin;
    this.currentPage = page;
    this.currentPageSize = pageSize;

    this.loadingSubject.next(true);
    this.fetchTransacciones().subscribe({
      next: (res) => {
        this.lastJsonString = JSON.stringify(res);
        this.transaccionesSubject.next(res);
        this.loadingSubject.next(false);
      },
      error: () => {
        this.loadingSubject.next(false);
      }
    });
  }

  refreshSilencioso(): void {
    this.fetchTransacciones().subscribe({
      next: (res) => {
        const newJsonString = JSON.stringify(res);
        if (newJsonString !== this.lastJsonString) {
          this.lastJsonString = newJsonString;
          this.transaccionesSubject.next(res);
        }
      }
    });
  }

  private fetchTransacciones(): Observable<PagedResult<Transaccion>> {
    let params = new HttpParams()
      .set('page', this.currentPage.toString())
      .set('pageSize', this.currentPageSize.toString());

    if (this.currentProductoId) params = params.set('productoId', this.currentProductoId.toString());
    if (this.currentTipoTransaccion) params = params.set('tipoTransaccion', this.currentTipoTransaccion);
    if (this.currentFechaInicio) params = params.set('fechaInicio', this.currentFechaInicio);
    if (this.currentFechaFin) params = params.set('fechaFin', this.currentFechaFin);

    return this.http.get<PagedResult<Transaccion>>(this.apiUrl, { params });
  }

  cargarHistorialPorProducto(
    productoId: number,
    fechaInicio?: string,
    fechaFin?: string,
    tipoTransaccion?: string
  ): void {
    this.loadingSubject.next(true);

    let params = new HttpParams();
    if (fechaInicio) params = params.set('fechaInicio', fechaInicio);
    if (fechaFin) params = params.set('fechaFin', fechaFin);
    if (tipoTransaccion) params = params.set('tipoTransaccion', tipoTransaccion);

    this.http.get<Transaccion[]>(`${this.apiUrl}/historial/producto/${productoId}`, { params }).subscribe({
      next: (res) => {
        this.historialSubject.next(res);
        this.loadingSubject.next(false);
      },
      error: () => {
        this.loadingSubject.next(false);
      }
    });
  }

  crearTransaccion(dto: CrearTransaccionDto): Observable<Transaccion> {
    return this.http.post<Transaccion>(this.apiUrl, dto).pipe(
      tap(() => {
        this.refreshSilencioso();
        this.productoService.refreshSilencioso();
      })
    );
  }

  editarTransaccion(id: number, dto: EditarTransaccionDto): Observable<Transaccion> {
    return this.http.put<Transaccion>(`${this.apiUrl}/${id}`, dto).pipe(
      tap(() => {
        this.refreshSilencioso();
        this.productoService.refreshSilencioso();
      })
    );
  }

  eliminarTransaccion(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      tap(() => {
        this.refreshSilencioso();
        this.productoService.refreshSilencioso();
      })
    );
  }
}
