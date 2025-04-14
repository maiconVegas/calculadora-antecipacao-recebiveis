import { ChangeDetectionStrategy, Component, inject, Injectable } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';

@Injectable({
  providedIn: "root"
})

export class DialogContentTutorial {
  constructor(private dialog: MatDialog){}
  //readonly dialog = inject(MatDialog);
  openDialog() {
    this.dialog.open(DialogContentTutorialDialog);
  }
}

@Component({
  selector: 'dialog-content-tutorial-dialog',
  templateUrl: 'dialog-content-tutorial.component-dialog.html',
  imports: [MatDialogModule, MatButtonModule, CommonModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  standalone: true,

})
export class DialogContentTutorialDialog {
}
