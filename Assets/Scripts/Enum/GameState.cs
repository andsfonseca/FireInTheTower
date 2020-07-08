using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintTower.Enum {
    /// <summary>
    /// Estados do Jogo
    /// </summary>
    public enum GameState {
        MENU,
        HOST_WARNING,
        GUEST_WARNING,
        HOST_PREPARATION,
        GUEST_PREPARATION
    }
}
