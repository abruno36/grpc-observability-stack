# gRPC Observability Stack — Prometheus & Grafana

Este projeto demonstra uma **arquitetura completa de observabilidade para serviços gRPC**, utilizando **Prometheus** e **Grafana**, com métricas reais, dashboards prontos e simulação de carga com erro controlado.

O repositório foi criado com foco em **boas práticas de SRE / Observability** e também como **case técnico para entrevistas**.

---

## 🧱 Arquitetura

```
┌─────────────────┐
│ GrpcLoadClient  │
│ (gera carga)    │
└────────┬────────┘
         │ gRPC
         ▼
┌─────────────────┐
│   GrpcServer    │
│ (.NET gRPC API) │
└────────┬────────┘
         │ /metrics
         ▼
┌─────────────────┐
│   Prometheus    │
│ (scrape a cada  │
│ 5 segundos)     │
└────────┬────────┘
         │ PromQL
         ▼
┌─────────────────┐
│    Grafana      │
│ Dashboards &    │
│ Alertas         │
└─────────────────┘

```

---

## 📦 Componentes

### 🔹 GrpcServer (.NET)
- Serviço gRPC simples (`SayHello`)
- Instrumentado com **Prometheus**
- Métrica principal:
  ```
  grpc_requests_total{method, status}
  ```

### 🔹 GrpcLoadClient (.NET)
- Cliente gRPC que gera tráfego contínuo
- Simula **50% de sucesso e 50% de erro**
- Erros reais (`StatusCode.Internal`)

### 🔹 Prometheus
- Scrape da endpoint `/metrics`
- Intervalo de coleta: **5 segundos**

### 🔹 Grafana
- Dashboards **provisionados via JSON**
- Não requer configuração manual

---

## 📊 Métricas Expostas

### Counter principal
```text
grpc_requests_total{method="SayHello", status="OK"}
grpc_requests_total{method="SayHello", status="Internal"}
```

Essas métricas permitem:
- Requests por segundo (RPS)
- Taxa de erro
- Error percentage
- Análise por status code

---

## 📈 Dashboards

### 1️⃣ gRPC Errors — Detailed Analysis

Dashboard focado em **erros e estabilidade**.

**Painéis:**
- Errors (Last 5 Minutes)
- gRPC Error Percentage (%)
- gRPC Errors — Last 5 Minutes (por status)
- gRPC Errors per Second (by Status Code)

---

### 2️⃣ gRPC Service Observability

Dashboard focado nos **Golden Signals**.

**Painéis:**
- gRPC Requests per Second (RPS)
- gRPC Error Rate (%)

> ⚠️ Latência (p95/p99) não foi incluída propositalmente, pois o serviço não expõe histogramas de duração — mantendo o exemplo simples e objetivo.

---

## 🧠 Principais Queries (PromQL)

### Requests por segundo
```promql
rate(grpc_requests_total[1m])
```

### Erros por segundo (por status)
```promql
sum by (status) (
  rate(grpc_requests_total{status!="OK"}[1m])
)
```

### Total de erros (últimos 5 minutos)
```promql
sum(
  increase(grpc_requests_total{status!="OK"}[5m])
)
```

### Error Percentage
```promql
(
  sum(rate(grpc_requests_total{status!="OK"}[5m]))
/
  sum(rate(grpc_requests_total[5m]))
) * 100
```

---

## 🚨 Simulação de Erro (50%)

A simulação de erro é feita **no cliente**, não no servidor.

### Estratégia
- Uma chamada retorna sucesso
- A próxima retorna erro (`StatusCode.Internal`)
- Resultado: **~50% de erro real**

Isso simula falhas intermitentes comuns em ambientes produtivos.

---

## ▶️ Como executar

### Pré-requisitos
- Docker + Docker Compose
- .NET 8 SDK (opcional, para rodar local)

---

### Subir stack completa
```bash
docker compose up -d
```

Acessos:
- Grafana: http://localhost:3000
- Prometheus: http://localhost:9090

---

### Rodar o gerador de carga
```bash
dotnet run --project GrpcLoadClient
```

Após alguns segundos, os dashboards começam a refletir:
- RPS estável
- Error rate ≈ 50%
- Erros por status code

---

## 💼 Por que este projeto é relevante?

Este projeto demonstra conhecimento prático em:

- Observabilidade moderna
- Prometheus & Grafana
- Golden Signals
- gRPC em produção
- Métricas orientadas a erro real
- Instrumentação correta
- Separação entre **carga**, **serviço** e **observabilidade**

É um **case completo e realista**, não apenas um exemplo acadêmico.

---

## 🎯 Próximos passos (opcionais)

- Alertas Prometheus (warning / critical)
- Histogramas de latência
- Exportação de dashboards
- CI para validação automática
- Chaos testing

---

## 🏁 Conclusão

Este repositório funciona como um **mini laboratório de observabilidade gRPC**, pronto para ser usado como **showcase técnico**, onboarding ou demonstração em entrevistas.

### 🧪 Como testar o projeto (passo a passo)

Este projeto foi pensado para ser executado e validado por qualquer pessoa
em poucos minutos.

#### Pré-requisitos
- Docker
- Docker Compose
- .NET 8 SDK (apenas para rodar o gerador de carga)

---

#### 1️⃣ Subir a stack de observabilidade

Na raiz do repositório, execute:

```bash
docker compose up -d
