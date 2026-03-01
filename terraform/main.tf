data "aws_vpc" "default" {
  default = true
}

data "aws_subnets" "default" {
  filter {
    name   = "vpc-id"
    values = [data.aws_vpc.default.id]
  }
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