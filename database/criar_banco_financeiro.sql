-- Script para criar o banco de dados db_financeiro no host "estudos"
-- Conexão: estudos | User: app | Password: app
-- Copie e execute no MySQL Workbench ou via linha de comando

-- Criar o banco de dados
CREATE DATABASE IF NOT EXISTS db_Financeiro;
USE db_Financeiro;

-- Tabela de Despesas Fixas
CREATE TABLE IF NOT EXISTS dispesa_fixa (
  Cod_dispesa_fixa INT AUTO_INCREMENT PRIMARY KEY,
  Cod_usuario INT NOT NULL,
  Nome VARCHAR(255) NOT NULL,
  Valor DECIMAL(10, 2) NOT NULL,
  Data DATE,
  Categoria VARCHAR(255),
  Comentario VARCHAR(500),
  Data_atualizacao DATETIME,
  Valor_parcela DECIMAL(10, 2),
  Quantidade_parcelas INT,
  Tempo_indeterminado BOOLEAN DEFAULT FALSE,
  Finalizado BOOLEAN DEFAULT FALSE,
  CREATED_AT TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  UPDATED_AT TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Tabela de Despesas Variáveis
CREATE TABLE IF NOT EXISTS dispesa_variavel (
  Cod_dispesa_variavel INT AUTO_INCREMENT PRIMARY KEY,
  Cod_usuario INT NOT NULL,
  Nome VARCHAR(255) NOT NULL,
  Valor DECIMAL(10, 2) NOT NULL,
  Data DATE,
  Categoria VARCHAR(255),
  Comentario VARCHAR(500),
  CREATED_AT TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  UPDATED_AT TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Tabela de Faturamento (Income/Receitas)
CREATE TABLE IF NOT EXISTS faturamento (
  Cod_faturamento INT AUTO_INCREMENT PRIMARY KEY,
  Cod_usuario INT NOT NULL,
  Origem VARCHAR(255) NOT NULL,
  Valor DECIMAL(10, 2) NOT NULL,
  Data DATE,
  Comentario VARCHAR(500),
  CREATED_AT TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  UPDATED_AT TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Criar índices para melhor performance
CREATE INDEX idx_dispesa_fixa_usuario ON dispesa_fixa(Cod_usuario);
CREATE INDEX idx_dispesa_variavel_usuario ON dispesa_variavel(Cod_usuario);
CREATE INDEX idx_faturamento_usuario ON faturamento(Cod_usuario);

-- Inserir dados iniciais de exemplo (opcional)
-- INSERT INTO dispesa_fixa (Cod_usuario, Nome, Valor, Data, Comentario) 
-- VALUES (1, 'Aluguel', 1500.00, NOW(), 'Moradia');
