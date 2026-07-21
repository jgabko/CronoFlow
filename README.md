<div align="center">

  <h1>TimeFlow (CronoFlow)</h1>

  <p>
    Aplicativo desktop de cronoanálise corporativa para rastreamento de tempo
    e análise de desempenho de equipes. Interface inspirada no Clockify.
  </p>

<!-- Badges -->
<p>
  <a href="https://github.com/jgabko/CronoFlow/graphs/contributors">
    <img src="https://img.shields.io/github/contributors/jgabko/CronoFlow" alt="contributors" />
  </a>
  <a href="">
    <img src="https://img.shields.io/github/last-commit/jgabko/CronoFlow" alt="last update" />
  </a>
  <a href="https://github.com/jgabko/CronoFlow/network/members">
    <img src="https://img.shields.io/github/forks/jgabko/CronoFlow" alt="forks" />
  </a>
  <a href="https://github.com/jgabko/CronoFlow/stargazers">
    <img src="https://img.shields.io/github/stars/jgabko/CronoFlow" alt="stars" />
  </a>
  <a href="https://github.com/jgabko/CronoFlow/issues/">
    <img src="https://img.shields.io/github/issues/jgabko/CronoFlow" alt="open issues" />
  </a>
</p>

<h4>
    <a href="https://github.com/jgabko/CronoFlow/">Ver Demo</a>
  <span> · </span>
    <a href="https://github.com/jgabko/CronoFlow">Documentação</a>
  <span> · </span>
    <a href="https://github.com/jgabko/CronoFlow/issues/">Reportar Bug</a>
  <span> · </span>
    <a href="https://github.com/jgabko/CronoFlow/issues/">Solicitar Feature</a>
  </h4>
</div>

<br />

<!-- Table of Contents -->
# Índice

- [Sobre o Projeto](#sobre-o-projeto)
  * [Screenshots](#screenshots)
  * [Tech Stack](#tech-stack)
  * [Funcionalidades](#funcionalidades)
- [Getting Started](#getting-started)
  * [Pré-requisitos](#pré-requisitos)
  * [Executar](#executar)
  * [Contas de Demonstração](#contas-de-demonstração)
  * [Banco de Dados](#banco-de-dados)
- [Roadmap](#roadmap)
- [Licença](#licença)


<!-- About the Project -->
## Sobre o Projeto

O **TimeFlow** (repositório CronoFlow) é um aplicativo desktop de cronoanálise
corporativa, feito para rastrear tempo de trabalho e analisar o desempenho de
equipes, com uma interface inspirada no Clockify.

<!-- Screenshots -->
### Screenshots

<div align="center"> 
  <img src="https://placehold.co/600x400?text=Your+Screenshot+here" alt="screenshot" />
</div>

<!-- TechStack -->
### Tech Stack

<details>
  <summary>Aplicação</summary>
  <ul>
    <li><a href="https://dotnet.microsoft.com/">.NET 10</a></li>
    <li><a href="https://avaloniaui.net/">Avalonia UI</a> (cross-platform)</li>
    <li>MVVM (<a href="https://github.com/CommunityToolkit/dotnet">CommunityToolkit.Mvvm</a>)</li>
  </ul>
</details>

<details>
  <summary>Banco de Dados</summary>
  <ul>
    <li><a href="https://www.sqlite.org/">SQLite</a></li>
    <li><a href="https://learn.microsoft.com/ef/core/">Entity Framework Core</a></li>
  </ul>
</details>

<!-- Features -->
### Funcionalidades

- Login com controle de acesso (Gerente / Funcionário)
- Cronômetro com Iniciar, Pausar e Parar
- Mini-player **Always on Top**
- Recuperação de tempo após crash (timestamps persistidos)
- Dashboards de desempenho individual, por tarefa e da equipe

<!-- Getting Started -->
## Getting Started

<!-- Prerequisites -->
### Pré-requisitos

Este projeto usa **.NET 10** e o SDK correspondente instalado na máquina.

<!-- Run Locally -->
### Executar

Clone o projeto

```bash
git clone https://github.com/jgabko/CronoFlow.git
```

Vá até o diretório da aplicação

```bash
cd src/TimeFlow
```

Rode a aplicação

```bash
dotnet run
```

### Contas de Demonstração

| Papel       | Usuário | Senha      |
| ----------- | ------- | ---------- |
| Gerente     | gerente | gerente123 |
| Funcionário | joao    | joao123    |
| Funcionário | maria   | maria123   |

### Banco de Dados

O banco SQLite é criado automaticamente em:

```
~/.local/share/TimeFlow/timeflow.db
```

<!-- Roadmap -->
## Roadmap

* [x] Login com controle de acesso (Gerente / Funcionário)
* [x] Cronômetro (Iniciar / Pausar / Parar)
* [x] Mini-player Always on Top
* [x] Recuperação de tempo após crash
* [x] Dashboards de desempenho
<!--* [ ] Exportação de relatórios
* [ ] Suporte multi-empresa-->

<!-- License -->
## Licença

Distribuído sem licença definida. Veja LICENSE.txt para mais informações.
