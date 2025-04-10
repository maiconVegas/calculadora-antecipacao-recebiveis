import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'dialog-content-erros',
  templateUrl: 'dialog-content-erros.component.html',
  imports: [MatButtonModule, MatDialogModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  standalone: true,

})
export class DialogContentErros {
  readonly dialog = inject(MatDialog);

  openDialog(mensagem?: string) {
    this.dialog.open(DialogContentErrosDialog, {
      data: { erros: mensagem }
    });
  }
}

@Component({
  selector: 'dialog-content-erros-dialog',
  templateUrl: 'dialog-content-erros.component-dialog.html',
  imports: [MatDialogModule, MatButtonModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  standalone: true,

})
export class DialogContentErrosDialog {
  constructor(@Inject(MAT_DIALOG_DATA) public data: { erros: string }) { }
}
