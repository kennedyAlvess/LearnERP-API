# Contribuindo com Issues no LearnERP-API

Para manter o desenvolvimento incremental do MVP ERP B2B, abra issues usando os templates em `.github/ISSUE_TEMPLATE`:

- `feature_request.yml`: novas features/user stories.
- `bug_report.yml`: defeitos e regressões.
- `task.yml`: tarefas técnicas com escopo pequeno e objetivo.

## Boas práticas (curto e direto)

1. **Defina o porquê de negócio** antes do como técnico.
2. **Mantenha escopo pequeno** (1 problema por issue).
3. Preencha sempre:
   - regras de negócio e exceções;
   - contratos de API (endpoints, payloads, erros, idempotência);
   - entidades/dados impactados;
   - checklist multi-tenant;
   - segurança (RBAC/policies + auditoria);
   - critérios de aceite em **Given/When/Then**;
   - dependências, riscos e prioridade (P0/P1/P2) com justificativa.
4. Marque módulo e sprint sugeridos para facilitar planejamento:
   - Plataforma, Cadastros, Comercial, Estoque, Compras, Expedição/Logística, Financeiro, Relatórios.

Se a demanda ainda estiver vaga, use Discussions para refinamento e só então abra a issue formal.
