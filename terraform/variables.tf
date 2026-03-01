variable "db_username" {
  type = string
}

variable "db_password" {
  type      = string
  sensitive = true
}

variable "api_image" {
  type        = string
  description = "Full ECR image URI for order api"
}