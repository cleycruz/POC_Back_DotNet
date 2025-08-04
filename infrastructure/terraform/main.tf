terraform {
  required_version = ">= 1.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.0"
    }
  }
}

provider "azurerm" {
  features {}
}

# Resource Group
resource "azurerm_resource_group" "main" {
  name     = var.resource_group_name
  location = var.location

  tags = var.common_tags
}

# App Service Plan
module "app_service" {
  source = "./modules/app-service"
  
  resource_group_name = azurerm_resource_group.main.name
  location           = azurerm_resource_group.main.location
  app_name          = var.app_name
  environment       = var.environment
  common_tags       = var.common_tags
}

# Database
module "database" {
  source = "./modules/database"
  
  resource_group_name = azurerm_resource_group.main.name
  location           = azurerm_resource_group.main.location
  database_name     = var.database_name
  environment       = var.environment
  common_tags       = var.common_tags
}

# Storage
module "storage" {
  source = "./modules/storage"
  
  resource_group_name = azurerm_resource_group.main.name
  location           = azurerm_resource_group.main.location
  storage_name      = var.storage_name
  environment       = var.environment
  common_tags       = var.common_tags
}

# Networking
module "networking" {
  source = "./modules/networking"
  
  resource_group_name = azurerm_resource_group.main.name
  location           = azurerm_resource_group.main.location
  vnet_name         = var.vnet_name
  environment       = var.environment
  common_tags       = var.common_tags
}

# Monitoring
module "monitoring" {
  source = "./modules/monitoring"
  
  resource_group_name = azurerm_resource_group.main.name
  location           = azurerm_resource_group.main.location
  app_name          = var.app_name
  environment       = var.environment
  common_tags       = var.common_tags
}
