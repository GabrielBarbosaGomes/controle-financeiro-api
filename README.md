# 💰 Sistema Financeiro - Backend

Este projeto é uma API RESTful desenvolvida em **C# (.NET)** que utiliza o micro ORM **Dapper** e banco de dados **MySQL**, com o objetivo de gerenciar um sistema financeiro simples. A aplicação permite que o usuario crie sua conta, realize lançamentos financeiros (entradas e saídas) e análise de dados.
Projeto em desenvolvimento, no momento estou modelando o banco de dados para criação das tabelas
---

## 🛠️ Tecnologias Utilizadas

- [.NET 7+](https://dotnet.microsoft.com/)
- [Dapper](https://github.com/DapperLib/Dapper)
- [MySQL](https://www.mysql.com/)
- [Swagger (Swashbuckle)](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) - Documentação da API
- [RESTful APIs](https://restfulapi.net/)

---

## 📁 Estrutura do Projeto

```plaintext
📦FinanceiroApi
├── 📂Controllers                   # Endpoints da API
│   └── LancamentoController.cs       # Controller para lançamentos
│   └── UsuarioController.cs          # Controller de autenticação e cadastro
│
├── 📂BLL                          # Camada de regras de negócio
│   └── LancamentoService.cs         # Regras de negócio dos lançamentos
│   └── UsuarioService.cs            # Regras de negócio para usuários
│
├── 📂DAL                          # Camada de acesso a dados (Data Access Layer)
│   └── LancamentoRepository.cs      # Operações com Dapper para lançamentos
│   └── UsuarioRepository.cs         # Operações com Dapper para usuários
│
├── 📂DALSQL                       # Queries SQL reutilizáveis
│   └── LancamentoSql.cs            # Strings SQL para lançamentos
│   └── UsuarioSql.cs               # Strings SQL para usuários
│
├── 📂Models                       # Modelos/Entidades da aplicação
│   └── Usuario.cs                  # Modelo do usuário (Id, Nome, Email, SenhaHash)
│   └── Lancamento.cs               # Modelo de lançamento (Id, Descricao, Valor, Tipo, Data, CategoriaId, UsuarioId)
│   └── Categoria.cs                # Modelo de categoria (Id, Nome)
│
├── 📂Utils                        # Classes auxiliares
│   └── CriptografiaHelper.cs       # Utilitário para hash de senhas
│
├── Program.cs                    # Arquivo principal da aplicação
├── appsettings.json              # Configurações (string de conexão, etc.)
