using Unity.Collections;
using UnityEngine;

namespace iShape.Mesh.Util {

    public struct NativePlainMesh {
        
        public NativeArray<Vector3> vertices;
        public NativeArray<int> triangles;

        public UnityEngine.Mesh Convert() {
            var mesh = new UnityEngine.Mesh {
                vertices = this.vertices.ToArray(),
                triangles = this.triangles.ToArray()
            };
            this.vertices.Dispose();
            this.triangles.Dispose();

            return mesh;
        }

    }
}