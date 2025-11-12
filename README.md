# Developer Evaluation Project

## Review após implementação
Foi ajustado o projeto para funcionar no ambiente do MacOS, mais detalhes em [](./docs/linux-or-mac-local-setup.md).
Nota-se que o domínio de Carts foi implementado e posteriormente de Sales, talvez tenha entendido errado a motivação de ambos, mas 
o desejado era que pudesse criar um Sale a partir de um Cart, ou seja, o Cart seria o "carrinho de compras" e o Sale a finalização da compra.

Dos detalhes de implementação, foi observado durante o desenvolvimento, mas descoberto depois algumas características do repositório, como
maior parte do código já seguia um padrão, como, por exemplo, aproveitar mais as validações das entidades do domínio.
Também notei que no código já de exemplo algumas operações de leitura também eram chamadas de command, segui o padrão já existente.

Algumas decisões foram tomadas para acelerar o desenvolvimento, dado o tempo que tenho disponível, não sendo estas negligenciadas por desconhecimento, mas propositalmente dado que o objetivo é entregar um projeto funcional e que demonstre as habilidades técnicas:
- Não implementei todos os filtros de listagem para todos os domínios, embora tenha feito uma implementação de Critérios de filtro agnóstica (CriteriaFilters).
- Não me preocupei com projeção dos dados armazenados usando cache, por exemplo.
- Não foi feito integração com Message Broker, apenas loguei as mensagens, foi implementado um Dispatcher em memória, que permite não só tratar o efeito colateral,
como uma operação de limpar cache sem depender de um recurso externo. Tento a flexibilidade de fazer o `forward` com um Handler para um broker externo.
- Dado ao tempo não foi feito outro tipo de teste salvo de unidade, mas o desejado seria implementar um e2e com playwright usando o SDK de APIs.
- Propositalmente não tratei identificadores como Value Objects, para acelerar o desenvolvimento, mas em um cenário real isso seria interessante.
- Também geralmetne prefiro tratar ValueObjects com tipos que garantem a imutabilidade, como records, 
mas para acelerar o desenvolvimento usei classes normais para evitar o overhead de criação de construtores e integração com EF Core.
- Não implementei versionamento de API ou Rate Limit mas em um cenário real isso seria interessante.
- Também não foi tratado com tanto afinco a documentação do OpenAPI (Swagger).
- A padrão tático `Serviço de Domínio` foi utilizado em pontos onde a lógica de negócio necessitou interagir com múltiplas entidades ou agregar regras que não se encaixavam diretamente em uma única entidade, como o cálculo de descontos em vendas.
---

`READ CAREFULLY`
## Instructions
**The test below will have up to 7 calendar days to be delivered from the date of receipt of this manual.**

- The code must be versioned in a public Github repository and a link must be sent for evaluation once completed
- Upload this template to your repository and start working from it
- Read the instructions carefully and make sure all requirements are being addressed
- The repository must provide instructions on how to configure, execute and test the project
- Documentation and overall organization will also be taken into consideration

## Use Case
**You are a developer on the DeveloperStore team. Now we need to implement the API prototypes.**

As we work with `DDD`, to reference entities from other domains, we use the `External Identities` pattern with denormalization of entity descriptions.

Therefore, you will write an API (complete CRUD) that handles sales records. The API needs to be able to inform:

* Sale number
* Date when the sale was made
* Customer
* Total sale amount
* Branch where the sale was made
* Products
* Quantities
* Unit prices
* Discounts
* Total amount for each item
* Cancelled/Not Cancelled

It's not mandatory, but it would be a differential to build code for publishing events of:
* SaleCreated
* SaleModified
* SaleCancelled
* ItemCancelled

If you write the code, **it's not required** to actually publish to any Message Broker. You can log a message in the application log or however you find most convenient.

### Business Rules

* Purchases above 4 identical items have a 10% discount
* Purchases between 10 and 20 identical items have a 20% discount
* It's not possible to sell above 20 identical items
* Purchases below 4 items cannot have a discount

These business rules define quantity-based discounting tiers and limitations:

1. Discount Tiers:
   - 4+ items: 10% discount
   - 10-20 items: 20% discount

2. Restrictions:
   - Maximum limit: 20 items per product
   - No discounts allowed for quantities below 4 items

## Overview
This section provides a high-level overview of the project and the various skills and competencies it aims to assess for developer candidates. 

See [Overview](/.doc/overview.md)

## Tech Stack
This section lists the key technologies used in the project, including the backend, testing, frontend, and database components. 

See [Tech Stack](/.doc/tech-stack.md)

## Frameworks
This section outlines the frameworks and libraries that are leveraged in the project to enhance development productivity and maintainability. 

See [Frameworks](/.doc/frameworks.md)

<!-- 
## API Structure
This section includes links to the detailed documentation for the different API resources:
- [API General](./docs/general-api.md)
- [Products API](/.doc/products-api.md)
- [Carts API](/.doc/carts-api.md)
- [Users API](/.doc/users-api.md)
- [Auth API](/.doc/auth-api.md)
-->

## Project Structure
This section describes the overall structure and organization of the project files and directories. 

See [Project Structure](/.doc/project-structure.md)