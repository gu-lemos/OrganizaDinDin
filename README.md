# OrganizaDinDin

Sistema de gerenciamento de gastos pessoais desenvolvido com ASP.NET Core MVC e Google Cloud Firestore.

## Sobre o Projeto

OrganizaDinDin é uma aplicação web para controle financeiro pessoal que permite aos usuários registrar, categorizar e visualizar seus gastos de forma organizada e intuitiva. O sistema oferece recursos de autenticação, CRUD completo de gastos e visualizações gráficas para análise de despesas.

## Funcionalidades

- **Autenticação de Usuários**
  - Cadastro de novos usuários
  - Login com validação customizada
  - Sistema de sessão com cookies
  - Senha criptografada com BCrypt

- **Gerenciamento de Gastos**
  - Criar, editar e excluir gastos
  - Categorização por tipo
  - Registro de descrição, valor e data
  - Validação de formulários em tempo real

- **Visualização e Análise**
  - Lista completa de gastos com filtros
  - Resumo financeiro com gráficos interativos
  - Gráfico de barras por categoria
  - Gráfico de pizza com distribuição percentual
  - Tabela detalhada com análise por tipo

- **Design Responsivo**
  - Interface adaptável para desktop, tablet e mobile
  - Sidebar retrátil em dispositivos móveis
  - Tema escuro e moderno
  - Feedback visual com toasts

## Tecnologias Utilizadas

### Backend
- **.NET 9.0** - Framework principal
- **ASP.NET Core MVC** - Arquitetura MVC
- **Google Cloud Firestore** - Banco de dados NoSQL
- **BCrypt.Net-Next** - Criptografia de senhas
- **Cookie Authentication** - Sistema de autenticação

### Frontend
- **Bootstrap 5** - Framework CSS
- **Bootstrap Icons** - Ícones
- **Chart.js** - Gráficos interativos
- **Vanilla JavaScript** - Validações e interações
- **Razor Pages** - Template engine

## Arquitetura

O projeto segue os princípios de Clean Architecture:

```
OrganizaDinDin/
├── Application/           # Camada de aplicação
│   ├── DTOs/             # Data Transfer Objects
│   ├── Filters/          # Filtros de validação
│   ├── Interfaces/       # Contratos de serviços
│   └── Services/         # Implementação de serviços
├── Controllers/          # Controladores MVC
├── Domain/              # Camada de domínio
│   ├── Entities/        # Entidades do domínio
│   ├── Enums/           # Enumeradores
│   └── Interfaces/      # Contratos de repositórios
├── Firebase/            # Configuração do Firebase
├── Helpers/             # Classes auxiliares
├── Infrastructure/      # Camada de infraestrutura
│   └── Repositories/    # Implementação de repositórios
├── Views/              # Views Razor
│   ├── Auth/           # Views de autenticação
│   ├── Gastos/         # Views de gastos
│   └── Shared/         # Componentes compartilhados
└── wwwroot/            # Arquivos estáticos
    ├── css/            # Estilos CSS
    └── js/             # Scripts JavaScript
```

## Pré-requisitos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- Conta no [Google Cloud Platform](https://cloud.google.com/)
- Projeto Firebase com Firestore habilitado

## Configuração

### 1. Clone o repositório

```bash
git clone https://github.com/seu-usuario/OrganizaDinDin.git
cd OrganizaDinDin
```

### 2. Configure o Firebase

Crie um arquivo `firebase-credentials.json` na raiz do projeto com as credenciais do seu projeto Firebase.

Ou configure as variáveis de ambiente:

```bash
# Windows (PowerShell)
$env:FIREBASE_PROJECT_ID="seu-project-id"
$env:GOOGLE_APPLICATION_CREDENTIALS_JSON="conteúdo-do-json-de-credenciais"

# Linux/Mac
export FIREBASE_PROJECT_ID="seu-project-id"
export GOOGLE_APPLICATION_CREDENTIALS_JSON="conteúdo-do-json-de-credenciais"
```

### 3. Restaure as dependências

```bash
dotnet restore
```

### 4. Execute o projeto

```bash
dotnet run
```

O aplicativo estará disponível em `https://localhost:7262`.

## Funcionalidades Técnicas

### Validação Customizada
- Validação client-side sem HTML5 nativo
- Feedback visual instantâneo
- Mensagens de erro contextuais
- Validação apenas após interação do usuário

### Sistema de Autenticação
- Cookie-based authentication
- Sessão com expiração de 8 horas
- Sliding expiration habilitado
- Proteção de rotas com `[Authorize]`

### Responsividade
- Mobile-first approach
- Sidebar adaptável
- Tabelas responsivas com scroll horizontal

### Login
Interface de autenticação com validação customizada e design moderno.

### Dashboard de Gastos
Visualização completa dos gastos com opções de criar, editar e excluir.

### Resumo com Gráficos
Análise visual dos gastos por categoria com gráficos interativos.

## Padrões de Commit

Este projeto segue o padrão [Conventional Commits](https://www.conventionalcommits.org/)