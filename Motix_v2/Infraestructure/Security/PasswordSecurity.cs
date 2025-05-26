using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Motix.Infraestructure.Security
{
    class PasswordSecurity
    {
        // Coste de trabajo (work factor) recomendado. Puedes ajustarlo si lo necesitas.
        private const int WorkFactor = 12;

        /// <summary>
        /// Genera un hash BCrypt de la contraseña en texto claro.
        /// </summary>
        /// <param name="password">Contraseña en texto claro.</param>
        /// <returns>Hash seguro que incluye la sal y el coste.</returns>
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
        }

        /// <summary>
        /// Verifica que una contraseña en texto claro coincida con el hash almacenado.
        /// </summary>
        /// <param name="password">Contraseña en texto claro.</param>
        /// <param name="hashedPassword">Hash almacenado (debe incluir sal y coste).</param>
        /// <returns>True si coincide, false en caso contrario.</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
