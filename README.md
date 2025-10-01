# TrabalhoRedesP2P

## 💡 Sobre o Projeto

Este projeto implementa uma solução de **rede Ponto a Ponto (P2P)** básica em C\# para demonstrar os princípios de comunicação e descoberta de nós em redes descentralizadas.

O problema principal abordado é a **descoberta de serviços e a comunicação direta** entre múltiplos *peers* (pares), eliminando a necessidade de um servidor central. Cada nó (peer) é capaz de atuar tanto como **cliente** (iniciando conexões) quanto como **servidor** (aceitando conexões), criando uma topologia de rede robusta e resiliente.

A comunicação entre os nós é gerenciada através de *sockets* TCP, garantindo a entrega confiável das mensagens, que neste projeto são usadas para compartilhar listas de *peers* conhecidos e, consequentemente, expandir o conhecimento da rede.

## 💻 Tecnologia Utilizada

  * **Linguagem de Programação:** **C\#** (necessita do **.NET** Runtime ou SDK).
  * **Framework:** **.NET 6.0** (ou superior).
  * **Contêineres:** **Docker** e **Docker Compose** para orquestração de múltiplos nós.

-----

## 🛠️ Pré-requisitos de Instalação

Para executar e testar o projeto, você precisará ter instalado em sua máquina:

1.  **.NET SDK (6.0 ou superior):** Necessário para compilar e rodar o projeto localmente.
2.  **Docker Desktop:** Necessário para construir e executar os contêineres e o ambiente com `compose.yaml`.

-----

## ▶️ Como Executar o Projeto

Existem duas formas principais de executar e testar a rede P2P: de forma **Local** e usando **Docker Compose**.

### 1\. Execução Local

A execução local simula a rede em sua própria máquina, usando arquivos de configuração locais que apontam para `localhost`.

#### Comando de Execução

Você deve abrir **múltiplos terminais** (um para cada *peer*) e executar o projeto, passando o **ID do *peer*** e o **arquivo de configuração** correspondente:

| Terminal | Comando | Descrição |
| :--- | :--- | :--- |
| **Peer 1** | `dotnet run -- 1 knownPeers/knownPeers1_local.txt` | Inicializa o Peer 1 na porta 8001. |
| **Peer 2** | `dotnet run -- 2 knownPeers/knownPeers2_local.txt` | Inicializa o Peer 2 na porta 8002. |
| **Peer 3** | `dotnet run -- 3 knownPeers/knownPeers3_local.txt` | Inicializa o Peer 3 na porta 8003. |
| **Peer N** | `dotnet run -- [ID] knownPeers/knownPeers[ID]_local.txt` | Siga o padrão para mais peers. |

### 2\. Execução com Docker Compose

O Docker Compose é a maneira **recomendada** para testar a rede, pois ele configura um ambiente isolado onde cada *peer* pode se referenciar usando nomes de serviço, como se fossem máquinas separadas.

#### Comandos de Execução

1.  **Construir as imagens e subir os serviços:**

    ```bash
    docker-compose up --build
    ```

    *Este comando irá criar os contêineres (Peer 1, Peer 2, etc.) e iniciará a comunicação da rede P2P de acordo com o `compose.yaml`.*

2.  **Visualizar os logs (opcional):**

    ```bash
    docker-compose logs -f
    ```

    *Use este comando em um terminal separado para acompanhar a troca de mensagens e a descoberta de peers em tempo real.*

3.  **Encerrar e limpar o ambiente:**

    ```bash
    docker-compose down
    ```

    *Este comando interrompe e remove os contêineres e a rede criada.*
    
