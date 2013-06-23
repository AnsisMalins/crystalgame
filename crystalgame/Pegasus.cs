using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace crystalgame
{
    public class Pegasus : Entity
    {
        public bool WingsSpread { get; set; }

        public void Run(World world)
        {
            Position += Velocity;
        }
    }
}