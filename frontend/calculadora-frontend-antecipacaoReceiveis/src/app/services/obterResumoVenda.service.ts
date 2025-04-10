import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Response } from '../models/calculadoraAntecipacao.model';

// Com isso posso usar esse serviço em qualquer componente, pois ele é injetável globalmente
// infelizmente nao tem nessa versão do angular o app.modules ;-;
@Injectable({
    providedIn: 'root'
})

export class ObterResumoVendaService {
    private apiUrl = 'https://localhost:7136/vendas/upload';

    constructor(private httpCliente: HttpClient) {
    }

    buscarDados(file: File): Observable<Response> {

        const formData = new FormData();
        formData.append('Arquivo', file);

        return this.httpCliente.post<Response>(this.apiUrl, formData);
    }
}

