namespace ProyectoWebFutbol.Models.Dominio
{

        public class GolItem
        {
            public string Jugador { get; set; } = string.Empty;
            public string Minuto { get; set; } = string.Empty;

            // Convierte el objeto al formato final de texto para la base de datos
            public override string ToString()
            {
                if (string.IsNullOrWhiteSpace(Jugador)) return string.Empty;

                return string.IsNullOrWhiteSpace(Minuto)
                    ? Jugador.Trim()
                    : $"{Jugador.Trim()}, {Minuto.Trim()}";
            }

            // Parsea las cadenas guardadas tipo "Nombre, Minuto" desde la base de datos
            public static GolItem DesdeString(string texto)
            {
                if (string.IsNullOrWhiteSpace(texto)) return new GolItem();

                var partes = texto.Split(',');
                return new GolItem
                {
                    Jugador = partes.Length > 0 ? partes[0].Trim() : string.Empty,
                    Minuto = partes.Length > 1 ? partes[1].Trim() : string.Empty
                };
            }
        }
    }
