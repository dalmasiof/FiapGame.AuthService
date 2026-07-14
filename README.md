# FiapGames.Auth

Microserviço responsável por autenticação, autorização e gerenciamento de usuários da plataforma FiapGames.

Este serviço centraliza funcionalidades de identidade, emissão e validação de tokens JWT, login, cadastro e manutenção de contas do ecossistema.

> Objetivo: fornecer uma base reutilizável e padronizada para microsserviços, seguindo princípios de Clean Architecture, separação de responsabilidades e baixo acoplamento.

---

## Arquitetura do Projeto

A solução segue uma estrutura em camadas inspirada em Clean Architecture / DDD, com separação entre API, aplicação, domínio e infraestrutura.

### Estrutura da solução

```txt
FiapGames.Auth.slnx

src/
├── 1-Auth.Api
├── 2-Auth.Application
├── 3-Auth.Infrastructure
└── 4-Auth.Domain

test/
├── Auth.Application.Test
└── Auth.Domain.Test
```

### Responsabilidades das camadas

#### 1-Auth.Api

Camada de exposição da API.

Responsável por:
- endpoints REST
- autenticação e autorização JWT
- Swagger/OpenAPI
- middleware e pipeline HTTP
- injeção de dependência

#### 2-Auth.Application

Camada de aplicação.

Responsável por:
- regras de negócio
- serviços de aplicação
- DTOs
- contratos de serviço

#### 4-Auth.Domain

Camada de domínio.

Responsável por:
- entidades
- regras de domínio
- contratos de repositório
- regras independentes de framework

#### 3-Auth.Infrastructure

Camada de infraestrutura.

Responsável por:
- persistência com Entity Framework Core
- repositórios
- integração com mensageria RabbitMQ
- implementação de serviços técnicos

---

## Funcionalidades atuais

O microserviço implementa:
- cadastro de usuários
- autenticação por login e senha
- emissão de tokens JWT
- troca de senha
- consulta de usuários por ID, e-mail e lista completa
- atualização e remoção de usuários
- integração com eventos de mensageria

---

## Stack Tecnológica

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT Authentication
- Swagger / OpenAPI
- RabbitMQ
- xUnit (testes)

---

## Pré-requisitos

Antes de executar o projeto, certifique-se de ter instalado:
- .NET SDK 10
- SQL Server ou SQL Server LocalDB
- RabbitMQ, caso deseje validar a integração de mensagens
- dotnet-ef CLI

Instalar o Entity Framework CLI:

```bash
dotnet tool install --global dotnet-ef
```

ou atualizar:

```bash
dotnet tool update --global dotnet-ef
```

---

## Configuração do ambiente

O projeto lê a string de conexão no arquivo de configuração da API. O valor padrão em appsettings.json usa LocalDB:

```json
{
  "ConnectionStrings": {
    "FIAPGamesConnection": "Server=(localdb)\\mssqllocaldb;Database=fiapgames_auth;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Jwt": {
    "Key": "ChaveSuperSecretaFiapGamesEAD12MuitoLongaParaGarantirSeguranca123!",
    "Issuer": "FiapGamesApi",
    "Audience": "FiapGamesClients"
  }
}
```

---

## Executando localmente

Restaurar dependências:

```bash
dotnet restore
```

Executar a aplicação:

```bash
dotnet run --project src/1-Auth.Api/1-Auth.Api.csproj
```

A API ficará disponível em http://localhost:5195 ou https://localhost:7286, conforme o profile de execução configurado em launchSettings.json.

---

## Executando com Docker

Na raiz do projeto, execute:

```bash
docker compose up --build
```

Esse ambiente sobe a API e o SQL Server. A aplicação também está preparada para usar RabbitMQ, mas o serviço de mensageria não é provisionado no compose atual.

---

## Kubernetes (autonomia por serviço)

Manifests próprios do serviço estão em `k8s/`:

- `auth-api-configmap.yaml`
- `auth-api-secret.yaml`
- `auth-api-service.yaml`
- `auth-api-deployment.yaml`

Configurações não sensíveis ficam em ConfigMap (ambiente, issuer/audience e host/porta de RabbitMQ).
Dados sensíveis ficam em Secret (connection string completa, chave JWT e credenciais de RabbitMQ).

---

## Migrations

Criar uma migration:

```bash
dotnet ef migrations add InitialCreate \
--project src/3-Auth.Infrastructure \
--startup-project src/1-Auth.Api
```

Aplicar migrations:

```bash
dotnet ef database update \
--project src/3-Auth.Infrastructure \
--startup-project src/1-Auth.Api
```

---

## Endpoints principais

A API expõe os seguintes endpoints:

- POST /api/login
- POST /api/auth/login
- POST /api/auth/trocar-senha
- GET /api/login/{id}
- GET /api/login/email/{email}
- GET /api/login
- PUT /api/login
- DELETE /api/login/{id}

---

## Testes

Executar a suíte de testes:

```bash
dotnet test
```

---

## Licença

Projeto desenvolvido para fins acadêmicos e evolução arquitetural da plataforma FiapGames.
