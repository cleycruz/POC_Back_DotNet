output "app_service_plan_id" {
  description = "ID of the App Service Plan"
  value       = azurerm_service_plan.main.id
}

output "backend_app_service_name" {
  description = "Name of the backend App Service"
  value       = azurerm_linux_web_app.backend.name
}

output "frontend_app_service_name" {
  description = "Name of the frontend App Service"
  value       = azurerm_linux_web_app.frontend.name
}

output "app_service_url" {
  description = "URL of the backend App Service"
  value       = "https://${azurerm_linux_web_app.backend.default_hostname}"
}
