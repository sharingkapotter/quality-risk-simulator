**Quality & Risk Simulator**
Overview

This project is a deliberately imperfect microservices-based system designed to explore authentication, resilience, and failure handling in distributed environments.

Rather than optimizing for completeness, the system focuses on observable failure modes and the engineering decisions required to manage them.

## Running the system with Docker

This project can be run locally using Docker and docker-compose.

### Prerequisites

- Docker Desktop

### Start all services

From the project root:

```bash
docker-compose up --build


Architecture

The system consists of three independent services:

UserService

Responsible for authentication

Issues JWT tokens

OrderService

Core business service

Performs independent JWT validation

Communicates with PaymentService

Implements resilience patterns

PaymentService

Intentionally unreliable

Simulates latency and intermittent failures

Request flow:

Client → UserService (authentication)
Client → OrderService (authorized requests)
OrderService → PaymentService (downstream dependency)

Services are loosely coupled and stateless.

Authentication model

Authentication is implemented using JWTs:

Tokens are issued by UserService

Tokens are validated independently by OrderService

No runtime dependency exists between services for authentication

This approach supports scalability and service autonomy.

Resilience and failure handling

PaymentService is intentionally unstable to simulate real-world downstream behavior.

OrderService applies multiple resilience mechanisms:

Timeouts to prevent request blocking

Retries for transient failures

Circuit breaker to isolate sustained outages

When the circuit breaker is open, OrderService fails fast rather than repeatedly calling an unhealthy dependency.

HTTP semantics

Order creation behavior is intentionally explicit:

Condition Response
Successful payment 200 OK
Downstream failure 502 Bad Gateway
Downstream timeout 504 Gateway Timeout
Circuit breaker open 503 Service Unavailable

This allows clients to distinguish between transient failures and temporary unavailability.

Observed failure scenario

Issue:
Order requests consistently returned 502.

Root cause:
OrderService was misconfigured to call itself instead of PaymentService.

Detection:
Downstream service logs showed no inbound traffic.

Resolution:
Corrected the downstream service endpoint configuration.

Outcome:
Restored expected behavior and validated resilience logic.

Key takeaways

Stateless authentication across services

Graceful handling of unreliable dependencies

Prevention of cascading failures

Practical use of retry and circuit breaker patterns

Debugging of real distributed system issues

Potential future improvements

API Gateway integration

Asynchronous processing for payments

Centralized logging and metrics

Configuration-driven resilience thresholds

These enhancements were intentionally deferred to preserve focus on core behavior.

Closing note

This project prioritizes engineering judgment and system behavior over completeness.

The imperfections are intentional and serve as learning points rather than limitations.
```
