using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuneStuff
{
    public class Node
    {
        public Vector2 position, parent;

        public Node(Vector2 _position, Vector2 _parent)
        {
            position = _position;
            parent = _parent;
        }
    }
}
