import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ObterResumoVendaService } from './services/obterResumoVenda.service';
import { TabelaProdutosComponent } from './components/tabela-produtos/tabela-produtos.component';
import { Produto } from './models/calculadoraAntecipacao.model';
import { MatButtonModule } from '@angular/material/button';
import { DialogContentErros } from './components/dialog-content-erros/dialog-content-erros.component';
import { ViewChild } from '@angular/core';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule, TabelaProdutosComponent, MatFormFieldModule, MatInputModule, MatButtonModule, DialogContentErros],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {

  @ViewChild(DialogContentErros) dialogContentComponent!: DialogContentErros;

  chamarDialogFilho(mensagemErro?: string) {
    if (this.dialogContentComponent) {
      this.dialogContentComponent.openDialog(mensagemErro);
    }
  }


  selectedFile: File | null = null;
  valorTotal = '';
  cnpjCedente = '';
  dataVencimento = '';
  diasRestantes = '';
  taxaDiaria = '';
  existeProduto: boolean = false;
  formTouched: boolean = false;
  botaoHabilitado = true;

  dadosProdutos: Produto[] = [];

  constructor(private calculadoraService: ObterResumoVendaService) { }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
    }

    if (!this.selectedFile) return;

    this.calculadoraService.buscarDados(this.selectedFile).subscribe({
      next: (res) => {
        this.existeProduto = res.produtos.length >= 1;
        this.dadosProdutos = res.produtos;
        this.valorTotal = res.valorTotal.toString();
        this.cnpjCedente = res.cnpjCedente.toString();
        this.dataVencimento = res.dataVencimento.toString();
        this.diasRestantes = res.diasRestantes.toString();
        this.taxaDiaria = res.taxaDiaria.toString();
        this.botaoHabilitado = false;
      },

      error: err => {
        this.chamarDialogFilho(err.error.errors.Mensagens + '');
        this.existeProduto = false;
        this.dadosProdutos.length = 0;
        this.valorTotal = '';
        this.cnpjCedente = '';
        this.dataVencimento = '';
        this.diasRestantes = '';
        this.taxaDiaria = '';
        this.botaoHabilitado = true;
      }
    });

  }

  onFieldChange() {
    this.selectedFile = null;
    this.existeProduto = false;

    if (!this.formTouched && (this.valorTotal || this.cnpjCedente || this.dataVencimento)) {
      this.formTouched = true;

    }

    if (this.formTouched) {
      if (this.valorTotal && this.cnpjCedente && this.dataVencimento) {

        const partes = this.dataVencimento.split('/');
        this.diasRestantes = Math.ceil((new Date(+partes[2], +partes[1] - 1, +partes[0]).getTime() - new Date().getTime()) / (1000 * 60 * 60 * 24)).toString();
        console.log('Todos os 3 campos foram preenchidos com sucesso!');
        const diasRest = Number(this.diasRestantes);
        if (diasRest >= 30 && diasRest <= 60) {
          this.taxaDiaria = 0.033.toString();
        } else if (diasRest > 60 && diasRest <= 90) {
          this.taxaDiaria = 0.028.toString();
        } else if (diasRest > 90 && diasRest <= 120) {
          this.taxaDiaria = 0.025.toString();
        } else if (diasRest > 120 && diasRest <= 180) {
          this.taxaDiaria = 0.023.toString();
        } else if (diasRest > 180 && diasRest <= 365) {
          this.taxaDiaria = 0.022.toString();
        } else if (diasRest > 365) {
          this.taxaDiaria = 0.020.toString();
        } else {
          this.taxaDiaria = '';
        }

        //this.botaoHabilitado = false;


      }
    }
  }

}
