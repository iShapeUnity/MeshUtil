using Unity.Collections;
using UnityEngine;

namespace iShape.Mesh.Util {

    public struct NetBuilder {
        
        public static NativePlainMesh Build(NativeArray<Vector2> points, NativeArray<int> indices, float depth, Allocator allocator) {

            int maxIndex = 0;
            int n = indices.Length;
            for (int i = 0; i < n; ++i) {
                int j = indices[i];
                if (j > maxIndex) {
                    maxIndex = j;
                }
            }

            int multiply = maxIndex + 1;
            
            int maxVertexIndex = points.Length - 1;
            var map = new NativeHashMap<int, bool>(2, Allocator.Temp);

            for (int i = 0; i < n; i += 3) {
                for(int ib = 0, ia = 2; ib < 3; ia = ib++){
                    int a = indices[ia];
                    int b = indices[ib];
                    if (a > b) {
                        int c = a;
                        a = b;
                        b = c;
                    }
                }
                if (b > maxVertexIndex || a + 1 != b) {
                    int key = a * maxIndex + b;
                    map[key] = true;
                }


            }


        }
    }

}