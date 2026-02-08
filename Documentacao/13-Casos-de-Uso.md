# 📘 Casos de Uso (Use Cases)

---

## 1. UC01 - Autenticação Multi-Fator (MFA)

**Ator Principal:** Usuário  
**Pré-condição:** Usuário cadastrado no sistema.

**Fluxo Principal:**

1. O usuário insere identificador (e-mail, CPF ou nickname).
2. O sistema valida a senha.
3. O sistema envia um código via SMS ou WhatsApp através do serviço de Segurança.
4. O usuário insere o código recebido.
5. O sistema libera o acesso e gera o token de sessão.

---

## 2. UC02 - Sincronização de Listas de Compras

**Ator Principal:** Usuário  
**Pré-condição:** Estar logado na conta.

**Fluxo Principal:**

1. O usuário adiciona um item à lista no App Mobile (Ionic).
2. O App envia a requisição para o Gateway.
3. O Gateway repassa para o Backend de Produtos.
4. O Backend salva no Banco e limpa o cache no Redis.
5. O usuário abre o navegador (Angular) e vê a lista atualizada instantaneamente.

---

## 3. UC03 - Consulta de Preços e Ofertas

**Ator Principal:** Usuário  
**Pré-condição:** Nenhuma (Acesso anônimo permitido para visualização).

**Fluxo Principal:**

1. O usuário acessa a Home.
2. O sistema recupera as ofertas do dia do cache Redis (para performance).
3. Se não houver no cache, busca no SQL Server e popula o Redis.
4. O sistema exibe o carrossel de ofertas com imagens e preços.

---

## 4. UC04 - Gestão de Categorias e Produtos

**Ator Principal:** Administrador  
**Pré-condição:** Perfil administrativo.

**Fluxo Principal:**

1. O administrador acessa a área de gestão.
2. Cria uma nova categoria de produto (ex: Limpeza).
3. Associa novos produtos a esta categoria.
4. O sistema valida as regras de negócio e persiste as alterações.
