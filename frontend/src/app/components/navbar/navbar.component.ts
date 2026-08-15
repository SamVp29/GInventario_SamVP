import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <header class="app-header">
      <div class="header-container">
        <div class="brand">
          <div class="brand-logo">
            <span class="brand-initials">SV</span>
          </div>
          <div class="brand-text">
            <span class="brand-title">GInventario</span>
            <span class="brand-subtitle">SamVPDev</span>
          </div>
        </div>

        <nav class="nav-menu">
          <a routerLink="/productos" routerLinkActive="active" class="nav-item">
            <i class="pi pi-box"></i>
            <span>Catálogo Productos</span>
          </a>
          <a routerLink="/transacciones" routerLinkActive="active" class="nav-item">
            <i class="pi pi-arrow-right-arrow-left"></i>
            <span>Movimientos Stock</span>
          </a>
          <a routerLink="/historial" routerLinkActive="active" class="nav-item">
            <i class="pi pi-history"></i>
            <span>Historial Producto</span>
          </a>
        </nav>
      </div>
    </header>
  `,
  styles: [`
    .app-header {
      background: #0f172a;
      color: white;
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
      position: sticky;
      top: 0;
      z-index: 1000;
    }
    .header-container {
      max-width: 1350px;
      margin: 0 auto;
      padding: 0.85rem 1.5rem;
      display: flex;
      align-items: center;
      justify-content: space-between;
    }
    .brand {
      display: flex;
      align-items: center;
      gap: 0.85rem;
    }
    .brand-logo {
      width: 44px;
      height: 44px;
      background: #2563eb;
      border-radius: 10px;
      display: flex;
      align-items: center;
      justify-content: center;
    }
    .brand-initials {
      font-weight: 800;
      font-size: 1.15rem;
      color: white;
      letter-spacing: -0.5px;
    }
    .brand-title {
      font-weight: 800;
      font-size: 1.25rem;
      display: block;
      letter-spacing: -0.5px;
      color: white;
    }
    .brand-subtitle {
      font-size: 0.75rem;
      color: #94a3b8;
      text-transform: uppercase;
      letter-spacing: 1px;
      font-weight: 600;
    }
    .nav-menu {
      display: flex;
      gap: 0.75rem;
      background: rgba(255, 255, 255, 0.05);
      padding: 0.35rem;
      border-radius: 12px;
      border: 1px solid rgba(255, 255, 255, 0.1);
    }
    .nav-item {
      color: #cbd5e1;
      text-decoration: none;
      font-weight: 600;
      font-size: 0.9rem;
      padding: 0.6rem 1.1rem;
      border-radius: 8px;
      display: flex;
      align-items: center;
      gap: 0.6rem;
      transition: all 0.15s ease;
    }
    .nav-item:hover {
      color: white;
      background: rgba(255, 255, 255, 0.1);
    }
    .nav-item.active {
      color: white;
      background: #2563eb;
      box-shadow: none;
    }
  `]
})
export class NavbarComponent { }

