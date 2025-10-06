# 🛍️ ECommerce API - Sistema de Processamento de Vendas Assíncrono

Este projeto é uma API RESTful desenvolvida em **.NET 8 (Core)** que simula um sistema de E-commerce. Seu foco principal é demonstrar a arquitetura limpa (Clean Architecture), o uso do padrão Assíncrono (Background Jobs) e a comunicação com serviços externos de forma resiliente.

## 🚀 Tecnologias e Padrões Utilizados

  * **Linguagem & Framework:** C\# e .NET 8 (Core)
  * **Banco de Dados:** **SQLite** (Para persistência de dados local)
  * **Mapeamento:** **Entity Framework Core (EF Core)**
  * **Arquitetura:** **Clean Architecture** (Separação em Domínio, Aplicação, Infraestrutura e API)
  * **Comunicação Assíncrona:** **BackgroundService** (Worker Service) para processar o faturamento.
  * **Comunicação Externa:** **HttpClientFactory** para comunicação com o serviço de faturamento (Simulado).
  * **Configuração:** Padrão **IOptions** para configuração *type-safe* de URLs e *headers*.
  * **Qualidade de Código:** **FluentValidation** para validação de DTOs de entrada.
  * **Tratamento de Erros:** **Padrão Resultado (Result Pattern)** para encapsular sucesso/falha nas camadas de Serviço.

-----

## 🏗️ Estrutura do Projeto

O projeto está organizado em camadas de acordo com a Clean Architecture:

| Projeto | Responsabilidade | Dependências (Referências) |
| :--- | :--- | :--- |
| **`ECommerce.Api`** | **Apresentação (Composition Root).** Contém os Controllers, o `Program.cs` e a configuração de DI. | `Application`, `Infrastructure` |
| **`ECommerce.Application`** | **Lógica de Negócio e Orquestração.** Contém DTOs, Services e o Background Job. | `Domain`, `Infrastructure` (Apenas para DTOs externos) |
| **`ECommerce.Infrastructure`** | **Implementação de Contratos.** Contém Repositórios (EF Core/SQLite), `DbContext`, Clientes HTTP, e DTOs de serviços externos (`SaleSummaryDto`). | `Domain` |
| **`ECommerce.Domain`** | **Regras de Negócio e Contratos.** Contém Entidades, Enums e Interfaces (`IRepository`, `IBillingHttpClient`). | Nenhuma |

-----

## ⚙️ Configuração e Execução

### Pré-requisitos

  * .NET 8 SDK ou superior.
  * Uma ferramenta para testar APIs (ex: Postman, Insomnia ou extensão REST Client para VS Code).

### 1\. Configurar Conexão e URL

O projeto utiliza **SQLite** (armazenando um arquivo `sales.db` na raiz da API) e a configuração é lida do `appsettings.json`.

**`appsettings.json`:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=sales.db" 
  },
  "BillingService": {
    "BaseUrl": "http://servico-faturamento-externo.com", 
    "Endpoint": "/api/billing",                       
    "Email": "sistema.ecommerce@suaempresa.com" 
  }
}
```

### 2\. Rodar o Projeto

1.  Navegue até o diretório **`ECommerce.Api`** no terminal.
2.  Execute o comando:
    ```bash
    dotnet run
    ```
3.  A API e o Background Job serão iniciados, e o arquivo `sales.db` será criado automaticamente.

-----

## 🎯 Endpoints da API

A API expõe o recurso de vendas no plural (`/api/Sales`).

### 1\. Criar uma Nova Venda (Assíncrona)

Cria uma nova venda e a coloca em uma fila para processamento de faturamento em segundo plano.

| Verbo | Rota | Status de Sucesso |
| :--- | :--- | :--- |
| **`POST`** | `/api/Sales` | `202 Accepted` |

**Exemplo de Corpo da Requisição:**

```json
{
  "SaleDate": "2024-10-06T12:00:00-03:00",
  "Cliente": {
    "CustomerId": "b5a9147f-8e27-466d-8c11-968393537e22",
    "Name": "Cliente Teste",
    "Cpf": "123.456.789-01",
    "Category": "REGULAR" // Aceita a string do Enum
  },
  "Itens": [
    {
      "ProductId": 101,
      "Descricao": "Produto X - Simples",
      "Quantidade": 2.00,
      "PrecoUnitario": 50.00
    }
  ]
}
```

### 2\. Consultar Vendas

| Verbo | Rota | Descrição |
| :--- | :--- | :--- |
| **`GET`** | `/api/Sales` | Lista todas as vendas registradas. |
| **`GET`** | `/api/Sales/{id}` | Obtém os detalhes de uma venda específica. |

-----

## 🧩 Detalhes de Implementação

### Validação de Entrada

A requisição `POST` é protegida por **FluentValidation**.

  * O CPF e a **`Descricao`** dos itens são campos obrigatórios.
  * A categoria do cliente é convertida automaticamente de `string` para `Enum`.

### Processamento de Faturamento

1.  O `SaleService` enfileira o `Identifier` da venda.
2.  O **`BillingProcessorService`** (Background Job) retira o ID da fila.
3.  O Job carrega a venda completa do DB, **traduz** o modelo (`Sale` entity) para o DTO externo (`SaleSummaryDto` - na Infraestrutura).
4.  O Job chama o **`BillingHttpClient`** para enviar o DTO.
5.  O `HttpClient` é configurado com a `BaseUrl` (via DI) e o `EmailHeader` (via `IOptions` injetado no construtor).
6.  Em caso de **sucesso/falha**, o `BillingProcessorService` atualiza o `Status` da venda no banco de dados.

### Tratamento de Erros

Todas as operações de serviço retornam um objeto **`Result<T>`**.

  * **`IsSuccess = true`:** O Controller retorna `202 Accepted` (se assíncrono) ou `200 OK`.
  * **`IsFailure = true`:** O Controller retorna `400 Bad Request` com a mensagem de erro encapsulada (seja de validação ou de regra de negócio).
