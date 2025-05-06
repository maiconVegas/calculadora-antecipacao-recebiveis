import { LiveAnnouncer } from '@angular/cdk/a11y';
import { AfterViewInit, Component, ViewChild, inject, Input, SimpleChanges } from '@angular/core';
import { MatSort, Sort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { Produto } from '../../models/calculadoraAntecipacao.model';
import {MatPaginator, MatPaginatorModule} from '@angular/material/paginator';

@Component({
  standalone: true,
  selector: 'tabela-produtos',
  styleUrl: './tabela-produtos.component.scss',
  templateUrl: './tabela-produtos.component.html',
  imports: [MatTableModule, MatSortModule, MatPaginatorModule],
})
export class TabelaProdutosComponent implements AfterViewInit {

  @Input() dados: Produto[] = [];

  private _liveAnnouncer = inject(LiveAnnouncer);

  displayedColumns: string[] = ['nome', 'quantidade', 'valorUnitario', 'valorTotalProduto', 'cedenteCNPJ', 'dataVencimento'];
  dataSource = new MatTableDataSource(this.dados);

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['dados'] && changes['dados'].currentValue) {
      this.dataSource.data = this.dados;
    }
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }

  announceSortChange(sortState: Sort) {

    if (sortState.direction) {
      this._liveAnnouncer.announce(`Sorted ${sortState.direction}ending`);
    } else {
      this._liveAnnouncer.announce('Sorting cleared');
    }
  }
}
