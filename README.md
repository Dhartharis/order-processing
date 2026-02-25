## Database initialization

Before starting the API service, the database must be created and migrated.

This is done by running the ECS task:


The migrator performs:

- Creates the database if it does not exist
- Applies all EF Core migrations

## Deployment flow

1. Run the ECS task `order-db-migrator`
2. Start the `order-api` service (set desired count to 1)

## Stack

- .NET 8
- Entity Framework Core
- PostgreSQL (Npgsql)
- AWS ECS Fargate
- Amazon RDS
- Terraform

# Order Processing API

End-to-end backend project using .NET 8 and AWS.

This project demonstrates a production-style architecture with separated API, background worker and database migrator.

## Architecture

- API Gateway
- AWS Lambda (.NET 8) – Order API
- ECS Fargate – async worker
- ECS Fargate – database migrator (one-shot task)
- PostgreSQL (Amazon RDS)
- Terraform for infrastructure
- CloudWatch logs
- GitHub Actions for CI

## Projects

Located under `src/`:

- `OrderProcessing.Api` – HTTP API
- `OrderProcessing.Application` – application layer
- `OrderProcessing.Domain` – domain model
- `OrderProcessing.Infrastructure` – EF Core + PostgreSQL
- `OrderProcessing.DbMigrator` – one-time database creation and migrations


