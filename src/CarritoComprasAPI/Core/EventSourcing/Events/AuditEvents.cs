namespace CarritoComprasAPI.Core.EventSourcing.Events
{
    // ============= EVENTOS DE PRODUCTOS =============

    /// <summary>
    /// Evento cuando se crea un producto
    /// </summary>
    public record ProductoCreadoEvent : EventBase
    {
        public int ProductoId { get; init; }
        public string Nombre { get; init; } = string.Empty;
        public string Descripcion { get; init; } = string.Empty;
        public decimal Precio { get; init; }
        public int Stock { get; init; }
        public string Categoria { get; init; } = string.Empty;

        public ProductoCreadoEvent() { }

        public ProductoCreadoEvent(int productoId, string nombre, string descripcion, decimal precio, int stock, string categoria)
            : base(productoId.ToString(), "Producto", 1)
        {
            ProductoId = productoId;
            Nombre = nombre;
            Descripcion = descripcion;
            Precio = precio;
            Stock = stock;
            Categoria = categoria;
        }
    }

    /// <summary>
    /// Evento cuando se actualiza un producto
    /// </summary>
    public record ProductoActualizadoEvent : EventBase
    {
        public int ProductoId { get; init; }
        public string? NombreAnterior { get; init; }
        public string? NombreNuevo { get; init; }
        public string? DescripcionAnterior { get; init; }
        public string? DescripcionNueva { get; init; }
        public decimal? PrecioAnterior { get; init; }
        public decimal? PrecioNuevo { get; init; }
        public int? StockAnterior { get; init; }
        public int? StockNuevo { get; init; }
        public string? CategoriaAnterior { get; init; }
        public string? CategoriaNueva { get; init; }
        public List<string> CamposModificados { get; init; } = new();

        public ProductoActualizadoEvent() { }

        public ProductoActualizadoEvent(int productoId, long version) : base(productoId.ToString(), "Producto", version)
        {
            ProductoId = productoId;
        }
    }

    /// <summary>
    /// Evento cuando se elimina un producto
    /// </summary>
    public record ProductoEliminadoEvent : EventBase
    {
        public int ProductoId { get; init; }
        public string Nombre { get; init; } = string.Empty;
        public string Descripcion { get; init; } = string.Empty;
        public decimal Precio { get; init; }
        public int Stock { get; init; }
        public string Categoria { get; init; } = string.Empty;
        public string MotivoEliminacion { get; init; } = string.Empty;

        public ProductoEliminadoEvent() { }

        public ProductoEliminadoEvent(int productoId, string nombre, string descripcion, decimal precio, int stock, string categoria, string motivo, long version)
            : base(productoId.ToString(), "Producto", version)
        {
            ProductoId = productoId;
            Nombre = nombre;
            Descripcion = descripcion;
            Precio = precio;
            Stock = stock;
            Categoria = categoria;
            MotivoEliminacion = motivo;
        }
    }

    /// <summary>
    /// Evento cuando cambia el stock de un producto
    /// </summary>
    public record ProductoStockCambiadoEvent : EventBase
    {
        public int ProductoId { get; init; }
        public int StockAnterior { get; init; }
        public int StockNuevo { get; init; }
        public int Diferencia { get; init; }
        public string Motivo { get; init; } = string.Empty;
        public string? CarritoId { get; init; }

        public ProductoStockCambiadoEvent() { }

        public ProductoStockCambiadoEvent(int productoId, int stockAnterior, int stockNuevo, string motivo, long version, string? carritoId = null)
            : base(productoId.ToString(), "Producto", version)
        {
            ProductoId = productoId;
            StockAnterior = stockAnterior;
            StockNuevo = stockNuevo;
            Diferencia = stockNuevo - stockAnterior;
            Motivo = motivo;
            CarritoId = carritoId;
        }
    }

    // ============= EVENTOS DE CARRITO =============

    /// <summary>
    /// Evento cuando se crea un carrito
    /// </summary>
    public record CarritoCreadoEvent : EventBase
    {
        public string CarritoId { get; init; } = string.Empty;
        public string UsuarioId { get; init; } = string.Empty;

        public CarritoCreadoEvent() { }

        public CarritoCreadoEvent(string carritoId, string usuarioId)
            : base(carritoId, "Carrito", 1)
        {
            CarritoId = carritoId;
            UsuarioId = usuarioId;
        }
    }

    /// <summary>
    /// Evento cuando se agrega un item al carrito
    /// </summary>
    public record ItemAgregadoAlCarritoEvent : EventBase
    {
        public string CarritoId { get; init; } = string.Empty;
        public string UsuarioId { get; init; } = string.Empty;
        public int ProductoId { get; init; }
        public string NombreProducto { get; init; } = string.Empty;
        public decimal PrecioUnitario { get; init; }
        public int Cantidad { get; init; }
        public decimal Subtotal { get; init; }
        public int CantidadAnterior { get; init; }
        public bool EsNuevoItem { get; init; }

        public ItemAgregadoAlCarritoEvent() { }

        public ItemAgregadoAlCarritoEvent(string carritoId, string usuarioId, int productoId, string nombreProducto, 
            decimal precioUnitario, int cantidad, decimal subtotal, int cantidadAnterior, bool esNuevoItem, long version)
            : base(carritoId, "Carrito", version)
        {
            CarritoId = carritoId;
            UsuarioId = usuarioId;
            ProductoId = productoId;
            NombreProducto = nombreProducto;
            PrecioUnitario = precioUnitario;
            Cantidad = cantidad;
            Subtotal = subtotal;
            CantidadAnterior = cantidadAnterior;
            EsNuevoItem = esNuevoItem;
        }
    }

    /// <summary>
    /// Evento cuando se actualiza la cantidad de un item en el carrito
    /// </summary>
    public record CantidadItemActualizadaEvent : EventBase
    {
        public string CarritoId { get; init; } = string.Empty;
        public string UsuarioId { get; init; } = string.Empty;
        public int ProductoId { get; init; }
        public string NombreProducto { get; init; } = string.Empty;
        public int CantidadAnterior { get; init; }
        public int CantidadNueva { get; init; }
        public int Diferencia { get; init; }
        public decimal PrecioUnitario { get; init; }
        public decimal SubtotalAnterior { get; init; }
        public decimal SubtotalNuevo { get; init; }

        public CantidadItemActualizadaEvent() { }

        public CantidadItemActualizadaEvent(string carritoId, string usuarioId, int productoId, string nombreProducto,
            int cantidadAnterior, int cantidadNueva, decimal precioUnitario, long version)
            : base(carritoId, "Carrito", version)
        {
            CarritoId = carritoId;
            UsuarioId = usuarioId;
            ProductoId = productoId;
            NombreProducto = nombreProducto;
            CantidadAnterior = cantidadAnterior;
            CantidadNueva = cantidadNueva;
            Diferencia = cantidadNueva - cantidadAnterior;
            PrecioUnitario = precioUnitario;
            SubtotalAnterior = cantidadAnterior * precioUnitario;
            SubtotalNuevo = cantidadNueva * precioUnitario;
        }
    }

    /// <summary>
    /// Evento cuando se elimina un item del carrito
    /// </summary>
    public record ItemEliminadoDelCarritoEvent : EventBase
    {
        public string CarritoId { get; init; } = string.Empty;
        public string UsuarioId { get; init; } = string.Empty;
        public int ProductoId { get; init; }
        public string NombreProducto { get; init; } = string.Empty;
        public int CantidadEliminada { get; init; }
        public decimal PrecioUnitario { get; init; }
        public decimal SubtotalPerdido { get; init; }
        public string Motivo { get; init; } = string.Empty;

        public ItemEliminadoDelCarritoEvent() { }

        public ItemEliminadoDelCarritoEvent(string carritoId, string usuarioId, int productoId, string nombreProducto,
            int cantidadEliminada, decimal precioUnitario, string motivo, long version)
            : base(carritoId, "Carrito", version)
        {
            CarritoId = carritoId;
            UsuarioId = usuarioId;
            ProductoId = productoId;
            NombreProducto = nombreProducto;
            CantidadEliminada = cantidadEliminada;
            PrecioUnitario = precioUnitario;
            SubtotalPerdido = cantidadEliminada * precioUnitario;
            Motivo = motivo;
        }
    }

    /// <summary>
    /// Evento cuando se vacía un carrito
    /// </summary>
    public record CarritoVaciadoEvent : EventBase
    {
        public string CarritoId { get; init; } = string.Empty;
        public string UsuarioId { get; init; } = string.Empty;
        public int CantidadItems { get; init; }
        public decimal TotalPerdido { get; init; }
        public string Motivo { get; init; } = string.Empty;
        public List<ItemCarritoInfo> ItemsEliminados { get; init; } = new();

        public CarritoVaciadoEvent() { }

        public CarritoVaciadoEvent(string carritoId, string usuarioId, int cantidadItems, decimal totalPerdido,
            string motivo, List<ItemCarritoInfo> itemsEliminados, long version)
            : base(carritoId, "Carrito", version)
        {
            CarritoId = carritoId;
            UsuarioId = usuarioId;
            CantidadItems = cantidadItems;
            TotalPerdido = totalPerdido;
            Motivo = motivo;
            ItemsEliminados = itemsEliminados;
        }
    }

    /// <summary>
    /// Información de item del carrito para eventos
    /// </summary>
    public record ItemCarritoInfo
    {
        public int ProductoId { get; init; }
        public string NombreProducto { get; init; } = string.Empty;
        public int Cantidad { get; init; }
        public decimal PrecioUnitario { get; init; }
        public decimal Subtotal { get; init; }

        public ItemCarritoInfo() { }

        public ItemCarritoInfo(int productoId, string nombreProducto, int cantidad, decimal precioUnitario)
        {
            ProductoId = productoId;
            NombreProducto = nombreProducto;
            Cantidad = cantidad;
            PrecioUnitario = precioUnitario;
            Subtotal = cantidad * precioUnitario;
        }
    }

    // ============= EVENTOS DEL SISTEMA =============

    /// <summary>
    /// Evento cuando se inicia sesión de usuario
    /// </summary>
    public record UsuarioInicioSesionEvent : EventBase
    {
        public string UsuarioId { get; init; } = string.Empty;
        public string NombreUsuario { get; init; } = string.Empty;
        public DateTime FechaHora { get; init; }
        public string IpOrigen { get; init; } = string.Empty;
        public new string UserAgent { get; init; } = string.Empty; // new para ocultar intencionalmente
        public bool EsExitoso { get; init; }

        public UsuarioInicioSesionEvent() { }

        public UsuarioInicioSesionEvent(string usuarioId, string nombreUsuario, string ipOrigen, string userAgent, bool esExitoso)
            : base(usuarioId, "Usuario", 1)
        {
            UsuarioId = usuarioId;
            NombreUsuario = nombreUsuario;
            FechaHora = DateTime.UtcNow;
            IpOrigen = ipOrigen;
            UserAgent = userAgent;
            EsExitoso = esExitoso;
        }
    }

    /// <summary>
    /// Evento cuando se intenta una operación no autorizada
    /// </summary>
    public record OperacionNoAutorizadaEvent : EventBase
    {
        public string UsuarioId { get; init; } = string.Empty;
        public string Operacion { get; init; } = string.Empty;
        public string Recurso { get; init; } = string.Empty;
        public string MotivoRechazo { get; init; } = string.Empty;
        public Dictionary<string, object> ParametrosOperacion { get; init; } = new();

        public OperacionNoAutorizadaEvent() { }

        public OperacionNoAutorizadaEvent(string usuarioId, string operacion, string recurso, string motivoRechazo)
            : base(usuarioId, "Seguridad", 1)
        {
            UsuarioId = usuarioId;
            Operacion = operacion;
            Recurso = recurso;
            MotivoRechazo = motivoRechazo;
        }
    }

    /// <summary>
    /// Evento cuando ocurre un error del sistema
    /// </summary>
    public record ErrorSistemaEvent : EventBase
    {
        public string TipoError { get; init; } = string.Empty;
        public string Mensaje { get; init; } = string.Empty;
        public string StackTrace { get; init; } = string.Empty;
        public string Operacion { get; init; } = string.Empty;
        public Dictionary<string, object> ContextoAdicional { get; init; } = new();

        public ErrorSistemaEvent() { }

        public ErrorSistemaEvent(string tipoError, string mensaje, string stackTrace, string operacion)
            : base(Guid.NewGuid().ToString(), "Sistema", 1)
        {
            TipoError = tipoError;
            Mensaje = mensaje;
            StackTrace = stackTrace;
            Operacion = operacion;
        }
    }

    /// <summary>
    /// Evento genérico para operaciones de dominio no específicamente mapeadas
    /// </summary>
    public record OperacionDominioEvent(
        string Operacion,
        string Entidad,
        string EntidadId,
        string Datos
    ) : EventBase(EntidadId, Entidad, 1)
    {
        public OperacionDominioEvent() : this("", "", "", "") { }
    }
}
