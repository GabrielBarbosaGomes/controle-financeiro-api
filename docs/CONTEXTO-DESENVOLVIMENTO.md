# Contexto de desenvolvimento — Sistema Financeiro Pessoal

Este documento é a fonte de verdade versionada (via Git/GitHub) sobre a missão, as regras de negócio e as decisões de arquitetura do projeto. Existe porque decisões de contexto guardadas só em memória local do assistente (fora da pasta do projeto) não são versionadas nem sincronizadas — este arquivo é o registro oficial.

Repositório irmão: [`controle-financeiro`](https://github.com/GabrielBarbosaGomes/controle-financeiro) (front-end). Este repositório (`controle-financeiro-api`) é o back-end.

## 1. Missão do sistema

Dashboard financeiro pessoal que deve mostrar:
- **Saúde financeira** (visão geral/score).
- **Saldo atual**.
- **Com o que mais se gastou** (maior categoria/origem de gasto).

## 2. Conceitos de negócio

- **Despesa fixa** (`dispesa_fixa`): despesa que se repete sempre/recorrente (ex.: conta de energia). Tem campos de parcelamento (`Valor_parcela`, `Quantidade_parcelas`, `Tempo_indeterminado`, `Finalizado`).
- **Despesa variável** (`dispesa_variavel`): despesa pontual ou com prazo de validade (não recorrente indefinidamente).
- **Income** (receitas): já existe como feature própria (`Origem`, `Valor`, `Data`, `Comentario`).

## 3. Padrão arquitetural (back-end)

.NET 8 + Dapper + MySQL (schema `db_financeiro`), **sem Entity Framework**. Camadas por feature:

```
Controllers/<Feature>/<Feature>Controller.cs   → [ApiController], rota Api/[controller], injeta a BLL
Code/Business/<Feature>/<Feature>BLL.cs        → regra de negócio, recebe DAL via injeção
Code/Business/<Feature>/<Feature>DAL.cs        → herda MySqlAccess, monta DynamicParameters, chama Db.Query/Db.Execute (Dapper)
Code/Business/<Feature>/<Feature>DALSQL.cs     → só métodos que retornam SQL cru (StringBuilder/AppendLine)
Model/<Feature>/<Feature>Request.cs            → DTO de entrada, propriedades nullable
Model/<Feature>/<Feature>Response.cs           → DTO de saída
```

Registro de DI manual em `Program.cs` (`builder.Services.AddScoped<...>()`). CORS liberado para qualquer origem. **Sem autenticação real ainda** — `CodUsuario` é fixado em `1` em vários pontos (BLL e front-end): sistema mono-usuário/protótipo até o momento.

Tabelas existentes: `db_financeiro.dispesa_fixa`, `db_financeiro.dispesa_variavel` (schema case-insensitive — `lower_case_table_names=1`, igual ao padrão Windows/MySQL Workbench, configurado no `docker-compose.yml`), e `db_financeiro.faturamento` (tabela do `Income`). Colunas em português (`Cod_usuario`, `Nome`, `Valor`, `Categoria`, `Comentario`, `Data`, etc.) — `Categoria` e `Comentario` são campos **distintos**: `Categoria` guarda a categoria (Moradia, Alimentação...) e `Comentario` é texto livre opcional. Script de criação em `database/criar_banco_financeiro.sql`; ambiente local sobe via `docker-compose.yml` (MySQL 8, roda o script automaticamente na primeira subida).

## 4. Import da planilha financeira do usuário

### 4.1 Fonte

Planilha pessoal do usuário, baixada como `Planilha RD - saude financeira.xlsx`. 5 abas:

- **`Sonhos`**: metas financeiras (Nome da meta + valor alvo, ex. "Comprar uma moto — R$ 25.000"). **Fora de escopo** por enquanto — não existe tabela/feature equivalente no sistema.
- **`Dívidas`, `Gastos 2024`, `Gastos 2025`, `Gastos 2026`**: todas em formato de **matriz larga** (apesar do nome "Dívidas", tem o mesmo formato das demais — aparenta ser o ano 2022/2023 mal nomeado):
  - Coluna A = **Categoria** (Moradia, Alimentação, Transporte, Viagens, Educação, Saude, Compras, Lazer, Doações, "Roubo Gov").
  - Coluna B = **Item/subcategoria** (ex. "Parc. Apt", "Luz", "Condominio", "Mercado", "Uber", "Academia").
  - Colunas seguintes = um **mês** cada, valor formatado como texto `"R$ 1.234,56"` (separador de milhar/decimal **inconsistente** em algumas células — parser precisa ser tolerante).
  - Linhas a **ignorar** no parser: título ("Planilha de gastos"), "Reserva dos sonhos e projetos", "Reserva de emergência", "Salário" (é receita → mapeia para `Income`, não para despesa), "Lucro" (calculado), cabeçalho ("Categorias"/"Gasto"), "Total:" (somatório calculado).
  - **Bug conhecido nos dados**: na aba `Gastos 2026` os rótulos de mês repetem "2025" para jan–set (só corrigem em out/nov/dez/2026) — não confiar cegamente no texto do header, inferir o ano pela aba/posição da coluna. Além disso essa aba tem valores preenchidos até dezembro/2026 (meses futuros em relação à data de hoje), o que sugere orçamento planejado, não gasto já realizado.

### 4.2 Coluna Categoria (decisão revista)

Decisão original era não alterar o schema (Categoria dentro de `Comentario`). **Revista em 2026-08-22**: depois de ver o dado na tela (o campo "comentario" mostrando a categoria era confuso), o usuário pediu uma coluna `Categoria` de verdade. Schema atualizado:
- `dispesa_fixa.Categoria` e `dispesa_variavel.Categoria` (`VARCHAR(255)`, nullable) — campo dedicado pra categoria (ex. "Moradia", "Alimentação").
- `Comentario` volta a ser texto livre opcional (`Nome` = Item da planilha, ex. "Luz").
- Dado já importado migrado via `UPDATE ... SET Categoria = Comentario, Comentario = NULL` (feito manualmente no banco rodando + refletido no `criar_banco_financeiro.sql`).

### 4.3 Regras de classificação fixa vs. variável e recorte temporal (confirmadas)

- Itens que se repetem quase todo mês (Moradia/Luz, Condominio, Internet fixa e movel, Parc. Apt, Parc. Apt Ex., Mercado, Academia, Curso ingles, Curso RD, Facudade, Plano de saude, BJJ, "Roubo Gov"/Das) → `dispesa_fixa` (lista hardcoded em `ImportBLL.ItensFixos`); todo o resto → `dispesa_variavel`. Cada mês continua sendo um lançamento próprio (o valor muda mês a mês, `QuantidadeParcelas=1`/`TempoIndeterminado=true` nas fixas).
- Corte de meses futuros: qualquer coluna cujo mês seja posterior ao mês atual é ignorada (evita poluir com orçamento planejado da aba `Gastos 2026`).
- Salário/receita e a aba `Sonhos` (metas) ficaram **fora** desta rodada de import — dados inconsistentes/sem tabela equivalente ainda.
- **Bug de parsing encontrado e corrigido**: nas abas `Dívidas` e `Gastos 2024`, o cabeçalho de mês é uma célula de **data real do Excel** (não texto) por causa de um autofill que incrementou mês e dia juntos — o parser (`ImportBLL.MapearColunasDeMes`) trata `XLDataType.DateTime` além de texto.

## 5. Plano de tasks (import + dashboard)

Fluxo de trabalho combinado com o usuário para cada task: **(1)** a IA informa o plano da task → **(2)** usuário valida → **(3)** IA implementa → **(4)** usuário testa em tela; se funcionar, marca como validada, senão corrige e repete o teste.

| ID | Task | Repositório | Status |
|----|------|--------------|--------|
| B1 | Adicionar biblioteca de leitura de Excel (ClosedXML) ao `financeiroApi.csproj` | api | VALIDADA |
| B2 | Models `Model/Import/ImportRequest.cs` e `ImportResponse.cs` | api | VALIDADA |
| B3 | `Code/Business/Import/ImportBLL.cs` — parser da matriz, classificação fixa/variável, inserts em lote (reaproveita `DebtDAL`, sem `ImportDAL`/`ImportDALSQL` separados) | api | VALIDADA |
| B4 | `Controllers/Import/ImportController.cs` — `POST /Api/Import/planilha` (upload multipart) | api | VALIDADA |
| B5 | `GET /Api/Dashboard/resumo` (saldo atual, maior categoria de gasto, saúde financeira) | api | PENDENTE |
| — | *(fora do plano original, feito por necessidade)* `GET /Api/Debt/all/get` reescrito — a query original fazia `JOIN` sem condição de mês (produto cartesiano) e o `DebtResponse` não tinha os campos que o front-end esperava; agora agrega por mês (`MesAno`/`TotalGasto`) | api | VALIDADA |
| — | *(fora do plano)* Coluna `Categoria` dedicada em `dispesa_fixa`/`dispesa_variavel` (ver 4.2) | api | IMPLEMENTADA (aguardando teste do usuário em tela) |
| F1 | Tela de import (`/Importar`) + service + rota + item de menu | front | PENDENTE |
| F2 | `home.tsx` consumindo o dashboard real (hoje mockado) | front | PENDENTE |
| F3 | Coluna Categoria na grid de despesas (`debt.tsx`) + campo Categoria nos formulários de criar/editar despesa | front | IMPLEMENTADA (aguardando teste do usuário em tela) |
| — | *(fora do plano)* `get-debit-all.ts` não enviava `codUsuario`, `debtList.tsx` sem mensagem de "sem dados" | front | VALIDADA |

## 6. Log de decisões

- 2026-08-22: `gh` CLI reautenticado com a conta pessoal `GabrielBarbosaGomes`; `user.name`/`user.email` configurados localmente (não globalmente) nos dois repositórios como `gabriel` / `gabryel122crf@gmail.com`.
- 2026-08-22: ambiente local montado — MySQL via Docker (`docker-compose.yml`, `lower_case_table_names=1`), runtime .NET 8 instalado via winget (Visual Studio já tinha sua própria cópia privada, por isso rodava por lá mas não via `dotnet run`).
- 2026-08-22: decisão inicial de não alterar schema (Categoria em `Comentario`) foi **revertida** — usuário pediu coluna `Categoria` dedicada depois de ver o dado confuso na tela. Ver seção 4.2.
- 2026-08-22: import rodado com sucesso contra a planilha real — 277 despesas fixas + 142 variáveis, cobrindo junho/2022 a agosto/2026, valores conferidos contra a linha "Total:" da planilha.
- 2026-08-22: corrigidos dois bugs pré-existentes (não relacionados ao import) descobertos ao testar com dado real: `GET /Api/Debt/all/get` fazia `JOIN` sem condição de mês (nunca tinha sido testado com dados) e `get-debit-all.ts` não enviava `codUsuario`.
