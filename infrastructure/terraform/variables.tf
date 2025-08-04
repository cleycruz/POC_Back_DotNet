variable "resource_group_name" {
  description = "Name of the resource group"
  type        = string
  default     = "rg-carrito-compras"
}

variable "location" {
  description = "Azure region for resources"
  type        = string
  default     = "East US"
}

variable "environment" {
  description = "Environment name (dev, staging, prod)"
  type        = string
  default     = "dev"
}

variable "app_name" {
  description = "Application name"
  type        = string
  default     = "carrito-compras"
}

variable "database_name" {
  description = "Database name"
  type        = string
  default     = "carritocomprasdb"
}

variable "storage_name" {
  description = "Storage account name"
  type        = string
  default     = "carritocomprasstorage"
}

variable "vnet_name" {
  description = "Virtual network name"
  type        = string
  default     = "vnet-carrito-compras"
}

variable "common_tags" {
  description = "Common tags for all resources"
  type        = map(string)
  default = {
    Project     = "CarritoCompras"
    Owner       = "DevTeam"
    Environment = "dev"
  }
}
