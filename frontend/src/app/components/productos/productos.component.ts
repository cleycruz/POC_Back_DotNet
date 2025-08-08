import { Component, OnInit } from '@angular/core';
import { CarritoService } from '../../services/carrito.service';
import { DialogService } from '../../services/dialog.service';
import { ProductoDto, CrearProductoDto } from '../../models/carrito.models';

@Component({
  selector: 'app-productos',
  templateUrl: './productos.component.html',
  styleUrls: ['./productos.component.css']
})
export class ProductosComponent implements OnInit {
  productos: ProductoDto[] = [];
  loading = false;
  showForm = false;
  editingProduct: ProductoDto | null = null;
  
  newProduct: CrearProductoDto = {
    nombre: '',
    descripcion: '',
    precio: 0,
    stock: 0,
    categoria: ''
  };

  constructor(
    private readonly carritoService: CarritoService,
    private readonly dialogService: DialogService
  ) { }

  ngOnInit(): void {
    this.loadProductos();
  }

  loadProductos(): void {
    this.loading = true;
    this.carritoService.getProductos().subscribe({
      next: (productos: ProductoDto[]) => {
        this.productos = productos;
        this.loading = false;
      },
      error: (error: any) => {
        console.error('Error al cargar productos:', error);
        this.loading = false;
      }
    });
  }

  addToCart(producto: ProductoDto): void {
    const usuarioId = this.carritoService.getCurrentUserId();
    this.carritoService.addToCarrito(usuarioId, {
      productoId: producto.id,
      cantidad: 1
    }).subscribe({
      next: () => {
        this.dialogService.success('Producto agregado al carrito exitosamente');
      },
      error: (error: any) => {
        console.error('Error al agregar al carrito:', error);
        this.dialogService.error('No se pudo agregar el producto al carrito. Por favor, inténtalo nuevamente.');
      }
    });
  }

  createProduct(): void {
    this.carritoService.createProducto(this.newProduct).subscribe({
      next: () => {
        this.loadProductos();
        this.resetForm();
        this.dialogService.success('Producto creado exitosamente');
      },
      error: (error: any) => {
        console.error('Error al crear producto:', error);
        this.dialogService.error('No se pudo crear el producto. Por favor, verifica los datos e inténtalo nuevamente.');
      }
    });
  }

  editProduct(producto: ProductoDto): void {
    this.editingProduct = producto;
    this.newProduct = { 
      nombre: producto.nombre,
      descripcion: producto.descripcion,
      precio: producto.precio,
      stock: producto.stock,
      categoria: producto.categoria || ''
    };
    this.showForm = true;
  }

  updateProduct(): void {
    if (this.editingProduct) {
      this.carritoService.updateProducto(this.editingProduct.id, this.newProduct).subscribe({
        next: () => {
          this.loadProductos();
          this.resetForm();
          this.dialogService.success('Producto actualizado exitosamente');
        },
        error: (error: any) => {
          console.error('Error al actualizar producto:', error);
          this.dialogService.error('No se pudo actualizar el producto. Por favor, verifica los datos e inténtalo nuevamente.');
        }
      });
    }
  }

  deleteProduct(id: number): void {
    // Buscar el producto para mostrar su nombre en el diálogo
    const producto = this.productos.find(p => p.id === id);
    const nombreProducto = producto ? producto.nombre : 'este producto';
    
    this.dialogService.confirm(
      'Eliminar producto',
      `¿Estás seguro de que quieres eliminar "${nombreProducto}"? Esta acción no se puede deshacer.`,
      'Eliminar',
      'Cancelar'
    ).subscribe(confirmed => {
      if (confirmed) {
        this.carritoService.deleteProducto(id).subscribe({
          next: () => {
            this.loadProductos();
            this.dialogService.success('Producto eliminado exitosamente');
          },
          error: (error: any) => {
            console.error('Error al eliminar producto:', error);
            this.dialogService.error('No se pudo eliminar el producto. Por favor, inténtalo nuevamente.');
          }
        });
      }
    });
  }

  resetForm(): void {
    this.showForm = false;
    this.editingProduct = null;
    this.newProduct = {
      nombre: '',
      descripcion: '',
      precio: 0,
      stock: 0,
      categoria: ''
    };
  }

  scrollToForm(): void {
    const formElement = document.querySelector('.form-section');
    if (formElement) {
      formElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }

  cancelEdit(): void {
    this.resetForm();
  }

  private isValidProduct(): boolean {
    return !!(this.newProduct.nombre && this.newProduct.descripcion && 
             this.newProduct.precio > 0 && this.newProduct.stock >= 0 && 
             this.newProduct.categoria);
  }

  private showSuccessMessage(message: string): void {
    // Implementar sistema de notificaciones
    console.log('✅ ' + message);
  }

  private showErrorMessage(message: string): void {
    // Implementar sistema de notificaciones
    console.error('❌ ' + message);
  }

  onSubmit(): void {
    if (this.editingProduct) {
      this.updateProduct();
    } else {
      this.createProduct();
    }
  }
}
