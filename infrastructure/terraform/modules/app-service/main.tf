# App Service Plan
resource "azurerm_service_plan" "main" {
  name                = "plan-${var.app_name}-${var.environment}"
  resource_group_name = var.resource_group_name
  location            = var.location
  os_type             = "Linux"
  sku_name            = "B1"

  tags = var.common_tags
}

# Backend App Service
resource "azurerm_linux_web_app" "backend" {
  name                = "app-${var.app_name}-backend-${var.environment}"
  resource_group_name = var.resource_group_name
  location            = var.location
  service_plan_id     = azurerm_service_plan.main.id

  site_config {
    always_on = false
    application_stack {
      dotnet_version = "8.0"
    }
  }

  app_settings = {
    "ASPNETCORE_ENVIRONMENT" = var.environment
  }

  tags = var.common_tags
}

# Frontend App Service
resource "azurerm_linux_web_app" "frontend" {
  name                = "app-${var.app_name}-frontend-${var.environment}"
  resource_group_name = var.resource_group_name
  location            = var.location
  service_plan_id     = azurerm_service_plan.main.id

  site_config {
    always_on = false
    application_stack {
      node_version = "18-lts"
    }
  }

  tags = var.common_tags
}
