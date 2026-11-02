namespace API_luceritoStore.Models.Errores
{
    public class NegocioException : Exception
    {
        public ErrorNegocio Codigo { get; }

        public NegocioException(ErrorNegocio codigo, string mensaje) : base(mensaje)
        {
            Codigo = codigo;
        }
    }
}
