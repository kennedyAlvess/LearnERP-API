# Sprint 1 Backlog — Drafts de Issues (Plataforma + Cadastros)

> Contexto: repositório `kennedyAlvess/LearnERP-API`, base branch `main`.
> Observação: usar labels quando disponíveis: `sprint-1`, `P0`, `module:platform`/`module:cadastros`, `multi-tenant`, `security`, `testing`.

---

## S1-01 Estrutura base da solução (.NET 8) + Health Check
**Prioridade:** P0  
**Labels sugeridas:** sprint-1, P0, module:platform, testing

### Descrição (o que/por quê)
Criar a fundação da API em .NET 8 com estrutura limpa para habilitar evolução incremental e testes desde o início.

### Regras de negócio / exceções
- Projeto inicial deve expor `GET /health` sem dependências de negócio.
- O endpoint de health não deve retornar dados sensíveis.

### Contrato de API
- `GET /health` → `200 OK` `{ "status": "ok" }`
- Erros: `503` se aplicação indisponível.
- Idempotência: não aplicável (GET).

### Modelo de dados / entidades impactadas
- Nenhuma entidade de domínio nesta issue.

### Multi-tenant
- Preparar pipeline para `TenantContext` futuro, sem aplicar regra de tenant no health.
- Logs do health devem suportar correlação futura.

### Segurança
- Health público no MVP.
- Sem autenticação obrigatória para `/health`.

### Aceitação (Given/When/Then)
- **Given** API em execução **When** chamar `/health` **Then** retorna 200 com status ok.
- **Given** erro interno de bootstrap **When** chamar `/health` **Then** retorna 503.

### Dependências / riscos
- Dependências: nenhuma.
- Risco: baixo.

---

## S1-02 Autenticação JWT + TenantContext via claim tenant_id
**Prioridade:** P0  
**Labels sugeridas:** sprint-1, P0, module:platform, multi-tenant, security, testing

### Descrição (o que/por quê)
Implementar autenticação JWT e resolução do tenant por claim `tenant_id` para isolar dados desde a camada de aplicação.

### Regras de negócio / exceções
- Requisição autenticada deve conter `tenant_id`.
- Ausência de `tenant_id` em endpoint protegido deve falhar.
- `tenant_id` inválido/formato incorreto deve falhar.

### Contrato de API
- Endpoints protegidos exigem `Authorization: Bearer <jwt>`.
- Erros:
  - `401` token ausente/inválido.
  - `403` token válido sem claim `tenant_id`.
- Idempotência: não aplicável.

### Modelo de dados / entidades impactadas
- `TenantContext` por request (infra/aplicação).

### Multi-tenant
- Tenant sempre resolvido de `tenant_id`.
- Nenhum dado deve ser consultado sem TenantContext resolvido.

### Segurança
- Validar assinatura/issuer/audience do JWT.
- Claims mínimas: `sub`, `tenant_id`, `role`.
- Registrar tentativa inválida em auditoria de segurança/log.

### Aceitação (Given/When/Then)
- **Given** token sem `tenant_id` **When** acessar endpoint protegido **Then** retorna 403.
- **Given** token com `tenant_id` válido **When** acessar endpoint protegido **Then** TenantContext é resolvido.

### Dependências / riscos
- Dependências: S1-01.
- Risco: alto (base de isolamento).

---

## S1-03 ProblemDetails global + validação de payload
**Prioridade:** P0  
**Labels sugeridas:** sprint-1, P0, module:platform, testing

### Descrição (o que/por quê)
Padronizar erros com ProblemDetails e validações de payload para melhorar integração e previsibilidade da API.

### Regras de negócio / exceções
- Toda falha de validação retorna formato único.
- Não expor stack trace em produção.

### Contrato de API
- Erros no padrão RFC 7807:
  - `400` validação
  - `401/403` autenticação/autorização
  - `404` recurso não encontrado
  - `409` conflito de negócio
  - `500` erro interno
- Idempotência: não aplicável.

### Modelo de dados / entidades impactadas
- Sem alteração de entidades de domínio.

### Multi-tenant
- Mensagens de erro não devem vazar dados de outros tenants.

### Segurança
- Sanitizar mensagens técnicas.
- Rastrear erro com CorrelationId.

