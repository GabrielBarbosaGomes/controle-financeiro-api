# 💰 Controle Financeiro — API

API REST de um sistema de controle financeiro pessoal: cadastro de despesas fixas e variáveis, receitas, categorização por tipo de gasto e import em lote de uma planilha financeira. Serve de back-end para o [front-end React](https://github.com/GabrielBarbosaGomes/controle-financeiro).

Contexto completo do projeto — missão, regras de negócio, decisões de arquitetura e histórico de mudanças — está em [`docs/CONTEXTO-DESENVOLVIMENTO.md`](docs/CONTEXTO-DESENVOLVIMENTO.md).

## Funcionalidades

- CRUD de despesas fixas (`dispesa_fixa`) — despesas recorrentes (ex. aluguel, internet), com controle de parcelamento.
- CRUD de despesas variáveis (`dispesa_variavel`) — despesas pontuais.
- CRUD de receitas (`faturamento`).
- Categorização das despesas (Moradia, Alimentação, Transporte, etc.).
- Resumo mensal de gastos (soma agregada por mês).
- Import de uma planilha `.xlsx` de gastos, com parser tolerante a formatos de data/valor inconsistentes, classificação automática em fixa/variável e corte de meses futuros.

## Stack e padrão de arquitetura

- **.NET 8** / ASP.NET Core Web API.
- **[Dapper](https://github.com/DapperLib/Dapper)** como micro-ORM — **sem Entity Framework**, SQL escrito à mão.
- **MySQL 8** como banco de dados.
- **[ClosedXML](https://github.com/ClosedXML/ClosedXML)** para leitura do `.xlsx` na feature de import.
- **Swashbuckle/Swagger** para documentação interativa da API (`/swagger`, em ambiente de desenvolvimento).
- **xUnit** para testes unitários (`financeiroApi.Tests`).

Não há um "sistema de design" visual aqui (é uma API), mas há um padrão arquitetural consistente que toda feature segue — camadas por feature em vez de uma camada só:

```
Controllers/<Feature>/<Feature>Controller.cs   → [ApiController], rota Api/[controller], injeta a BLL
Code/Business/<Feature>/<Feature>BLL.cs        → regra de negócio, recebe a DAL por injeção
Code/Business/<Feature>/<Feature>DAL.cs        → herda MySqlAccess, monta parâmetros, executa via Dapper
Code/Business/<Feature>/<Feature>DALSQL.cs     → só métodos que retornam o SQL cru (StringBuilder)
Model/<Feature>/<Feature>Request.cs            → DTO de entrada
Model/<Feature>/<Feature>Response.cs           → DTO de saída
```

Registro de dependências manual em `Program.cs` (`builder.Services.AddScoped<...>()`), sem container de DI de terceiros. CORS liberado para qualquer origem. **Sistema ainda é mono-usuário/protótipo, sem autenticação real** — `CodUsuario` é fixado em `1` em vários pontos.

## Como rodar

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download) (o `dotnet run` pelo terminal precisa do runtime 8 instalado — o Visual Studio traz uma cópia própria e funciona mesmo sem isso, mas a linha de comando não).
- [Docker](https://www.docker.com/) (recomendado, pra subir o MySQL local) — ou um MySQL 8 já rodando em algum lugar.

### 1. Banco de dados

Com Docker (mais simples):

```bash
docker compose up -d
```

Sobe um MySQL 8 em `localhost:3306` (usuário `root`, senha `senhabanco123`, banco `db_Financeiro`) e já roda `database/criar_banco_financeiro.sql` automaticamente na primeira subida, criando as tabelas.

Sem Docker: rode `database/criar_banco_financeiro.sql` manualmente num MySQL 8 já existente e ajuste a `ConnectionStrings:DefaultConnection` em `appsettings.json` para apontar pra ele.

### 2. API

```bash
dotnet run --launch-profile https
```

Sobe em `https://localhost:7262` (e `http://localhost:5083`). Com `ASPNETCORE_ENVIRONMENT=Development` (padrão do profile), a documentação interativa fica em `https://localhost:7262/swagger`.

### 3. Testes

```bash
dotnet test
```

Roda a suíte do `financeiroApi.Tests` (cobre principalmente o parser da feature de import — sem necessidade de banco rodando, são testes unitários puros).

## Estrutura de pastas

```
Controllers/<Feature>/     # endpoints HTTP
Code/Business/<Feature>/   # BLL, DAL, DALSQL por feature
Code/Connection/           # acesso ao MySQL (MySqlAccess)
Model/<Feature>/           # DTOs de request/response
database/                  # script de criação do schema
financeiroApi.Tests/       # testes unitários (xUnit)
docker-compose.yml         # MySQL local para desenvolvimento
```
