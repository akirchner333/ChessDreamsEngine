using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Pieces.Movers
{
    public interface IMover
    {
        ulong MoveMask(int Index, Board board);
    }
    //In case it ever becomes useful to have a mover base class
    //public class Movers
    //{
    //}
}
