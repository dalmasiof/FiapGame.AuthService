# FiapGames.Auth

Microserviço responsável pela autenticação, autorização e gerenciamento de usuários da plataforma **FiapGames**.

Este serviço centraliza funcionalidades relacionadas a identidade dos usuários, emissão e validação de tokens JWT, login, cadastro e manutenção de contas da plataforma.

> **Objetivo:** fornecer uma base reutilizável e padronizada para microsserviços do ecossistema FiapGames, seguindo princípios de Clean Architecture, separação de responsabilidades e baixo acoplamento.

---

## Arquitetura do Projeto

A solução segue uma arquitetura em camadas inspirada em **Clean Architecture / DDD (Domain-Driven Design)**, visando facilitar manutenção, testes e evolução do sistema.

## Executar via Docker
Executar comando no cmd na raiz do projeto:
	docker compose up --build

A solução segue uma arquitetura em camadas inspirada em **Clean Architecture / DDD (Domain-Driven Design)**, visando facilitar manutenção, testes e evolução do sistema.


### Estrutura da solução

```txt
FiapGames.Auth.sln

src/
├── Auth.Api
├── Auth.Application
├── Auth.Infrastructure
└── Auth.Domain

test/
├── Auth.Application.Test
└── Auth.Domain.Test
```

### Responsabilidades das camadas

#### `Auth.Api`

Camada de exposição da API.

Responsável por:

* Endpoints REST
* Configuração de autenticação/autorização
* Swagger/OpenAPI
* Middleware e pipeline HTTP
* Configurações de DI (Dependency Injection)

#### `Auth.Application`

Camada de aplicação.

Responsável por:

* Regras de negócio da aplicação
* Serviços de aplicação
* Casos de uso
* DTOs
* Interfaces de contratos

#### `Auth.Domain`

Camada de domínio.

Responsável por:

* Entidades
* Regras de domínio
* Objetos de valor
* Contratos centrais
* Regras independentes de framework

#### `Auth.Infrastructure`

Camada de infraestrutura.

Responsável por:

* Persistência de dados
* Entity Framework Core
* Contextos (`DbContext`)
* Repositórios
* Integrações externas
* Implementações técnicas

#### `Tests`

Projetos de testes automatizados.

Responsável por:

* Testes unitários
* Testes de regras de negócio
* Garantia de qualidade do domínio e aplicação

---

## Principais Funcionalidades

Este microserviço é responsável por:

* Cadastro de usuários
* Autenticação de usuários
* Login com e-mail e senha
* Emissão de tokens JWT
* Controle de acesso
* CRUD de usuários
* Validação de credenciais
* Gerenciamento de status do usuário

---

## Stack Tecnológica

* **.NET 9**
* **ASP.NET Core Web API**
* **Entity Framework Core**
* **MySQL**
* **JWT Authentication**
* **Swagger / OpenAPI**
* **xUnit** (testes)

---

## Padrões Utilizados

O projeto segue alguns princípios e padrões arquiteturais:

* Clean Architecture
* SOLID
* Dependency Injection
* Repository Pattern
* Separation of Concerns
* Domain-Oriented Design

---

## Configuração do Ambiente

### Pré-requisitos

Antes de executar o projeto, certifique-se de possuir instalado:

* .NET SDK 9+
* MySQL
* Visual Studio 2022+ ou Rider
* EF Core CLI

Instalação do Entity Framework CLI:

```bash
dotnet tool install --global dotnet-ef
```

ou atualização:

```bash
dotnet tool update --global dotnet-ef
```

---

## Configuração do `appsettings.json`

Exemplo de configuração:

```json
{
  "ConnectionStrings": {
    "FIAPGamesConnection": "server=localhost;database=fiapgames_auth;user=root;password=sua_senha"
  },

  "Jwt": {
    "Key": "sua-chave-super-secreta",
    "Issuer": "FiapGames",
    "Audience": "FiapGamesUsers"
  }
}
```

---

## Executando o Projeto

Restaurar dependências:

```bash
dotnet restore
```

Executar a aplicação:

```bash
dotnet run --project src/Auth.Api
```

---

## Migrations

Criar uma migration:

```bash
dotnet ef migrations add InitialCreate \
--project src/Auth.Infrastructure \
--startup-project src/Auth.Api
```

Aplicar migrations:

```bash
dotnet ef database update \
--project src/Auth.Infrastructure \
--startup-project src/Auth.Api
```

---

## Testes

Executar testes:

```bash
dotnet test
```

---

## Convenções do Projeto

### Nomenclatura

#### Projetos

```txt
<Serviço>.Api
<Serviço>.Application
<Serviço>.Domain
<Serviço>.Infrastructure
```

#### Testes

```txt
<Serviço>.Application.Test
<Serviço>.Domain.Test
```

Essa convenção foi pensada para permitir reutilização da estrutura entre múltiplos microsserviços do ecossistema.

Exemplo:

```txt
FiapGames.Auth
FiapGames.Inventory
FiapGames.Payment
FiapGames.Matchmaking
```

---

## Roadmap

Funcionalidades previstas:

* Refresh Token
* Recuperação de senha
* Controle de perfis e permissões
* Rate limiting
* Observabilidade e logging
* Integração com mensageria

---

## Licença

Projeto desenvolvido para fins acadêmicos e evolução arquitetural da plataforma **FiapGames**.
