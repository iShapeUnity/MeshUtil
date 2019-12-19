using Unity.Collections;
using UnityEngine;

namespace iShape.MeshUtil {

    public struct NativePlainMesh {
        
        public NativeArray<Vector3> vertices;
        public NativeArray<int> triangles;

        public NativePlainMesh(NativePlainMesh a, NativePlainMesh b, Allocator allocator) {
            int aVxLen = a.vertices.Length;
            int bVxLen = b.vertices.Length;
            var vxs = new NativeArray<Vector3>(aVxLen + bVxLen, allocator);
            vxs.Slice(0, aVxLen).CopyFrom(a.vertices.Slice(0, aVxLen));
            vxs.Slice(aVxLen, bVxLen).CopyFrom(b.vertices.Slice(0, bVxLen));

            int aTrLen = a.triangles.Length;
            int bTrLen = b.triangles.Length;
            
            var trs = new NativeArray<int>(aTrLen + bTrLen, allocator);
            trs.Slice(0,aTrLen).CopyFrom(a.triangles.Slice(0, aTrLen));
            trs.Slice(aTrLen,bTrLen).CopyFrom(b.triangles.Slice(0, bTrLen));

            for (int i = aTrLen; i < aTrLen + bTrLen; ++i) {
                int prevIndex = trs[i]; 
                trs[i] = prevIndex + aVxLen;
            }

            this.vertices = vxs;
            this.triangles = trs;
        }


        public UnityEngine.Mesh Convert() {
            var mesh = new UnityEngine.Mesh {
                vertices = this.vertices.ToArray(),
                triangles = this.triangles.ToArray()
            };
            this.Dispose();

            return mesh;
        }

        public void Dispose() {
            this.vertices.Dispose();
            this.triangles.Dispose();            
        }

    }
}