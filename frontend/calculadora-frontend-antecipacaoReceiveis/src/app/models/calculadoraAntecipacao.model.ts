export interface Produto {
  nome: string;
  quantidade: number;
  valorUnitario: number;
  valorTotalProduto: number;
  cedenteCNPJ: string;
  dataVencimento: string;
}

export interface Response {
  valorTotal: number;
  cnpjCedente: string;
  dataVencimento: string;
  diasRestantes: number;
  taxaDiaria: number;
  produtos: Produto[];
}

export interface SimulacaoResponse {
  id: number;
  valorTotal: number;
  valorTotalComTaxa: number;
  cnpjCedente: string;
  dataVencimento: string;
  taxaDiaria: number;
  enderecoCsvBlob: string;
  dateTimeSimulado: string;
}
