namespace iShape.MeshUtil {

    public struct MeshLayout {

        public struct Layout {
            public readonly int begin;
            public readonly int length;
            
            public Layout(int begin, int length) {
                this.begin = begin;
                this.length = length;
            }
        }

        public Layout vertexLayout;
        public Layout indexLayout;

        public MeshLayout(Layout vertexLayout, Layout indexLayout) {
            this.vertexLayout = vertexLayout;
            this.indexLayout = indexLayout;
        }
    }

}