### Aceitação (Given/When/Then)
- **Given** payload inválido **When** chamar endpoint **Then** retorna 400 ProblemDetails com erros por campo.
- **Given** exceção não tratada **When** chamar endpoint **Then** retorna 500 ProblemDetails padronizado.

### Dependências / riscos
- Dependências: S1-01.
- Risco: médio.

---

## S1-04 RBAC (roles) + policies mínimas por módulo
**Prioridade:** P0  
**Labels sugeridas:** sprint-1, P0, module:platform, security, testing

### Descrição (o que/por quê)
Definir autorização por papéis/policies mínimas para separar responsabilidades (Admin, Vendas, Estoque, Financeiro).

### Regras de negócio / exceções
- Deny-by-default para endpoints protegidos.
- `Admin` com acesso completo no MVP.
- Policies por módulo (platform/cadastros).

### Contrato de API
- Endpoints retornam `403` quando role/policy não atende.
- Idempotência: não aplicável.

### Modelo de dados / entidades impactadas
- Sem persistência de permissões nesta sprint (policies em configuração).

### Multi-tenant
- RBAC deve operar após tenant resolvido.
- Acesso permitido nunca rompe isolamento por tenant.

### Segurança
- Validar roles no token.
- Log de negação com `sub`, `tenant_id`, policy.

### Aceitação (Given/When/Then)
- **Given** usuário role Vendas **When** acessa endpoint restrito de estoque write **Then** retorna 403.
- **Given** usuário Admin **When** acessa endpoint protegido **Then** autorizado.

### Dependências / riscos
- Dependências: S1-02.
- Risco: médio.

---

## S1-05 Persistência: EF Core + SQL Server + migrations + TenantId base
**Prioridade:** P0  
**Labels sugeridas:** sprint-1, P0, module:platform, multi-tenant, testing

### Descrição (o que/por quê)
Implementar infraestrutura de persistência com EF Core/SQL Server e base de TenantId em entidades tenant-aware.

### Regras de negócio / exceções
- Entidades tenant-aware devem conter `TenantId` obrigatório.
- Migrações versionadas e reproduzíveis.

### Contrato de API
- Não adiciona endpoint novo diretamente.
- Erros de persistência devem mapear para ProblemDetails apropriado.

### Modelo de dados / entidades impactadas
- Base entity tenant-aware (`Id`, `TenantId`, auditoria básica).
- Contexto EF + primeira migration.

### Multi-tenant
- Estrutura pronta para filtros por tenant.
- Índices com `TenantId` em entidades relevantes.

### Segurança
- String de conexão por configuração segura.
- Não logar segredos.

### Aceitação (Given/When/Then)
- **Given** ambiente limpo **When** aplicar migration **Then** schema criado com TenantId nas entidades base.
- **Given** entidade sem TenantId **When** persistir **Then** operação falha por validação/restrição.

### Dependências / riscos
- Dependências: S1-01, S1-02.
- Risco: alto.

---

## S1-06 Filtro global por TenantId + testes de isolamento (2 tenants)
**Prioridade:** P0  
**Labels sugeridas:** sprint-1, P0, module:platform, multi-tenant, testing

### Descrição (o que/por quê)
Garantir isolamento automático entre tenants com filtro global de consultas e testes de integração com dois tenants.

### Regras de negócio / exceções
- Toda query de entidade tenant-aware deve ser filtrada por TenantId corrente.
- Operações cross-tenant devem retornar não encontrado/negado sem vazamento.

### Contrato de API
- Endpoints de listagem/consulta retornam apenas dados do tenant corrente.
- Erros: `404` para recurso de outro tenant.

### Modelo de dados / entidades impactadas
- Configuração de global query filters no EF.
- Testes de integração para isolamento.

### Multi-tenant
- Isolamento por `tenant_id` obrigatório em leitura e escrita.
- Logs/auditoria incluem tenant da operação.

### Segurança
- Prevenção de IDOR cross-tenant.

### Aceitação (Given/When/Then)
- **Given** dados no tenant A e B **When** usuário do A lista recursos **Then** recebe só dados do A.
- **Given** usuário do A consulta ID do B **When** GET por ID **Then** recebe 404.

### Dependências / riscos
- Dependências: S1-05.
- Risco: alto crítico.

---

## S1-07 Auditoria automática (Created/Updated/Deleted + UserId)
**Prioridade:** P0  
**Labels sugeridas:** sprint-1, P0, module:platform, security, testing

