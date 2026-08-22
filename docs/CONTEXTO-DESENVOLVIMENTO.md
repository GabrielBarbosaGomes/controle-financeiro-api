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

Tabelas existentes: `db_financeiro.dispesa_fixa`, `db_financeiro.dispesa_variavel`, e as de `Income`. Colunas em português (`Cod_usuario`, `Nome`, `Valor`, `Data`, `Comentario`, etc.).

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

### 4.2 Restrição de schema (decisão do usuário)

**O schema do banco NÃO deve ser alterado** (não criar coluna `Categoria` nova). A Categoria da planilha deve ser mapeada dentro das colunas já existentes:
- `Nome` = Item da planilha (ex. "Luz").
- `Comentario` = Categoria da planilha (ex. "Moradia").

### 4.3 Regras de classificação fixa vs. variável e recorte temporal

> Estas regras entram aqui assim que validadas com o usuário (ver seção 6 — Plano de tasks). Editar esta seção quando a decisão for confirmada.

- Proposta em avaliação: itens que se repetem quase todo mês (Moradia/Luz, Condominio, Internet, Parc. Apt, Mercado, Academia, cursos, "Roubo Gov"/Das) → `dispesa_fixa`; itens esporádicos (Viagens, Compras, Lazer, Doações, Restaurante/Delivery quando majoritariamente zerados) → `dispesa_variavel`. Cada mês continua sendo um lançamento próprio (o valor muda mês a mês).
- Proposta em avaliação: importar a aba `Gastos 2026` só até o mês atual, ignorando meses futuros (para não distorcer o saldo atual).

## 5. Plano de tasks (import + dashboard)

Fluxo de trabalho combinado com o usuário para cada task: **(1)** a IA informa o plano da task → **(2)** usuário valida → **(3)** IA implementa → **(4)** usuário testa em tela; se funcionar, marca como validada, senão corrige e repete o teste.

| ID | Task | Repositório | Status |
|----|------|--------------|--------|
| B1 | Adicionar biblioteca de leitura de Excel (ClosedXML) ao `financeiroApi.csproj` | api | PENDENTE |
| B2 | Models `Model/Import/ImportRequest.cs` e `ImportResponse.cs` | api | PENDENTE |
| B3 | `Code/Business/Import/ImportDALSQL.cs` + `ImportDAL.cs` + `ImportBLL.cs` — parser da matriz, classificação fixa/variável, inserts em lote | api | PENDENTE |
| B4 | `Controllers/Import/ImportController.cs` — `POST /Api/Import/planilha` (upload multipart) | api | PENDENTE |
| B5 | `GET /Api/Dashboard/resumo` (saldo atual, maior categoria de gasto, saúde financeira) | api | PENDENTE |
| F1 | Tela de import (`/Importar`) + service + rota + item de menu | front | PENDENTE |
| F2 | `home.tsx` consumindo o dashboard real (hoje mockado) | front | PENDENTE |
| F3 | Coluna Categoria na grid de despesas (`debt.tsx`), lida do `Comentario` | front | PENDENTE |

## 6. Log de decisões

- 2026-08-22: decidido manter o schema do banco sem alterações — Categoria da planilha mapeada em `Comentario` (não cria coluna nova).
- 2026-08-22: `gh` CLI reautenticado com a conta pessoal `GabrielBarbosaGomes`; `user.name`/`user.email` configurados localmente (não globalmente) nos dois repositórios como `gabriel` / `gabryel122crf@gmail.com`.
