# OrganizaDinDin

Aplicação web para organizar os gastos do apartamento e do casamento, acompanhar quanto falta pagar em cada conta e guardar dinheiro no cofrinho. Feita com ASP.NET Core MVC e Google Cloud Firestore.

## Funcionalidades

- **Gastos do apartamento e do casamento**
  - Cada área tem seus próprios tipos de gasto (entrada, evolução de obra, buffet, decoração…)
  - Filtros por descrição, tipo e período, com paginação
- **Contas e parcelas**
  - Uma conta pode ter valor total (ex.: buffet de R$ 33.000) e/ou data final (ex.: evolução de obra até a entrega)
  - Os pagamentos vinculados a uma conta são numerados automaticamente (3/34, 10/~30); os demais aparecem como parcela única
  - Painel “Quanto falta pagar” com o total geral e o progresso de cada conta
  - Sem valor total, o que falta é estimado pela média dos últimos pagamentos
- **Cofrinho**
  - Depósitos e resgates por pessoa, saldo atual e meta de economia
- **Usuários** (administradores)
  - Alterar permissão, ativar/desativar e redefinir senha
  - Novos cadastros só acessam o sistema depois que um administrador define a permissão

## Tecnologias

- .NET 9 / ASP.NET Core MVC com Razor
- Google Cloud Firestore
- Autenticação por cookie e senhas com BCrypt
- Bootstrap 5, Bootstrap Icons e JavaScript puro

## Estrutura

```
OrganizaDinDin/
├── Application/        # DTOs, filtros, interfaces e serviços
├── Controllers/        # Controllers MVC
├── Domain/             # Entidades, enums e interfaces de repositório
├── Helpers/            # Extensões e utilitários
├── Infrastructure/     # Repositórios do Firestore
├── Views/              # Telas Razor
└── wwwroot/            # CSS, JavaScript e bibliotecas
```

## Como rodar

Pré-requisitos: [.NET 9 SDK](https://dotnet.microsoft.com/download) e um projeto Firebase com Firestore.

1. Configure as credenciais do Firebase de uma destas formas:
   - arquivo `firebase-credentials.json` na raiz do projeto (já está no `.gitignore`); ou
   - variável de ambiente `GOOGLE_APPLICATION_CREDENTIALS_JSON` com o conteúdo do JSON.
2. Se usar outro projeto Firebase, defina `Firebase:ProjectId` no `appsettings.json` ou a variável `FIREBASE_PROJECT_ID`.
3. Rode:

```bash
dotnet run
```

O aplicativo fica disponível em `http://localhost:5097`.

> Atenção: rodando localmente com as credenciais de produção, o app lê e grava no banco de produção.

## Padrões de commit

Este projeto segue o padrão [Conventional Commits](https://www.conventionalcommits.org/).
