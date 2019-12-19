using Unity.Collections;
using UnityEngine;

namespace iShape.MeshUtil {

	public struct PathBuilder {

        public static NativePlainMesh BuildClosedPath(NativeArray<Vector2> path, float depth, bool isClockWise, Allocator allocator
        ) {
            if (isClockWise) {
                return BuildClosedPath(path, depth, allocator);
            }

            var reversed = new NativeArray<Vector2>(path, Allocator.Temp);
            int j = path.Length - 1;
            int n = path.Length >> 1;
            for (int i = 0; i < n; ++i, --j) {
                var ai = reversed[i];
                var aj = reversed[j];
                reversed[i] = aj;
                reversed[j] = ai;
            }
            var result = BuildClosedPath(reversed, depth, allocator);
            reversed.Dispose();
            
            return result;
        }

        private static NativePlainMesh BuildClosedPath(NativeArray<Vector2> path, float depth, Allocator allocator) {
            int n = path.Length;

            var vertices = new NativeArray<Vector3>(4 * n, Allocator.Temp);
            int vxCnt = 0;
            var triangles = new NativeArray<int>(4 * 3 * n, Allocator.Temp);
            int trCnt = 0;

            float r = 0.5f * depth;

            Vector2 a = path[n - 2];
            Vector2 b = path[n - 1];

            var d0 = (b - a).normalized;
            var n0 = new Vector2(-d0.y, d0.x) * r;

            int left_0 = -1;

            int right_0 = -1;

            for(int i = 0; i < n; ++i) {
                int trIx0 = trCnt;
                trCnt += 6;

                var c = path[i];

                var d1 = (c - b).normalized;
                var n1 = new Vector2(-d1.y, d1.x) * r;

                float dot = d0.Multiply(d1);

                var isEqual = Mathf.Approximately(dot, 0f);
                int left_1;
                int left_2;
                int right_1;
                int right_2;
                if(!isEqual) {
                    if(dot < 0f) {
                        var cross = Cross(a - n0, b - n0, b - n1, c - n1);
                        if(dot > -0.5f) {
                            left_1 = vxCnt;
                            left_2 = vxCnt + 1;
                            right_1 = vxCnt + 2;
                            right_2 = right_1;

                            triangles[trCnt++] = left_1;
                            triangles[trCnt++] = left_2;
                            triangles[trCnt++] = right_1;

                            vertices[left_1] = b + n0;
                            vertices[left_2] = b + n1;
                            vertices[right_1] = cross;

                            vxCnt += 3;
                        } else {
                            left_1 = vxCnt;
                            left_2 = vxCnt + 2;
                            int m = vxCnt + 1;
                            right_1 = vxCnt + 3;
                            right_2 = right_1;

                            triangles[trCnt++] = left_1;
                            triangles[trCnt++] = m;
                            triangles[trCnt++] = right_1;

                            triangles[trCnt++] = m;
                            triangles[trCnt++] = left_2;
                            triangles[trCnt++] = right_1;

                            var vLeft0 = b + n0;
                            var vLeft1 = b + n1;

                            var vMiddle = b + (0.5f * (n0 + n1)).normalized * r;

                            vertices[left_1] = vLeft0;
                            vertices[m] = vMiddle;
                            vertices[left_2] = vLeft1;
                            vertices[right_1] = cross;

                            vxCnt += 4;
                        }
                    } else {
                        var cross = Cross(a + n0, b + n0, b + n1, c + n1);
                        if(dot < 0.5f) {
                            right_1 = vxCnt;
                            right_2 = vxCnt + 1;
                            left_1 = vxCnt + 2;
                            left_2 = left_1;

                            triangles[trCnt++] = right_1;
                            triangles[trCnt++] = left_1;
                            triangles[trCnt++] = right_2;

                            vertices[vxCnt++] = b - n0;
                            vertices[vxCnt++] = b - n1;
                            vertices[vxCnt++] = cross;
                        } else {
                            right_1 = vxCnt;
                            right_2 = vxCnt + 2;
                            int m = vxCnt + 1;
                            left_1 = vxCnt + 3;
                            left_2 = left_1;

                            triangles[trCnt++] = right_1;
                            triangles[trCnt++] = left_1;
                            triangles[trCnt++] = m;

                            triangles[trCnt++] = m;
                            triangles[trCnt++] = left_1;
                            triangles[trCnt++] = right_2;

                            var vRight0 = b - n0;
                            var vRight1 = b - n1;

                            var vMiddle = b - (0.5f * (n0 + n1)).normalized * r;

                            vertices[right_1] = vRight0;
                            vertices[m] = vMiddle;
                            vertices[right_2] = vRight1;
                            vertices[left_1] = cross;

                            vxCnt += 4;
                        }
                    }
                } else {
                    left_1 = vxCnt;
                    right_1 = vxCnt + 1;
                    left_2 = left_1;
                    right_2 = right_1;

                    vertices[vxCnt++] = b + n0;
                    vertices[vxCnt++] = b - n0;
                }

                a = b;
                b = c;
                n0 = n1;
                d0 = d1;

                triangles[trIx0++] = right_0;
                triangles[trIx0++] = left_0;
                triangles[trIx0++] = left_1;

                triangles[trIx0++] = right_0;
                triangles[trIx0++] = left_1;
                triangles[trIx0] = right_1;

                right_0 = right_2;
                left_0 = left_2;
            }

            triangles[0] = right_0;
            triangles[1] = left_0;
            triangles[3] = right_0;

            var slicedTriangles = new NativeArray<int>(trCnt, allocator);
            slicedTriangles.Slice(0, trCnt).CopyFrom(triangles.Slice(0, trCnt));
            triangles.Dispose();

            var slicedVertices = new NativeArray<Vector3>(vxCnt, allocator);
            slicedVertices.Slice(0, vxCnt).CopyFrom(vertices.Slice(0, vxCnt));
            vertices.Dispose();

            return new NativePlainMesh {
                vertices = slicedVertices,
                triangles = slicedTriangles
            };
        }


        private static Vector2 Cross(Vector2 a0, Vector2 a1, Vector2 b0, Vector2 b1) {
            Vector2 dA = a0 - a1;
            Vector2 dB = b0 - b1;

            float divider = dA.x * dB.y - dA.y * dB.x;

            float xyA = a0.x * a1.y - a0.y * a1.x;
            float xyB = b0.x * b1.y - b0.y * b1.x;

            float invert_divider = 1.0f / divider;

            float x = xyA * (b0.x - b1.x) - (a0.x - a1.x) * xyB;
            float y = xyA * (b0.y - b1.y) - (a0.y - a1.y) * xyB;

            return new Vector2(x * invert_divider, y * invert_divider);
        }
    }

    internal static class Vector2Extension {
        internal static float Multiply(this Vector2 self, Vector2 vector) {
            return self.x * vector.y - self.y * vector.x;
        }
    }
}