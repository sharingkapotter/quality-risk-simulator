Quality & Risk Simulator — Overview

This project is a deliberately imperfect microservices-based system built to explore authentication, resilience, and failure handling in distributed environments.

Instead of optimizing for feature completeness, the focus is on observable failure modes and the engineering decisions required to handle them cleanly.

The goal is to make system behavior easy to reason about when things don’t go well.

Running the system with Docker

The entire system runs locally using Docker and docker-compose.

Prerequisites

Docker Desktop

Start all services

From the project root:

docker-compose up --build


Once running, each service exposes its own Swagger UI for testing.

Architecture

The system consists of three independent services:

UserService

Responsible for authentication

Issues JWT tokens

OrderService

Core business service

Performs independent JWT validation

Communicates with PaymentService

Implements resilience patterns (timeouts, retries, circuit breaker)

PaymentService

Intentionally unreliable

Simulates latency and intermittent failures

Request flow
Client → UserService (authentication)
Client → OrderService (authorized requests)
OrderService → PaymentService (downstream dependency)


All services are loosely coupled and stateless.

Authentication model

Authentication is implemented using JWTs:

Tokens are issued by UserService

Tokens are validated independently by OrderService

There is no runtime dependency between services for authentication

This design keeps services autonomous and allows them to scale independently.

Resilience and failure handling

PaymentService is intentionally unstable to simulate real-world downstream behavior.

OrderService applies multiple resilience mechanisms:

Timeouts to avoid blocking requests

Retries for transient failures

Circuit breaker to isolate sustained outages

When the circuit breaker is open, OrderService fails fast instead of repeatedly calling an unhealthy dependency.

HTTP semantics

Order creation behavior is intentionally explicit:

Condition	Response
Successful payment	200 OK
Downstream failure	502 Bad Gateway
Downstream timeout	504 Gateway Timeout
Circuit breaker open	503 Service Unavailable

This allows clients to clearly distinguish between:

transient failures

temporary unavailability

sustained downstream outages

Observed failure scenario

Issue
Order requests consistently returned 502.

Root cause
OrderService was misconfigured and was calling itself instead of PaymentService.

Detection
PaymentService logs showed no inbound traffic.

Resolution
Corrected the downstream service endpoint configuration.

Outcome
Expected behavior was restored and resilience logic was validated.

Key takeaways

Stateless authentication across services

Graceful handling of unreliable dependencies

Prevention of cascading failures

Practical use of retry and circuit breaker patterns

Debugging real distributed system issues

Potential future improvements

These were intentionally deferred to keep the focus on core behavior:

API Gateway integration

Asynchronous payment processing

Centralized logging and metrics

Configuration-driven resilience thresholds

Closing note

This project prioritizes engineering judgment and system behavior over completeness.

The imperfections are intentional and serve as learning points rather than limitations.
