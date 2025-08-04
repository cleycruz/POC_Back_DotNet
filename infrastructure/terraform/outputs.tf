output "resource_group_name" {
  description = "Name of the resource group"
  value       = azurerm_resource_group.main.name
}

output "app_service_url" {
  description = "URL of the App Service"
  value       = module.app_service.app_service_url
}

output "database_connection_string" {
  description = "Database connection string"
  value       = module.database.connection_string
  sensitive   = true
}

output "storage_account_name" {
  description = "Storage account name"
  value       = module.storage.storage_account_name
}

output "application_insights_key" {
  description = "Application Insights instrumentation key"
  value       = module.monitoring.instrumentation_key
  sensitive   = true
}
