import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { SimulacaoResponse } from "../models/calculadoraAntecipacao.model";

@Injectable({
    providedIn: "root"
})

export class AdicionarSimulacaoService {
    private apiUrl = "https://localhost:7136/Simulacao";

    constructor(private httpClient: HttpClient) { }

    simularDados(
        valorTotal: string,
        cnpjCedente: string,
        dataVencimento: string,
        diasRestantes: string,
        taxaDiaria: string,
        file: File | null = null,): Observable<SimulacaoResponse> {
        const formData = new FormData();
        formData.append("ValorTotal", valorTotal.toString().replace('.', ','));


        formData.append("CNPJCedente", cnpjCedente);
        formData.append("DataVencimento", dataVencimento);
        formData.append("DiasRestantes", diasRestantes.toString());
        formData.append("TaxaDiaria", taxaDiaria.toString().replace('.', ','));
        
        if (file) {
            formData.append("Arquivo", file);
        }

        return this.httpClient.post<SimulacaoResponse>(this.apiUrl, formData);
    }
}