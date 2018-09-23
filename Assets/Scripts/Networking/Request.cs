using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Networking
{
    public class Request
    {
        public string Type;
        public Guid Unit;
        public Vector3Int Target;
        public Guid Side;

        public Request(string type, Guid unit, Vector3Int target)
        {
            this.Type = type;
            this.Unit = unit;
            this.Target = target;
        }

        public Request()
        {

        }

        public static Request ToMove(Guid unit, Vector3Int destination)
        {
            return new Request("Move", unit, destination);
        }

        public static Request ToEndTurn(Guid side)
        {
            var request = new Request();
            request.Side = side;
            return request;
        }
    }
}
