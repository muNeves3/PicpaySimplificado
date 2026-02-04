-- Criar banco de dados se não existir
CREATE DATABASE IF NOT EXISTS picpaysimplificado CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;

USE picpaysimplificado;

-- Tabela Wallets (Carteiras)
CREATE TABLE IF NOT EXISTS `Wallets` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `NomeCompleto` longtext CHARACTER SET utf8mb4 NOT NULL,
    `CPFCNPJ` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Email` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Senha` longtext CHARACTER SET utf8mb4 NOT NULL,
    `SaldoConta` decimal(18,2) NOT NULL,
    `UserType` longtext CHARACTER SET utf8mb4 NOT NULL,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `IX_Wallets_CPFCNPJ_Email` (`CPFCNPJ`, `Email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Tabela Transfers (Transferências)
CREATE TABLE IF NOT EXISTS `Transfers` (
    `IdTransferencia` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
    `SenderId` int NOT NULL,
    `ReciverId` int NOT NULL,
    `Valor` decimal(18,2) NOT NULL,
    PRIMARY KEY (`IdTransferencia`),
    KEY `IX_Transfers_ReciverId` (`ReciverId`),
    KEY `IX_Transfers_SenderId` (`SenderId`),
    CONSTRAINT `FK_Transaction_Reciver` FOREIGN KEY (`ReciverId`) REFERENCES `Wallets` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Transaction_Sender` FOREIGN KEY (`SenderId`) REFERENCES `Wallets` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Tabela de controle de migrations do EF Core
CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Inserir registro da migration já aplicada
INSERT IGNORE INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`) 
VALUES ('20260125171306_recriando_tabelas', '9.0.12');
