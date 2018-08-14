using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PathSegment {
    public Vector3 begin;
    public Vector3 end;
    public float weight;
    public string name;

    public PathSegment(Vector3 begin, Vector3 end, float weight, string name = "Segment") {
        this.begin = begin;
        this.end = end;
        this.weight = weight;
        this.name = name;
    }

    public PathSegment(Vector3 begin, Vector3 end, string name = "Segment") {
        this.begin = begin;
        this.end = end;
        this.weight = Util.manhattanWeight(begin, end);
        this.name = name;
    }

    public override string ToString() {
        return name + " " + begin + " => " + end + " [" + weight + "]";
    }
}

public struct Path {
    private List<PathSegment> segments;
    public float weight;

    public Path(List<PathSegment> segments) {
        this.segments = segments;

        if (segments.Count == 0) {
            weight = float.MaxValue;
        } else {
            float sum = 0;
            segments.ForEach(ps => sum += ps.weight);
            weight = sum;
        }
    }

    public Path(Path p) {
        this.segments = p.segments;
        this.weight = p.weight;
    }

    public void AddSegment(PathSegment segment) {
        if (segments.Count == 0) weight = 0;

        segments.Add(segment);
        weight += segment.weight;
    }

    public static Path Empty() {
        return new Path(new List<PathSegment>());
    }

    public bool IsEmpty() {
        if (segments == null) return true;
        return segments.Count == 0;
    }

    public Vector3 Beginning() {
        return segments[0].begin;
    }

    public Vector3 NextDestination() {
        return segments[0].end;
    }

    public Vector3 DestinationAt(int i) {
        return segments[i].end;
    }

    public Vector3 Ending() {
        return segments[segments.Count - 1].end;
    }

    public int Count() {
        return segments.Count;
    }

    public void RemoveFirstSegment() {
        segments.RemoveAt(0);
    }

    public static bool operator<(Path p1, Path p2) {
        return p1.weight < p2.weight;
    }

    public static bool operator>(Path p1, Path p2) {
        return p1.weight > p2.weight;
    }

    public static Path operator+(Path p1, Path p2) {
        Path p3 = new Path(p1);
        p2.segments.ForEach(s => p3.AddSegment(s));
        return p3;
    }

    public override string ToString() {
        string desc = "Path (" + segments.Count + ": " + weight + "):";
        segments.ForEach(s => desc += ("\n\t- " + s));
        return desc;
    }
}
