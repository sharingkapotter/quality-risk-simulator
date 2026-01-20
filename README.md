# Quality & Risk Simulator  
**A microservices playground for learning failure modes, resilience, and distributed systems**

[![Docker Compose](https://img.shields.io/badge/docker%20compose-%230db7ed.svg?style=for-the-badge&logo=docker&logoColor=white)](https://docs.docker.com/compose/)  
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](./LICENSE)  
[![GitHub stars](https://img.shields.io/github/stars/YOUR_USERNAME/quality-risk-simulator?style=for-the-badge)](https://github.com/YOUR_USERNAME/quality-risk-simulator)

This is **not** a production-grade application.  

It is a **deliberately imperfect** microservices system designed to expose real-world failure patterns and teach clean handling of authentication, resilience, timeouts, retries, circuit breakers, and cascading failures.

The focus is on **observability** and **reasoning about behavior when things break** — not on feature richness or polish.

## ✨ Key Learning Objectives

- Stateless, independent JWT authentication
- Graceful degradation with unreliable downstream services
- Preventing cascading failures using timeouts, retries & circuit breakers
- Explicit HTTP semantics for different failure modes
- Debugging distributed systems (logs, misconfigurations, etc.)

## 🏗️ Architecture Overview

Three loosely coupled, stateless services:

| Service          | Responsibility                              | Key Behaviors / Patterns                                      |
|------------------|---------------------------------------------|----------------------------------------------------------------|
| **UserService**     | Issues JWT tokens                           | Authentication only, no downstream calls                       |
| **OrderService**    | Core business logic, order creation         | Validates JWT independently<br>• Timeouts<br>• Retries<br>• Circuit breaker |
| **PaymentService**  | Payment processing (intentionally flaky)   | Simulates latency, errors, and outages                         |

**Request flow**  
Client → UserService (login) → JWT  
Client → OrderService (create order) → PaymentService (pay)

All inter-service communication uses HTTP + JWT validation — **no shared session or auth coupling at runtime**.

## 🚀 Quick Start – Run Locally with Docker

### Prerequisites

- Docker Desktop (or Docker + docker-compose)

### Start everything

```bash
# From the project root
docker compose up --build
```

That's it! 🎉

Once healthy, open these Swagger UIs in your browser:

- **UserService**  http://localhost:8001/swagger-ui.html
- **OrderService**  http://localhost:8002/swagger-ui.html
- **PaymentService** http://localhost:8003/swagger-ui.html

## 🔐 Authentication Model

- JWTs issued **only** by UserService
- OrderService validates tokens **independently** (using public key or shared secret)
- No runtime dependency between services for auth → easy horizontal scaling

## 🛡️ Resilience & Failure Handling

**PaymentService** is intentionally unreliable:

- Random latency (0–5s)
- Random 5xx errors
- Random connection refusals / timeouts

**OrderService** protects itself with:

- **Timeouts** — prevent hanging
- **Retries** — handle transient issues
- **Circuit breaker** — fail fast during sustained outages

When the circuit is **open**, OrderService returns fast failures instead of hammering the unhealthy service.

## 📡 HTTP Response Semantics (deliberately explicit)

| Scenario                     | HTTP Status             | Meaning to Client                                      |
|------------------------------|-------------------------|--------------------------------------------------------|
| Successful payment           | 200 OK                  | All good                                               |
| Downstream failure (5xx)     | 502 Bad Gateway         | Payment gateway responded with error                   |
| Downstream timeout           | 504 Gateway Timeout     | Payment gateway didn't respond in time                 |
| Circuit breaker open         | 503 Service Unavailable | Downstream dependency is unhealthy — try later         |

This clarity helps clients distinguish between transient issues, timeouts, and prolonged outages.

## 🐞 Observed Failure Scenario (real example)

**Symptom**  
All `/orders` requests returned **502 Bad Gateway** consistently.

**Root cause**  
OrderService was misconfigured — calling its own endpoint instead of PaymentService.

**Detection**  
PaymentService logs showed **zero** incoming traffic.

**Resolution**  
Fixed the downstream URL in configuration.

**Lesson**  
Resilience patterns are only as good as your observability and configuration hygiene.

## 🎯 Key Takeaways

- Keep authentication **stateless** and **autonomous**
- Make failure modes **explicit** and **distinguishable**
- Use timeouts + retries + circuit breakers to avoid cascades
- Invest in logs & tracing — they saved hours of debugging
- Imperfections are **intentional learning tools**

## 🔮 Potential Future Extensions (intentionally left out)

- API Gateway / Ingress
- Async payment webhook processing
- Centralized observability (Prometheus + Grafana + Loki)
- Configurable resilience thresholds via env vars / config service
- Rate limiting & bulkhead patterns

---

**Closing Note**  
This project values **engineering judgment** and **understandable failure behavior** over completeness.  
The "flaws" are **features** — they create teachable moments.

Happy breaking (and fixing) things! 🛠️💥

Feel free to open issues, suggest improvements, or fork it for your own chaos experiments.
```

