# FiapSrvUser - API de Gerenciamento de Usuários

## 📖 Sobre o Projeto

**FiapSrvUser** é uma API RESTful desenvolvida em .NET 8, focada no gerenciamento de usuários para a plataforma de jogos. Ela é responsável por todas as operações relacionadas a perfis de usuários, incluindo visualização, atualização e exclusão, distinguindo entre os papéis de *Player* e *Publisher*.

O projeto foi estruturado seguindo os princípios da arquitetura limpa, garantindo a separação clara de responsabilidades entre as camadas de Domínio, Aplicação, Infraestrutura e API.

## ✨ Funcionalidades Principais

  - **Gerenciamento de Usuários**: Operações de CRUD (Leitura, Atualização e Exclusão) para os perfis de usuário.
  - **Sistema de Papéis (Roles)**: Distinção clara entre usuários do tipo `Player` e `Publisher`, com DTOs específicos para cada um.
  - **Segurança**: Proteção dos endpoints com autenticação baseada em JWT (JSON Web Tokens) e autorização por papéis.
  - **Infraestrutura em Nuvem**: Integração com AWS para gerenciamento de configurações e segurança.
  - **Middleware Customizado**: Tratamento centralizado de exceções e injeção de `Correlation ID` para rastreabilidade de logs.

## 🚀 Tecnologias Utilizadas

  - **.NET 8**: Framework principal para a construção da API.
  - **ASP.NET Core**: Para a criação da API RESTful.
  - **MongoDB**: Banco de dados NoSQL para a persistência dos dados de usuários.
  - **AWS (Amazon Web Services)**:
      - **Parameter Store**: Para gerenciamento seguro de `secrets` (como connection strings e chaves JWT) em ambiente de produção.
      - **S3 (Simple Storage Service)**: Para persistência das chaves de criptografia do Data Protection.
      - **ECS (Elastic Container Service)**: Utilizado para a orquestração dos contêineres em produção.
  - **Docker**: Para a containerização da aplicação.
  - **Serilog**: Para logging estruturado e de fácil rastreabilidade.
  - **Swagger (OpenAPI)**: Para documentação e teste interativo dos endpoints da API.
  - **xUnit & Moq**: Para a criação de testes unitários robustos.

## 🏗️ Arquitetura

O projeto é dividido em 4 camadas principais, promovendo um design modular, testável e de fácil manutenção:

  - **`FiapSrvUser.Domain`**: Camada central que contém as entidades de negócio (`User`, `Player`, `Publisher`) e enums. Não possui dependências de outras camadas.
  - **`FiapSrvUser.Application`**: Contém a lógica de negócio da aplicação. Define os DTOs, as interfaces de serviços e repositórios, e as classes de serviço que orquestram as operações.
  - **`FiapSrvUser.Infrastructure`**: Implementa as interfaces definidas na camada de Aplicação. É responsável pelo acesso ao banco de dados (repositório MongoDB), configuração de middlewares e comunicação com serviços externos como a AWS.
  - **`FiapSrvUser.API`**: Camada de entrada da aplicação. Expõe os endpoints RESTful, gerencia as requisições HTTP, e lida com a autenticação e autorização dos usuários.

## ⚙️ CI/CD - Integração e Implantação Contínua

O projeto utiliza **GitHub Actions** para automatizar o ciclo de vida da aplicação, desde o build até o deploy em produção.

1.  **Orquestrador (`ci-cd.yml`)**: Dispara o pipeline sempre que há um push ou um pull request é fechado na branch `main`.
2.  **CI (`ci.yml`)**:
      - Realiza o checkout do código.
      - Configura o ambiente .NET.
      - Executa o build da solução.
      - Roda os testes unitários e coleta a cobertura de código.
      - Envia os resultados para análise de qualidade e segurança no **SonarCloud**.
3.  **CD (`cd.yml`)**:
      - Após o sucesso da etapa de CI, faz o login no Docker Hub.
      - Constrói a imagem Docker da aplicação.
      - Publica a imagem no **Docker Hub** com a tag do commit e a `latest`.
4.  **Deploy (`deploy-aws.yml`)**:
      - Autentica-se na AWS.
      - Faz o download da definição de tarefa (task definition) do **AWS ECS**.
      - Atualiza a task definition com a nova imagem Docker.
      - Realiza o deploy da nova versão no serviço do ECS, garantindo a atualização sem interrupção do serviço.

## Endpoints da API

Abaixo estão os principais endpoints disponíveis. Para detalhes sobre os `requests` e `responses`, consulte a documentação do Swagger (`/swagger`).

### Users (`/api/users`)

  - `GET /`: Retorna uma lista de todos os usuários (`Player` e `Publisher`).
  - `GET /{id}`: Retorna um usuário específico pelo seu ID.
  - `PATCH /`: Atualiza o nome e username do usuário autenticado.
  - `DELETE /{id}`: Deleta o usuário autenticado.
  - `GET /players`: Retorna uma lista com todos os usuários do tipo `Player`.
  - `GET /publishers`: Retorna uma lista com todos os usuários do tipo `Publisher`.

*Nota: Todos os endpoints requerem autenticação.*

## 🏁 Como Executar Localmente

### Pré-requisitos

  - [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
  - [Docker Desktop](https://www.docker.com/products/docker-desktop)
  - Um editor de código de sua preferência (ex: VS Code, Visual Studio).

### 1\. Configuração do Ambiente

1.  **Clone o repositório:**

    ```bash
    git clone https://github.com/jpedroduarte23/fiap-srv-user.git
    cd fiap-srv-user
    ```

2.  **Inicie o MongoDB com Docker:**

    ```bash
    docker run -d -p 27017:27017 --name mongo mongo:latest
    ```

### 2\. Configuração da Aplicação

1.  **Configure a Connection String**:
    No arquivo `FiapSrvUser.API/appsettings.Development.json`, certifique-se de que a connection string do MongoDB está configurada corretamente:

    ```json
    "ConnectionStrings": {
      "MongoDbConnection": "mongodb://localhost:27017/"
    }
    ```

2.  **Restaure as dependências e execute a aplicação**:
    Navegue até a pasta raiz do projeto e execute o seguinte comando:

    ```bash
    dotnet run --project FiapSrvUser.API/FiapSrvUser.API.csproj
    ```

3.  **Acesse a API**:
    A aplicação estará disponível em `https://localhost:7155` ou `http://localhost:5234`.
    A documentação do Swagger pode ser acessada através da URL `https://localhost:7155/swagger`.