### Descrição (o que/por quê)
Adicionar trilha automática de auditoria para rastreabilidade operacional e segurança.

### Regras de negócio / exceções
- Preencher automaticamente: `CreatedAt/By`, `UpdatedAt/By`, `DeletedAt/By`.
- `UserId` vem da claim `sub`.

### Contrato de API
- Não requer endpoint específico nesta issue.
- Erros de auditoria devem ser tratados sem interromper operação válida (quando possível).

### Modelo de dados / entidades impactadas
- Base auditable entity para entidades de cadastro/plataforma.

### Multi-tenant
- Auditoria deve registrar também `TenantId`.

### Segurança
- Trilha de auditoria imutável para eventos principais (ao menos por aplicação na sprint).

### Aceitação (Given/When/Then)
- **Given** criação de entidade **When** salvar **Then** CreatedAt/By preenchidos.
- **Given** atualização/exclusão lógica **When** salvar **Then** Updated/Deleted preenchidos com UserId.

### Dependências / riscos
- Dependências: S1-05, S1-02.
- Risco: médio.

---

## S1-08 Observabilidade mínima: Correlation-Id + logs estruturados
**Prioridade:** P0  
**Labels sugeridas:** sprint-1, P0, module:platform, testing

### Descrição (o que/por quê)
Adicionar correlação de requisições e logs estruturados para troubleshooting e suporte.

### Regras de negócio / exceções
- Reutilizar `X-Correlation-Id` se enviado; caso contrário gerar.
- Retornar `X-Correlation-Id` em todas as respostas.

### Contrato de API
- Header de entrada/saída: `X-Correlation-Id`.
- Idempotência: não aplicável.

### Modelo de dados / entidades impactadas
- Sem impacto em entidades de domínio.

### Multi-tenant
- Cada log deve incluir `tenant_id` (quando autenticado).

### Segurança
- Não logar payloads sensíveis/credenciais.
- Logs de erro com nível e contexto mínimo seguro.

### Aceitação (Given/When/Then)
- **Given** request sem correlation id **When** chamar endpoint **Then** resposta contém id gerado.
- **Given** request com correlation id **When** chamar endpoint **Then** resposta preserva mesmo id.

### Dependências / riscos
- Dependências: S1-01, S1-02.
- Risco: baixo.

---

## S1-09 Depósitos (CRUD) com regra 1..3 e default por tenant
**Prioridade:** P0  
**Labels sugeridas:** sprint-1, P0, module:cadastros, multi-tenant, security, testing

### Descrição (o que/por quê)
Implementar cadastro de depósitos por tenant com limite de 1 a 3 depósitos e um depósito padrão por tenant.

### Regras de negócio / exceções
- Mínimo operacional: 1 depósito ativo por tenant.
- Máximo: 3 depósitos por tenant.
- Exatamente 1 depósito padrão ativo por tenant.
- Não permitir excluir/inativar único depósito ativo.

### Contrato de API
- `POST /v1/depositos`
- `GET /v1/depositos`
- `GET /v1/depositos/{id}`
- `PUT /v1/depositos/{id}`
- `DELETE /v1/depositos/{id}` (preferir inativação lógica)
- Erros:
  - `409` ao exceder limite 3.
  - `400` ao tentar estado inválido de default.
  - `404` para recurso inexistente/no outro tenant.
- Idempotência: POST não idempotente.

### Modelo de dados / entidades impactadas
- `Deposito { Id, TenantId, Nome, Ativo, IsDefault, Audit... }`
- Índice único por tenant para default ativo.

### Multi-tenant
- Tenant via JWT `tenant_id`.
- Filtro por tenant em todas consultas.
- Auditoria e logs com tenant.

### Segurança
- RBAC: write para Estoque/Admin; read conforme policy.
- Auditoria de mudanças de default.

### Aceitação (Given/When/Then)
- **Given** tenant com 3 depósitos **When** criar 4º **Then** retorna 409.
- **Given** novo default definido **When** salvar **Then** antigo default é removido automaticamente.

### Dependências / riscos
- Dependências: S1-04, S1-05, S1-06, S1-07.
- Risco: médio.

---

