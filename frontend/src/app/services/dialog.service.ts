import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { ConfirmDialogComponent } from '../components/dialogs/confirm-dialog/confirm-dialog.component';
import { AlertDialogComponent } from '../components/dialogs/alert-dialog/alert-dialog.component';

@Injectable({
  providedIn: 'root'
})
export class DialogService {

  constructor(private readonly dialog: MatDialog) { }

  /**
   * Muestra un diálogo de confirmación moderno
   * @param title Título del diálogo
   * @param message Mensaje del diálogo
   * @param confirmText Texto del botón de confirmación (por defecto: 'Confirmar')
   * @param cancelText Texto del botón de cancelar (por defecto: 'Cancelar')
   * @returns Observable<boolean> - true si confirma, false si cancela
   */
  confirm(
    title: string, 
    message: string, 
    confirmText: string = 'Confirmar', 
    cancelText: string = 'Cancelar'
  ): Observable<boolean> {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      maxWidth: '90vw',
      data: {
        title,
        message,
        confirmText,
        cancelText
      },
      panelClass: 'custom-dialog-container',
      backdropClass: 'custom-dialog-backdrop'
    });

    return dialogRef.afterClosed();
  }

  /**
   * Muestra un diálogo de alerta moderno
   * @param title Título del diálogo
   * @param message Mensaje del diálogo
   * @param okText Texto del botón OK (por defecto: 'OK')
   * @returns Observable<void>
   */
  alert(
    title: string, 
    message: string, 
    okText: string = 'OK'
  ): Observable<void> {
    const dialogRef = this.dialog.open(AlertDialogComponent, {
      width: '400px',
      maxWidth: '90vw',
      data: {
        title,
        message,
        okText
      },
      panelClass: 'custom-dialog-container',
      backdropClass: 'custom-dialog-backdrop'
    });

    return dialogRef.afterClosed();
  }

  /**
   * Muestra un diálogo de éxito
   */
  success(message: string, title: string = '¡Éxito!'): Observable<void> {
    return this.alert(title, message, 'Genial');
  }

  /**
   * Muestra un diálogo de error
   */
  error(message: string, title: string = 'Error'): Observable<void> {
    return this.alert(title, message, 'Entendido');
  }

  /**
   * Muestra un diálogo de información
   */
  info(message: string, title: string = 'Información'): Observable<void> {
    return this.alert(title, message, 'OK');
  }
}
