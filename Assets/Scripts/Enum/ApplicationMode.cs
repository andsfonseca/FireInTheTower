using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintTower.Enum {
    /// <summary>
    /// Estados da Aplicação
    /// </summary>
    public enum ApplicationMode {
        /// <summary>
        /// Indica que a aplicação está pronta para entrar em modo de Host ou de Guest
        /// </summary>
        Ready,

        /// <summary>
        /// Indica que a aplicação está em modo de Host
        /// </summary>
        Hosting,

        /// <summary>
        /// Indica que a aplicação está em modo de Guest
        /// </summary>
        Resolving,
    }
}