## S1-10 Clientes (CRUD) com score, crédito calculado e override + bloqueio + depósito padrão opcional
**Prioridade:** P0  
**Labels sugeridas:** sprint-1, P0, module:cadastros, multi-tenant, security, testing

### Descrição (o que/por quê)
Criar cadastro de clientes com score (0..1000), crédito calculado por faixa, override opcional, bloqueio e depósito padrão opcional.

### Regras de negócio / exceções
- `Score` obrigatório entre 0 e 1000.
- Crédito calculado por faixas simples (config fixa na sprint):
  - 0–499: 5.000
  - 500–699: 20.000
  - 700–799: 50.000
  - 800–899: 100.000
  - 900–1000: 200.000
- `CreditLimitOverride` (se informado) substitui calculado.
- `IsBlocked=true` marca cliente indisponível para venda futura.
- `DefaultDepositoId` opcional deve pertencer ao mesmo tenant.

### Contrato de API
- `POST /v1/clientes`
- `GET /v1/clientes`
- `GET /v1/clientes/{id}`
- `PUT /v1/clientes/{id}`
- `PATCH /v1/clientes/{id}/bloqueio`
- Erros:
  - `400` score inválido.
  - `400` depósito padrão de outro tenant.
  - `404` recurso não encontrado.
- Idempotência: não aplicável.

### Modelo de dados / entidades impactadas
- `Cliente { Id, TenantId, Nome, Documento, Score, CreditLimitOverride, IsBlocked, DefaultDepositoId?, Audit... }`

### Multi-tenant
- Isolamento completo por tenant.
- Validação de FK lógica para depósito no mesmo tenant.
- Logs/auditoria com tenant.

### Segurança
- RBAC: Vendas/Admin write; Financeiro para ajustes de crédito/bloqueio (policy dedicada).

### Aceitação (Given/When/Then)
- **Given** score fora de faixa **When** criar cliente **Then** retorna 400.
- **Given** override informado **When** consultar cliente **Then** crédito efetivo = override.
- **Given** depósito de outro tenant **When** vincular no cliente **Then** retorna 400.

### Dependências / riscos
- Dependências: S1-09.
- Risco: médio.

---

## S1-11 Produtos (CRUD) + SKU único por tenant
**Prioridade:** P0  
**Labels sugeridas:** sprint-1, P0, module:cadastros, multi-tenant, testing

### Descrição (o que/por quê)
Implementar cadastro de produtos para suportar precificação e pedidos futuros, garantindo SKU único por tenant.

### Regras de negócio / exceções
- SKU obrigatório e único por tenant.
- Produto pode ser ativado/inativado.

### Contrato de API
- `POST /v1/produtos`
- `GET /v1/produtos`
- `GET /v1/produtos/{id}`
- `PUT /v1/produtos/{id}`
- Erros:
  - `409` SKU duplicado no mesmo tenant.
  - `404` recurso inexistente.
- Idempotência: não aplicável.

### Modelo de dados / entidades impactadas
- `Produto { Id, TenantId, Sku, Nome, Unidade, Ativo, Audit... }`
- Índice único `(TenantId, Sku)`.

### Multi-tenant
- Filtro obrigatório por tenant em listagem e leitura.

### Segurança
- RBAC: write para Admin/Estoque; read para Vendas.

### Aceitação (Given/When/Then)
- **Given** SKU já existente no tenant **When** criar produto **Then** retorna 409.
- **Given** mesmo SKU em tenant diferente **When** criar **Then** permitido.

### Dependências / riscos
- Dependências: S1-04, S1-05, S1-06.
- Risco: baixo.

---

## S1-12 Taxa regional por UF (CRUD simples)
**Prioridade:** P0  
**Labels sugeridas:** sprint-1, P0, module:cadastros, multi-tenant, testing

### Descrição (o que/por quê)
Criar cadastro de taxa regional por UF (percentual) para cálculo de encargo de transporte por pedido.

### Regras de negócio / exceções
- 1 taxa por UF por tenant.
- `RatePercent` entre 0 e 100.
- UF deve ser válida (2 letras).

### Contrato de API
- `POST /v1/taxas-regionais`
- `GET /v1/taxas-regionais`
- `PUT /v1/taxas-regionais/{uf}`
- `DELETE /v1/taxas-regionais/{uf}`
- Erros:
  - `400` UF/rate inválidos.
  - `409` duplicidade por UF.
