﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Pieces.Movers
{
    public interface IRiderCalc
    {
        ulong EmptyMask(int i);
        ulong CalculateMask(int i, ulong pieces);
    }
}
