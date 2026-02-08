# 📘 Plano de Riscos

---

## 1. Riscos Identificados

| Risco | Probabilidade | Impacto | Estratégia de Mitigação |
| :--- | :--- | :--- | :--- |
| Mudança de Requisitos UI | Média | Médio | Uso de componentes reutilizáveis e feedback constante. |
| Atraso na integração SMS/Zap | Baixa | Alto | Implementação de Mocks para desenvolvimento paralelo. |
| Incompatibilidade Docker | Baixa | Médio | Testes contínuos em ambiente local simulando prod. |
| Sobrecarga no Banco/Redis | Baixa | Alto | Implementação de Circuit Breaker e Retry Policies. |

## 2. Monitoramento

- Check semanal de integridade de logs (Core_Logs).
- Validação de custos de infraestrutura mensalmente.
