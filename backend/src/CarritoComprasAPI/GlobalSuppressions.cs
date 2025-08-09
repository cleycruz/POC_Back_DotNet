// Este archivo se usa para suprimir warnings de análisis de código
// para todo el proyecto
using System.Diagnostics.CodeAnalysis;

// Suprimir warnings de ensamblado que no son críticos para el funcionamiento
[assembly: SuppressMessage("Microsoft.Globalization", "CA1016:MarkAssembliesWithAssemblyVersionAttribute")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1014:MarkAssembliesWithCLSCompliantAttribute")]
[assembly: SuppressMessage("Microsoft.Globalization", "CA1017:MarkAssembliesWithComVisibleAttribute")]

// Suprimir warnings CA1002 de List<T> específicos (aceptables en contexto de API Web)
[assembly: SuppressMessage("Design", "CA1002:Do not expose generic lists", Scope = "member", Target = "~P:CarritoComprasAPI.DTOs.CarritoDto.Items")]
[assembly: SuppressMessage("Design", "CA1002:Do not expose generic lists", Scope = "member", Target = "~P:CarritoComprasAPI.Models.Carrito.Items")]
[assembly: SuppressMessage("Design", "CA1002:Do not expose generic lists", Scope = "member", Target = "~P:CarritoComprasAPI.Core.Configuration.AppSettings.ApiConfig.AllowedOrigins")]
[assembly: SuppressMessage("Design", "CA1002:Do not expose generic lists", Scope = "member", Target = "~P:CarritoComprasAPI.Core.EventSourcing.EventSourcedRepository.AuditReport.EventosRecientes")]
[assembly: SuppressMessage("Design", "CA1002:Do not expose generic lists", Scope = "member", Target = "~P:CarritoComprasAPI.Core.Metrics.MetricsModels.CyclomaticComplexityReport.Methods")]
[assembly: SuppressMessage("Design", "CA1002:Do not expose generic lists", Scope = "member", Target = "~P:CarritoComprasAPI.Core.Metrics.MetricsModels.CyclomaticComplexityReport.HighComplexityMethods")]
[assembly: SuppressMessage("Design", "CA1002:Do not expose generic lists", Scope = "member", Target = "~P:CarritoComprasAPI.Core.Validators.ValidationExceptionMiddleware.ErrorResponse.Errors")]
[assembly: SuppressMessage("Design", "CA1002:Do not expose generic lists", Scope = "member", Target = "~P:CarritoComprasAPI.Core.Metrics.MetricsModels.MetricsExecutiveSummary.KeyStrengths")]
[assembly: SuppressMessage("Design", "CA1002:Do not expose generic lists", Scope = "member", Target = "~P:CarritoComprasAPI.Core.Metrics.MetricsModels.MetricsExecutiveSummary.CriticalIssues")]
[assembly: SuppressMessage("Design", "CA1002:Do not expose generic lists", Scope = "member", Target = "~P:CarritoComprasAPI.Core.Metrics.MetricsModels.MetricsExecutiveSummary.Recommendations")]

// Suprimir warnings de List<T> en eventos de auditoria y validators
[assembly: SuppressMessage("Design", "CA1002:Do not expose generic lists", Scope = "member", Target = "~P:CarritoComprasAPI.Core.EventSourcing.Events.ProductoActualizadoEvent.CamposModificados")]
[assembly: SuppressMessage("Design", "CA1002:Do not expose generic lists", Scope = "member", Target = "~P:CarritoComprasAPI.Core.EventSourcing.Events.CarritoVaciadoEvent.ItemsEliminados")]
[assembly: SuppressMessage("Design", "CA1002:Do not expose generic lists", Scope = "member", Target = "~P:CarritoComprasAPI.Core.EventSourcing.EventSourcedRepository.AuditReport.EventosRecientes")]
[assembly: SuppressMessage("Design", "CA1002:Do not expose generic lists", Scope = "member", Target = "~P:CarritoComprasAPI.Core.Validators.ValidationExceptionMiddleware.ErrorResponse.Errors")]
[assembly: SuppressMessage("Design", "CA1002:Do not expose generic lists", Scope = "member", Target = "~M:CarritoComprasAPI.Core.EventSourcing.Events.CarritoVaciadoEvent.#ctor(System.String,System.String,System.Int32,System.Decimal,System.String,System.Collections.Generic.List{CarritoComprasAPI.Core.EventSourcing.Events.ItemCarritoInfo},System.Int64)")]

// Suprimir warnings de alertas y cache
[assembly: SuppressMessage("Design", "CA1002:Do not expose generic lists", Scope = "member", Target = "~M:CarritoComprasAPI.Core.Alerting.IAlertingService.GetActiveAlerts")]
[assembly: SuppressMessage("Design", "CA1002:Do not expose generic lists", Scope = "member", Target = "~M:CarritoComprasAPI.Core.Caching.ConcurrentHashSet`1.ToList")]

// Suprimir warnings de ToString() sin IFormatProvider en eventos de auditoria (no crítico para funcionalidad)
[assembly: SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Scope = "member", Target = "~M:CarritoComprasAPI.Core.EventSourcing.Events.ProductoCreadoEvent.#ctor(System.Int32,System.String,System.String,System.Decimal,System.Int32,System.String)")]
[assembly: SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Scope = "member", Target = "~M:CarritoComprasAPI.Core.EventSourcing.Events.ProductoActualizadoEvent.#ctor(System.Int32,System.Int64)")]
[assembly: SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Scope = "member", Target = "~M:CarritoComprasAPI.Core.EventSourcing.Events.ProductoEliminadoEvent.#ctor(System.Int32,System.String,System.String,System.Decimal,System.Int32,System.String,System.String,System.Int64)")]
[assembly: SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Scope = "member", Target = "~M:CarritoComprasAPI.Core.EventSourcing.Events.ProductoStockCambiadoEvent.#ctor(System.Int32,System.Int32,System.Int32,System.String,System.Int64,System.String)")]
[assembly: SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Scope = "member", Target = "~M:CarritoComprasAPI.Core.EventSourcing.DomainEventToEventStoreBridge.ExtractEntityIdFromDomainEvent(CarritoComprasAPI.Core.Domain.Events.DomainEvent)")]

// Suprimir warnings CLS Compliance para API Web (no necesario en contexto web)
[assembly: SuppressMessage("Microsoft.Interoperability", "CA1407:Avoid static members in COM visible types")]
[assembly: SuppressMessage("Microsoft.Design", "CA1014:MarkAssembliesWithClsCompliant")]

// Suprimir todos los warnings CLS Compliance (CS3xxx) - no críticos para API Web
[assembly: SuppressMessage("Microsoft.Usage", "CS3001:Argument type is not CLS-compliant")]
[assembly: SuppressMessage("Microsoft.Usage", "CS3002:Return type is not CLS-compliant")]  
[assembly: SuppressMessage("Microsoft.Usage", "CS3009:Base type is not CLS-compliant")]
[assembly: SuppressMessage("Microsoft.Usage", "CS3016:Arrays as attribute arguments is not CLS-compliant")]
