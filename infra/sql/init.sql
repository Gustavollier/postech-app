-- Script de criação das tabelas principais para o ambiente da oficina
CREATE DATABASE PosTechChallenge;
GO
USE PosTechChallenge;
GO

CREATE TABLE Funcionario (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nome NVARCHAR(100) NOT NULL,
    Contato NVARCHAR(50),
    CPF NVARCHAR(14) NOT NULL,
    Cargo INT NOT NULL, -- Referencia o enum CargoFuncionario
    ValorHora INT NOT NULL
);

CREATE TABLE Seguranca (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FuncionarioId INT NOT NULL,
    SenhaHash NVARCHAR(255) NOT NULL,
    CriadoEm DATETIME NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (FuncionarioId) REFERENCES Funcionario(Id)
);

CREATE TABLE Cliente (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NOT NULL,
    CPF NVARCHAR(14),
    CNPJ NVARCHAR(18),
    NomeCompleto NVARCHAR(100) NOT NULL,
    Telefone NVARCHAR(20),
    Email NVARCHAR(100)
);

CREATE TABLE Pecas (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nome NVARCHAR(100) NOT NULL,
    Marca NVARCHAR(50),
    Codigo NVARCHAR(50),
    Preco NVARCHAR(20),
    UnidadeMedida INT NOT NULL,
    CriadoEm DATETIME NOT NULL,
    AtualizadoEm DATETIME NOT NULL
);

CREATE TABLE Veiculo (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Marca NVARCHAR(50) NOT NULL,
    Modelo NVARCHAR(50) NOT NULL,
    Placa NVARCHAR(10) NOT NULL,
    Cor NVARCHAR(30),
    AnoModelo INT NOT NULL,
    AnoFabricacao INT NOT NULL,
    KmEntrada INT NOT NULL
);
