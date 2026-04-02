# 📦 Order Processing Ecosystem (.NET 8 + AWS)

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![AWS ECS](https://img.shields.io/badge/AWS-ECS%20Fargate-orange.svg)](https://aws.amazon.com/ecs/)
[![Terraform](https://img.shields.io/badge/IAC-Terraform-623ce4.svg)](https://www.terraform.io/)

A production-grade, distributed backend architecture designed for scalability and high availability. This project showcases a modern approach to handling order lifecycles using **Clean Architecture** and **Cloud-Native** patterns.

---

## 🏗 Architectural Highlights

This ecosystem is decoupled into specialized components to optimize resource allocation and maintainability:

* **Order API (Lambda/ECS):** High-availability entry point for order commands and queries.
* **Async Background Worker (ECS Fargate):** Dedicated worker service for processing long-running business logic without blocking the user-facing API.
* **Infrastructure as Code (Terraform):** Fully automated provisioning of AWS resources (RDS, ECS, IAM).
* **Resilient Database Strategy:** Independent **Database Migrator** task to ensure schema consistency in multi-instance deployments before scaling services.

---

## 🛠 Tech Stack & Tools

| Category | Technology |
| :--- | :--- |
| **Backend** | .NET 8 (C#), Entity Framework Core |
| **Cloud** | AWS (ECS Fargate, Lambda, RDS, CloudWatch) |
| **Infrastructure** | Terraform, Docker |
| **Database** | PostgreSQL (Npgsql) |
| **CI/CD** | GitHub Actions (Multi-image validation) |

---

## 📂 Project Structure (Clean Architecture)

Organized under `src/` to follow the Separation of Concerns principle:

- `OrderProcessing.Domain`: Core business entities and logic (Zero dependencies).
- `OrderProcessing.Application`: Use cases, DTOs, and interface definitions.
- `OrderProcessing.Infrastructure`: Data access (EF Core), external integrations, and AWS implementations.
- `OrderProcessing.Api`: REST entry point and request orchestration.
- `OrderProcessing.DbMigrator`: Specialized one-shot task for idempotent database initialization.

---

## 🚀 Deployment & CI Workflow

The project utilizes a robust **GitHub Actions** pipeline that builds and validates three independent Docker images: `order-api`, `order-worker`, and `order-db-migrator`.

### Deployment Sequence:
1.  **Schema Migration:** Execute the `order-db-migrator` ECS task to verify/update the PostgreSQL schema.
2.  **Service Activation:** Spin up `order-api` and `order-worker` instances once the data layer is validated.

---

## 🧪 Testing (In Progress)
- [x] **Unit Tests:** Validating Domain Model and Application Use Cases.
- [ ] **Integration Tests:** Database constraint and API contract verification.
