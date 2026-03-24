# Audit-logic

API .NET com foco em auditoria de entidades de dominio usando atributo, Dapper e MySQL.

## 1. Objetivo do projeto

Este projeto implementa uma base de persistencia com trilha de auditoria automatica para entidades marcadas com o atributo `AuditableEntity`.

A ideia central e:

- manter a escrita em tabelas principais (clients, products, orders)
- registrar automaticamente cada alteracao relevante em tabelas de log
- garantir consistencia com transacao unica por operacao de escrita

Escopo atual da auditoria:

- INSERT
- UPDATE
- SOFT DELETE (seta `IsDeleted = 1`)

## 2. Stack tecnica

- .NET 10 (`net10.0`)
- ASP.NET Core
- Dapper
- MySqlConnector
- MySQL (InnoDB)

Pacotes definidos em `Audit-Project/Audit-Project.csproj`:

- Dapper 2.1.72
- Microsoft.AspNetCore.OpenApi 10.0.1
- MySqlConnector 2.5.0

## 3. Arquitetura (resumo)

O fluxo principal segue esta estrutura:

1. Service recebe a intencao de negocio (criar, atualizar, remover logicamente).
2. Repository executa SQL da entidade principal.
3. AuditService grava o snapshot em tabela de log.
4. Tudo ocorre na mesma transacao para evitar estado parcial.

```text
Service -> GenericRepository<T> -> (Tabela principal + Tabela de log) [mesma transacao]
```

Componentes principais:

- `Audit-Project/Attributes/AuditableEntityAttribute.cs`
- `Audit-Project/Repository/GenericRepository.cs`
- `Audit-Project/Audit/AuditService.cs`
- `Audit-Project/Audit/AuditMetadataResolver.cs`
- `Audit-Project/DependencyInjectionExtensions.cs`

## 4. Como a auditoria funciona

### 4.1 Marcacao de entidade auditavel

Entidades com auditoria usam o atributo:

```csharp
[AuditableEntity("orders")]
public class Order : BaseModel
{
		// ...
}
```

Convencao de tabela de log:

- `orders` -> `order_log`
- `products` -> `product_log`
- `clients` -> `client_log`

Se necessario, o nome da tabela de log pode ser sobrescrito no proprio atributo.

### 4.2 Operacoes auditadas

Em `GenericRepository<T>`:

- `AddAsync(entity, loggedBy)`
	- insere na tabela principal
	- registra log com `OperationType = INSERT`

- `UpdateAsync(entity, loggedBy)`
	- busca estado anterior
	- atualiza tabela principal
	- registra log com `OperationType = UPDATE`

- `DeleteAsync(id, loggedBy)`
	- faz soft delete (`IsDeleted = 1`)
	- registra log com `OperationType = SOFT_DELETE`

### 4.3 Metadados dinamicos da tabela de log

`AuditMetadataResolver` consulta `INFORMATION_SCHEMA.COLUMNS` para descobrir quais colunas existem em cada tabela de log, evitando insercao em coluna inexistente.

### 4.4 Snapshot especial para pedidos

Para `Order`, o `AuditService` grava tambem `ProductsSnapshot` em JSON quando a coluna existe na tabela de log.

## 5. Estrutura do banco

### 5.1 Tabelas principais

Script: `Audit-Project/Database/Create_All_Tables.sql`

- `Clients`
- `Products`
- `Orders`
- `OrderProducts`

### 5.2 Tabelas de log

Script: `Audit-Project/Database/Create_All_Log_Tables.sql`

- `client_log`
- `product_log`
- `order_log`
- `order_product_log`

Campos comuns de auditoria:

- `OperationType`
- `LoggedAt`
- `LoggedBy`

### 5.3 Migracao para bancos existentes

Se o banco ainda estiver no padrao antigo (`ClientLogs`, `ProductLogs`, etc), execute:

- `Audit-Project/Database/Migrations/Rename_Log_Tables_To_Snake.sql`

## 6. Como executar localmente

### 6.1 Pre-requisitos

- .NET SDK 10 instalado
- MySQL ativo e acessivel

### 6.2 Configuracao de conexao

Defina a chave `MySqlConnection` em `Audit-Project/appsettings.Development.json` (ou via variavel de ambiente/config secret).

Exemplo:

```json
{
	"MySqlConnection": "Server=localhost;Port=3306;Database=audit_logic;Uid=root;Pwd=senha;"
}
```

Observacao: o projeto tenta validar a conexao no startup usando `DatabaseTest`.

### 6.3 Criacao do schema

1. Execute `Create_All_Tables.sql`
2. Execute `Create_All_Log_Tables.sql`
3. Se necessario, execute a migracao de rename das tabelas de log

### 6.4 Build e run

```bash
dotnet restore
dotnet build Audit-logic.sln
dotnet run --project Audit-Project
```

Em ambiente de desenvolvimento, o OpenAPI e mapeado automaticamente.

## 7. Uso da camada de servico

As operacoes de escrita agora exigem `loggedBy`.

Exemplo de chamada de servico:

```csharp
await clientService.AddClientAsync(client, "lucas.jorge");
await clientService.UpdateClientAsync(client, "lucas.jorge");
await clientService.DeleteClientAsync(clientId, "lucas.jorge");
```

Isso permite rastreabilidade sem depender diretamente de `HttpContext` nesta fase.

## 8. Injeção de dependencias

Registros principais em `Audit-Project/DependencyInjectionExtensions.cs`:

- `IAuditMetadataResolver -> AuditMetadataResolver`
- `IAuditService -> AuditService`
- repositorios e servicos de dominio

Conexao com banco em `Audit-Project/Startup.cs`:

- `IDbConnection` com `MySqlConnection`
- health check inicial com `DatabaseTest`

## 9. Validacao recomendada (checklist)

Checklist rapido para validar auditoria ponta a ponta:

1. Inserir um `Client`.
2. Confirmar linha correspondente em `client_log` com `INSERT` e `LoggedBy`.
3. Atualizar esse `Client`.
4. Confirmar nova linha em `client_log` com `UPDATE`.
5. Remover logicamente (`DeleteAsync`) e confirmar `SOFT_DELETE`.
6. Verificar que `Clients.IsDeleted = 1` e que a trilha foi registrada.

## 10. Decisoes de design

- Soft delete por padrao no metodo de exclusao da camada generica.
- Auditoria acoplada ao repositorio para garantir transacao unica.
- Metadados de log resolvidos dinamicamente para reduzir acoplamento com schema.
- `loggedBy` propagado por parametro no service/repository.

## 11. Limitacoes atuais

- Nao ha testes de integracao automatizados no repositorio para validar auditoria.
- O projeto ainda nao fornece endpoints completos de CRUD expostos nesta documentacao.
- Operacoes em relacionamentos complexos (ex.: `OrderProducts`) podem exigir estrategia de log mais especifica dependendo da evolucao do dominio.

## 12. Proximos passos sugeridos

1. Adicionar testes de integracao para INSERT/UPDATE/SOFT_DELETE com assert em tabelas de log.
2. Padronizar `LoggedBy` via provider de contexto de usuario na API.
3. Criar endpoint/handler de consulta de historico por entidade (por Id).
4. Adicionar constraints/check para `OperationType` nas tabelas de log.

## 13. Licenca

Defina aqui a licenca do projeto (MIT, Apache-2.0, proprietaria, etc).