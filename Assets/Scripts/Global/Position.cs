using System;
using UnityEngine;

[Serializable]
public class Position {
    public int x;
    public int y;

    public static Position Zero => new Position();

    public Position(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public Position() {
        x = 0;
        y = 0;
    }

    public Position(Position pos) {
        x = pos.x;
        y = pos.y;
    }

    public Position(Vector2Int pos) {
        x = pos.x;
        y = pos.y;
    }

    public static bool operator ==(Position a, Position b) {
        if (ReferenceEquals(a, b)) {
            return true;
        }
        if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) {
            return false;
        }
        return a.Equals(b);
    }

    public static bool operator !=(Position a, Position b) => !(a == b);

    public static Position operator +(Position a, Position b) {
        return new Position(a.x + b.x, a.y + b.y);
    }

    public static Position operator -(Position a) {
        return new Position(-a.x, -a.y);
    }

    public static Position operator -(Position a, Position b) {
        return new Position(a.x - b.x, a.y - b.y);
    }

    public bool Equals(Position other) {
        if (ReferenceEquals(other, null))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return x == other.x && y == other.y;
    }

    public override bool Equals(object obj) => Equals(obj as Position);

    public override int GetHashCode() {
        return HashCode.Combine(x, y);
    }

    public override string ToString() {
        return $"TilePos({x}, {y})";
    }
}