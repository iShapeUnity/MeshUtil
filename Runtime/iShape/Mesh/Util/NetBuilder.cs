using Unity.Collections;
using UnityEngine;

namespace iShape.Mesh.Util {

    public struct NetBuilder {

        private struct Edge {
            internal readonly int a;
            internal readonly int b;

            internal Edge(int a, int b) {
                this.a = a;
                this.b = b;
            }
        }

        public static NativePlainMesh Build(NativeArray<Vector2> points, NativeArray<int> indices, float depth, Allocator allocator) {

            var edges = Convert(indices, Allocator.Temp);

            int n = edges.Length;
            var vertices = new NativeArray<Vector3>(4 * n, allocator);
            var triangles = new NativeArray<int>(6 * n, allocator);

            int vi = 0;
            int ti = 0;

            float r = 0.5f * depth;
            for (int i = 0, j = 0; i < edges.Length; ++i) {
                var edge = edges[i];
                var a = points[edge.a];
                var b = points[edge.b];
                var dir = (b - a).normalized * r;
                var normal = new Vector2(-dir.y, dir.x);

                triangles[ti++] = j;
                triangles[ti++] = j + 1;
                triangles[ti++] = j + 2;

                triangles[ti++] = j;
                triangles[ti++] = j + 2;
                triangles[ti++] = j + 3;
                
                var a0 = new Vector3(a.x - normal.x, a.y - normal.y);
                var a1 = new Vector3(a.x + normal.x, a.y + normal.y);
                var b1 = new Vector3(b.x + normal.x, b.y + normal.y);
                var b0 = new Vector3(b.x - normal.x, b.y - normal.y);
                
                vertices[j++] = a0;
                vertices[j++] = a1;
                vertices[j++] = b1;
                vertices[j++] = b0;
            }


            var nativeMesh = new NativePlainMesh {
                vertices = vertices,
                triangles = triangles
            };

            return nativeMesh;
        }

        /*
        private static NativeArray<Edge> Convert(NativeArray<int> indices, Allocator allocator) {
            int n = indices.Length;
            var edges = new NativeArray<Edge>(n, Allocator.Temp); 

            int counter = 0;
            int maxIndex = 0;
            for (int i = 0; i < n; i += 3) {
                for(int ib = 0, ia = 2; ib < 3; ia = ib++){
                    int a = indices[i + ia];
                    int b = indices[i + ib];
                    if (a > b) {
                        int c = a;
                        a = b;
                        b = c;
                    }

                    if (b > maxIndex) {
                        maxIndex = b;
                    }

                    edges[counter++] = new Edge(a, b);
                }
            }

            int m = maxIndex + 1;
            var indexCounter = new NativeArray<int>(m, Allocator.Temp);
            var maxCount = 0;
            for(int i = 0; i < counter; ++i) {
                var edge = edges[i];
                int count = indexCounter[edge.a] + 1;
                indexCounter[edge.a] = count;
                if (maxCount < count) {
                    maxCount = count;
                }
            }
            indexCounter.Dispose();

            int bufferLength = m * maxCount;
            var buffer = new NativeArray<int>(bufferLength, Allocator.Temp);
            for (int i = 0; i < bufferLength; ++i) {
                buffer[i] = -1;
            }

            var set = new NativeArray<Edge>(counter, Allocator.Temp);

            int edgeCounter = 0;
            
            for(int i = 0; i < counter; ++i) {
                var edge = edges[i];
                int index = edge.a * maxCount;
                for (int j = index; j < index + maxCount; ++j) {
                    var e = buffer[j];
                    if (e == -1) {
                        buffer[j] = edge.b;
                        set[edgeCounter++] = edge;
                        break;
                    }
                    if (e == edge.b) {
                        break;
                    }
                }
            }
            
            buffer.Dispose();

            var result = new NativeArray<Edge>(edgeCounter, allocator);
            result.Slice(0, edgeCounter).CopyFrom(set.Slice(0, edgeCounter));

            set.Dispose();
            
            return result;
        }
        */
        private static NativeArray<Edge> Convert(NativeArray<int> indices, Allocator allocator) {
            int n = indices.Length;
            var edges = new NativeArray<Edge>(n, allocator);
            for (int i = 0; i < n; i += 3) {
                int a = indices[i];
                int b = indices[i + 1];
                int c = indices[i + 2];
                edges[i] = new Edge(a, b);
                edges[i + 1] = new Edge(b,c);
                edges[i + 2] = new Edge(c,a);
            }

            return edges;
        }
    }


}