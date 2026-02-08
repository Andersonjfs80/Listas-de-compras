# 📘 Escopo do Projeto

---

## 1. O que será feito (In-Scope)

- **Frontend Web (Angular 21)**: Painel administrativo e visualização de listas.
- **Frontend Mobile (Ionic 8)**: Aplicativo para iOS e Android com foco em usabilidade móvel.
- **Backend (.NET 8)**: microserviços independentes para Autenticação, Produtos e Segurança.
- **Gateway**: Ponto de entrada único para o ecossistema.
- **Integrações**: SMS (Twilio/AWS), WhatsApp (Meta Cloud API).
- **Banco de Dados**: SQL Server para persistência e Redis para cache.
- **DevOps**: Arquivos Dockerfile e docker-compose para ambiente local e produção.

## 2. O que NÃO será feito (Out-of-Scope)

- Sistemas de pagamento direto no app (v1.0).
- Gestão de estoque para supermercados (B2B).
- Suporte para versões de browsers legados (IE11).

## 3. Requisitos Funcionais (RF)

- [RF01] O usuário deve poder se logar via E-mail, CPF ou Nickname.
- [RF02] O sistema deve exibir carrosséis de ofertas na home.
- [RF03] O sistema deve permitir a validação de identidade via SMS.
- [RF04] Listagem de produtos com imagens e categorias.

## 4. Requisitos Não Funcionais (RNF)

- [RNF01] O sistema deve ser responsivo.
- [RNF02] O tempo de resposta das APIs não deve exceder 200ms em condições normais.
- [RNF03] Segurança via tokens JWT/JOSE.