- Idempotência: `PUT` idempotente.

### Modelo de dados / entidades impactadas
- `TaxaRegionalUf { Id, TenantId, Uf, RatePercent, Audit... }`
- Índice único `(TenantId, Uf)`.

### Multi-tenant
- Isolamento por tenant.

### Segurança
- RBAC: write Admin; read Vendas/Admin.

### Aceitação (Given/When/Then)
- **Given** UF já cadastrada no tenant **When** criar novamente **Then** retorna 409.
- **Given** `PUT` mesma UF/mesmo valor **When** repetir requisição **Then** resultado permanece estável.

### Dependências / riscos
- Dependências: S1-05, S1-06.
- Risco: baixo.

---

## S1-13 Desconto por faixa (CRUD) com validação de sobreposição
**Prioridade:** P0  
**Labels sugeridas:** sprint-1, P0, module:cadastros, multi-tenant, testing

### Descrição (o que/por quê)
Cadastrar faixas de desconto por valor bruto do pedido, com validação de sobreposição para consistência de regras comerciais.

### Regras de negócio / exceções
- Faixas não podem se sobrepor no mesmo tenant.
- `DiscountPercent` entre 0 e 100.
- Regra de aplicação futura: só para clientes com score bom (>=700).

### Contrato de API
- `POST /v1/descontos-faixa`
- `GET /v1/descontos-faixa`
- `PUT /v1/descontos-faixa/{id}`
- `DELETE /v1/descontos-faixa/{id}`
- Erros:
  - `409` faixa sobreposta.
  - `400` percentuais/limites inválidos.
- Idempotência: `PUT` idempotente.

### Modelo de dados / entidades impactadas
- `DescontoFaixa { Id, TenantId, MinGross, MaxGross, DiscountPercent, Audit... }`

### Multi-tenant
- Sobreposição validada apenas dentro do tenant.

### Segurança
- RBAC: write Admin; read Vendas/Admin.
- Auditoria de alterações em faixas.

### Aceitação (Given/When/Then)
- **Given** faixa 10k–20k existente **When** criar 15k–25k **Then** retorna 409.
- **Given** faixa válida sem sobreposição **When** salvar **Then** persiste com sucesso.

### Dependências / riscos
- Dependências: S1-05, S1-06.
- Risco: médio.

---

## S1-14 Tabela de preço base (Cliente+Produto) com upsert
**Prioridade:** P0  
**Labels sugeridas:** sprint-1, P0, module:cadastros, multi-tenant, testing

### Descrição (o que/por quê)
Implementar tabela de preço base por combinação Cliente+Produto com operação de upsert para simplificar manutenção comercial no MVP.

### Regras de negócio / exceções
- Chave única: Cliente+Produto por tenant.
- `UnitPrice` > 0.
- Cliente e produto devem existir no mesmo tenant.

### Contrato de API
- `PUT /v1/tabela-preco-base` (upsert por `clientId` + `productId`)
- `GET /v1/tabela-preco-base?clientId={id}`
- Erros:
  - `400` preço inválido.
  - `404` cliente/produto inexistente no tenant.
  - `409` conflito de concorrência (quando aplicável).
- Idempotência: `PUT` idempotente.

### Modelo de dados / entidades impactadas
- `TabelaPrecoBaseItem { Id, TenantId, ClientId, ProductId, UnitPrice, Audit... }`
- Índice único `(TenantId, ClientId, ProductId)`.

### Multi-tenant
- Validação de pertencimento de client/produto ao tenant corrente.
- Query sempre filtrada por tenant.

### Segurança
- RBAC: write Admin (ou policy comercial gerente); read Vendas/Admin.
- Auditoria para alterações de preço.

### Aceitação (Given/When/Then)
- **Given** item inexistente **When** `PUT` válido **Then** cria novo preço.
- **Given** item existente **When** `PUT` com novo valor **Then** atualiza mesmo registro.
- **Given** client de outro tenant **When** upsert **Then** retorna 404 sem vazamento.

### Dependências / riscos
- Dependências: S1-10, S1-11.
- Risco: médio.

---

## Checklist de criação no GitHub (main)
Para cada issue acima, criar em `kennedyAlvess/LearnERP-API` com referência explícita à base branch `main` no corpo, por exemplo:

`Base branch de referência para desenvolvimento: main`
