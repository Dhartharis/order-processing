data "aws_vpc" "default" {
  default = true
}

data "aws_subnets" "default" {
  filter {
    name   = "vpc-id"
    values = [data.aws_vpc.default.id]
  }
}

data "aws_ecs_cluster" "existing" {
  cluster_name = "order-cluster"
}

data "aws_iam_role" "ecs_execution_role" {
  name = "ecsTaskExecutionRole"
}

resource "aws_security_group" "rds" {
  name        = "orderprocessing-rds-sg"
  description = "RDS access"
  vpc_id      = data.aws_vpc.default.id

  ingress {
    from_port   = 5432
    to_port     = 5432
    protocol    = "tcp"
    cidr_blocks = [data.aws_vpc.default.cidr_block]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

resource "aws_db_subnet_group" "main" {
  name       = "orderprocessing-db-subnet"
  subnet_ids = data.aws_subnets.default.ids
}

resource "aws_db_instance" "postgres" {

  identifier = "orderprocessing-db"

  engine         = "postgres"
  engine_version = "16"

  instance_class = "db.t3.micro"
  allocated_storage = 20

  db_name  = "orderprocessing"
  username = var.db_username
  password = var.db_password

  vpc_security_group_ids = [
    aws_security_group.rds.id
  ]

  db_subnet_group_name = aws_db_subnet_group.main.name

  publicly_accessible = true
  skip_final_snapshot = true
}

resource "aws_ecs_task_definition" "api" {
  family                   = "order-api"
  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"
  cpu                      = "256"
  memory                   = "512"
  execution_role_arn       = data.aws_iam_role.ecs_execution_role.arn

  container_definitions = jsonencode([
    {
      name      = "api"
      image     = var.api_image
      essential = true

      portMappings = [
        {
          containerPort = 8080
          protocol      = "tcp"
        }
      ]

      secrets = [
        {
          name      = "ConnectionStrings__Default"
          valueFrom = aws_secretsmanager_secret.orderprocessing_db.arn
        }
      ]

      logConfiguration = {
        logDriver = "awslogs"
        options = {
          awslogs-group         = "/ecs/order-api"
          awslogs-region        = "ap-southeast-1"
          awslogs-stream-prefix = "ecs"
        }
      }
    }
  ])
}

resource "aws_secretsmanager_secret" "orderprocessing_db" {
  name = "orderprocessing/db/connection"
}

resource "aws_secretsmanager_secret_version" "orderprocessing_db" {
  secret_id = aws_secretsmanager_secret.orderprocessing_db.id

  secret_string = format(
    "Host=%s;Port=5432;Database=%s;Username=%s;Password=%s",
    aws_db_instance.postgres.address,
    aws_db_instance.postgres.db_name,
    var.db_username,
    var.db_password
  )
}

resource "aws_security_group" "alb" {
  name   = "orderprocessing-alb-sg"
  vpc_id = data.aws_vpc.default.id

  ingress {
    from_port   = 8080
    to_port     = 8080
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

resource "aws_lb" "api" {
  name               = "orderprocessing-api-alb"
  load_balancer_type = "application"
  security_groups    = [aws_security_group.alb.id]
  subnets            = data.aws_subnets.default.ids
}

resource "aws_lb_target_group" "api" {
  name        = "orderprocessing-api-tg"
  port        = 8080
  protocol    = "HTTP"
  vpc_id      = data.aws_vpc.default.id
  target_type = "ip"

  health_check {
    path = "/health"
    port = "traffic-port"
  }
}

resource "aws_lb_listener" "api" {
  load_balancer_arn = aws_lb.api.arn
  port              = 8080
  protocol          = "HTTP"

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.api.arn
  }
}

resource "aws_ecs_service" "api" {
  name            = "order-api"
  cluster         = data.aws_ecs_cluster.existing.id
  task_definition = aws_ecs_task_definition.api.arn
  desired_count   = 1
  launch_type     = "FARGATE"

  network_configuration {
    subnets         = data.aws_subnets.default.ids
    security_groups = [aws_security_group.ecs_api.id]
    assign_public_ip = true
  }

  load_balancer {
    target_group_arn = aws_lb_target_group.api.arn
    container_name   = "api"
    container_port   = 8080
  }

  depends_on = [
    aws_lb_listener.api
  ]
}

resource "aws_security_group" "ecs_api" {
  name   = "orderprocessing-api-sg"
  vpc_id = data.aws_vpc.default.id

  ingress {
    from_port       = 8080
    to_port         = 8080
    protocol        = "tcp"
    security_groups = [aws_security_group.alb.id]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

output "api_url" {
  value = "http://${aws_lb.api.dns_name}:8080"
}